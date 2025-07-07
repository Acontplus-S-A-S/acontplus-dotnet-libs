namespace Acontplus.FactElect.Services.Conversion;


// Factory to create document parsers based on document type
public class DocumentParserFactory
{
    private readonly IDetailsParser _detailsParser;

    public DocumentParserFactory(IDetailsParser detailsParser)
    {
        _detailsParser = detailsParser ?? throw new ArgumentNullException(nameof(detailsParser));
    }

    public IDictionary<string, IDocumentTypeParser> CreateDocumentParsers()
    {
        return new Dictionary<string, IDocumentTypeParser>
        {
            { "01", new FacturaDocumentParser(_detailsParser) },
            // Add other parser implementations for different document types
            // { "03", new LiquidacionCompraParser() },
            // { "04", new NotaCreditoParser(_detailsParser) },
            // etc.
        };
    }
}