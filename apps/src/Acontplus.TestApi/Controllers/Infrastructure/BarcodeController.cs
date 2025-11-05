namespace Acontplus.TestApi.Controllers.Infrastructure;

using Acontplus.TestApi.Controllers.Core;

public class BarcodeController(IUsuarioService usuarioService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get(string text, bool includeLabel = false)
    {
        var barcodeConfig = new BarcodeConfig
        {
            Text = text ?? "0605202201030150819800120010030000012904948150712",
            Format = ZXing.BarcodeFormat.QR_CODE,
            IncludeLabel = includeLabel
        };
        var barcode = BarcodeGen.Create(barcodeConfig);

        //var response = await usuarioService.CreateAsync();

        return File(barcode, "image/png", "ci.png");
    }
}
