namespace Acontplus.TestHostApi.Controllers;

public class CustomerController(
    IRucService rucService,
    ICedulaService cedulaService,
    ICustomerService customerService)
    : BaseApiController
{
    [HttpGet("GetRucSri")]
    public async Task<RucModel> GetRucSri(string ruc, bool sriOnly = false)
    {
        if (sriOnly)
        {
            return await rucService.GetRucSriAsync(ruc);
        }

        var parameters = new Dictionary<string, object> { { "id", ruc } };
        var dt = await customerService.GetByIdCardAsync(parameters);

        var serialized = DataConverters.DataTableToJson(dt);

        var dictionarySimple = new Dictionary<string, object>
        {
            {"id", 0 },
            {"tiempo", "sasas" }
        };

        string json = @"{
            ""infoFactura"": {
                ""idCliente"": 268143,
                ""idTiempoCredito"": 17,
                ""idPuntoEmision"": 6,
                ""idUsuario"": 14,
                ""fechaEmision"": ""17-04-2025 12:59 PM"",
                ""fechaVencimiento"": ""02-05-2025 12:39 PM"",
                ""subTotalSinImpuestos"": 26.09,
                ""totalDescuento"": 0,
                ""porcentajeDescuento"": 0,
                ""irbpnr"": 0,
                ""propina"": 0,
                ""recargo"": 0,
                ""transporte"": 0,
                ""total"": 30,
                ""idTipoIdentificacion"": 1,
                ""identificacionComprador"": ""1105246605"",
                ""razonSocialComprador"": ""PAZ CHAMBA IVAN FERNANDO"",
                ""telefono"": ""9890297817""
            },
            ""detalles"": [
                {
                    ""orderIndex"": 1,
                    ""uuid"": ""d3eb8ead-adc5-61a7-c846-c1cad8dfcd55"",
                    ""idFacturaDetalle"": 0,
                    ""idArticulo"": 267,
                    ""idTarifa"": 23,
                    ""idBodega"": 5,
                    ""idMedida"": 8,
                    ""idTipoIva"": 2008,
                    ""tarifa"": 15,
                    ""codigoPorcentaje"": ""4"",
                    ""ice"": 0,
                    ""descripcion"": ""ALQUILER HABITACION 101 SIMPLE"",
                    ""cantidad"": 1,
                    ""precioSinSubsidio"": 0,
                    ""precioUnitario"": 26.09,
                    ""valorDescuento"": 0,
                    ""precioTotalSinImpuesto"": 26.09,
                    ""porcentajeDescuento"": 0,
                    ""nota"": null,
                    ""llevaInventario"": false,
                    ""precioCosto"": 10,
                    ""precioCostoFraccionado"": 10,
                    ""conversion"": 1,
                    ""cantidadTransaccion"": 1,
                    ""estado"": 1,
                    ""valorImpuesto"": 3.9135
                }
            ],
            ""impuestos"": [
                {
                    ""taxId"": 1,
                    ""taxPercentId"": 2008,
                    ""percent"": 15,
                    ""taxBase"": 26.09,
                    ""taxValue"": 3.9135,
                    ""discount"": 0
                }
            ],
            ""infoAdicional"": [],
            ""infoAdicionalArticulo"": [],
            ""formaPago"": {
                ""idFormaPagoSri"": 1,
                ""idTiempoCredito"": 17,
                ""plazo"": 15,
                ""total"": 30,
                ""infoReferencia"": ""Ref.: EFECTIVO""
            },
            ""cuotas"": [
                {
                    ""nroCuota"": 1,
                    ""cuota"": 30,
                    ""fechaVencimiento"": ""02-05-2025 12:59 PM"",
                    ""entrada"": 0
                }
            ]
        }";

        var dictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);




        var serializeDictionary = DataConverters.SerializeDictionary(dictionary);

        //var serializeDictionary = DataConverters.SerializeDictionary(dictionary);



        if (!DataValidation.DataTableIsNull(dt))
        {
            return DataTableMapper.MapDataRowToModel<RucModel>(dt.Rows[0]);
        }

        return await rucService.GetRucSriAsync(ruc);
    }
    [HttpGet("GetCedulaSri")]
    public async Task<CedulaModel> GetCedulaSri(string ruc, bool sriOnly = false)
    {
        if (sriOnly)
        {
            return await cedulaService.GetCedulaSriAsync(ruc);
        }

        var parameters = new Dictionary<string, object> { { "id", ruc }, { "IDType", "05" } };
        var dt = await customerService.GetByIdCardAsync(parameters);

        if (!DataValidation.DataTableIsNull(dt))
        {
            return DataTableMapper.MapDataRowToModel<CedulaModel>(dt.Rows[0]);
        }

        return await cedulaService.GetCedulaSriAsync(ruc);
    }
}
