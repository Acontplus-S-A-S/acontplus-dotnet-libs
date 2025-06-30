using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;
using System.Web;
using static System.Enum;

namespace Acontplus.Reports.Services
{
    public class RdlcReportService(IConfiguration configuration) : IRdlcReportService, IDisposable
    {
        private readonly ConcurrentDictionary<string, Lazy<MemoryStream>> _reportCache = new();
        private bool _disposed;
        private readonly string _mainDirectory = configuration["Reports:MainDirectory"]!;

        public ReportResponse GetReport(DataSet parameters, DataSet data, bool externalDirectory)
        {
            var reportProps = DataTableMapper.MapDataRowToModel<ReportProps>(parameters.Tables["ReportProps"].Rows[0]);

            using var lr = new LocalReport();
            var reportPath = GetReportPath(reportProps, externalDirectory);

            var reportDefinitionStream = _reportCache.GetOrAdd(reportPath, path =>
            {
                return new Lazy<MemoryStream>(() =>
                {
                    var stream = RdlcHelpers.LoadReportDefinition(path);
                    return stream;
                });
            }).Value;

            reportDefinitionStream.Seek(0, SeekOrigin.Begin); // Ensure the stream position is reset before reading
            lr.LoadReportDefinition(reportDefinitionStream);

            AddDataSources(lr, parameters, data);
            AddReportParameters(lr, parameters, data);

            var fileReport = lr.Render(reportProps.ReportFormat, null, out _, out _, out _, out _, out _);
            var response = BuildReportResponse(reportProps, fileReport);

            return response;
        }

        private string GetReportPath(ReportProps reportProps, bool offline)
        {
            if (offline)
            {
                var value = configuration["Reports:ExternalDirectory"];
                if (value != null)
                {
                    var reportPath = reportProps.ReportPath.TrimStart('/', '\\');
                    return Path.Combine(value, reportPath);
                }
            }
            else
            {
                var mainPath = Path.Combine(Directory.GetCurrentDirectory(), _mainDirectory);
                var paths = reportProps.ReportPath.Split("/");
                paths = paths.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                return paths.Length switch
                {
                    1 => Path.Combine(mainPath, paths[0]),
                    2 => Path.Combine(mainPath, paths[0], paths[1]),
                    _ => Path.Combine(mainPath, reportProps.ReportPath),
                };
            }

            return string.Empty;
        }

        private void AddDataSources(LocalReport lr, DataSet parameters, DataSet data)
        {
            if (parameters.Tables.Contains("DataSources"))
            {
                var dataSources = parameters.Tables["DataSources"];
                if (dataSources != null)
                {
                    foreach (DataRow row in dataSources.Rows)
                    {
                        lr.DataSources.Add(new ReportDataSource(row["dataSource"].ToString(),
                            data.Tables[row.Field<int>("position")]));
                    }
                }
            }
            else
            {
                var dataSourceNames = lr.GetDataSourceNames();
                foreach (var dataSourceName in dataSourceNames)
                {
                    lr.DataSources.Add(new ReportDataSource(dataSourceName, data.Tables[dataSourceName]));
                }
            }
        }

        private void AddReportParameters(LocalReport lr, DataSet parameters, DataSet data)
        {
            var firstDataSource = data.Tables[0];
            if (firstDataSource.Columns.Contains("codigoAutorizacion"))
            {
                var barcodeConfig = new BarcodeConfig
                {
                    Text = firstDataSource.Rows[0].Field<string>("codigoAutorizacion")
                };
                var byteBarcode = BarcodeGen.Create(barcodeConfig);
                lr.SetParameters(new ReportParameter("barcode", Convert.ToBase64String(
                    byteBarcode,
                    0,
                    byteBarcode.Length)));
                lr.SetParameters(new ReportParameter("mimeTypeBarcode", "image/png"));
            }

            if (parameters.Tables.Contains("ReportParams") && parameters.Tables["ReportParams"]!.Rows.Count > 0)
            {
                foreach (DataRow item in parameters.Tables["ReportParams"].Rows)
                {
                    var paramValue = "";
                    if (Convert.ToBoolean(item["isPicture"]))
                    {
                        paramValue = item.Field<bool>("isCompressed")
                            ? FileExtensions.GetBase64FromByte(
                                CompressionUtils.DecompressGZip((byte[])item["paramValue"]))
                            : FileExtensions.GetBase64FromByte((byte[])item["paramValue"]);
                        lr.SetParameters(new ReportParameter(item["paramName"].ToString(), paramValue));
                    }
                    else
                    {
                        paramValue = Encoding.UTF8.GetString(item.Field<byte[]>("paramValue"));
                    }

                    lr.SetParameters(new ReportParameter(item["paramName"].ToString(), paramValue));
                }
            }
        }

        private ReportResponse BuildReportResponse(ReportProps reportProps, byte[] fileReport)
        {
            TryParse(reportProps.ReportFormat.ToUpper(), out FileFormats.FileContentType fc);
            TryParse(reportProps.ReportFormat.ToUpper(), out FileFormats.FileExtension fe);

            var response = new ReportResponse
            {
                FileContents = fileReport,
                ContentType = fc.DisplayName(),
                FileDownloadName = HttpUtility.UrlEncode(reportProps.ReportName + fe.DisplayName(), Encoding.UTF8)
            };
            return response;
        }

        public async Task<ReportResponse> GetErrorAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "NotFound.pdf");
            var fileContents = await File.ReadAllBytesAsync(filePath);
            return new ReportResponse
            {
                FileContents = fileContents,
                ContentType = "application/pdf",
                FileDownloadName = "Not Found.pdf"
            };
        }

        private void CleanupCache()
        {
            foreach (var lazyMemoryStream in _reportCache.Values)
            {
                if (lazyMemoryStream.IsValueCreated)
                {
                    lazyMemoryStream.Value.Dispose();
                }
            }

            _reportCache.Clear();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                CleanupCache();
            }

            _disposed = true;
        }

        ~RdlcReportService()
        {
            Dispose(false);
        }
    }
}
