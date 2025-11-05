using Acontplus.Billing.Interfaces.Services;
using Acontplus.Billing.Models.Responses;

namespace Acontplus.Billing.Services.External;

public class WebServiceSri : IWebServiceSri
{
    //SRI AUTORIZA EL COMPROBANTE
    public async Task<ResponseSri> AuthorizationAsync(string claveAcceso, string url)
    {
        var responseSri = new ResponseSri();
        try
        {
            var xml =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">                         
                            <soapenv:Body>
                                <ec:autorizacionComprobante>
                                   <claveAccesoComprobante>{claveAcceso}</claveAccesoComprobante>
                                </ec:autorizacionComprobante>
                             </soapenv:Body>
                             </soapenv:Envelope>";

            using var sriService = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            using var response =
                await sriService.PostAsync(url, new StringContent(xml, Encoding.UTF8, "text/xml"));
            await using var streamResponse = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(streamResponse);
            responseSri.XmlSri = await streamReader.ReadToEndAsync();

            var doc = new XmlDocument();
            doc.LoadXml(responseSri.XmlSri);
            var estadoComp = doc.GetElementsByTagName("estado");

            var nlNroCompAuth = doc.GetElementsByTagName("numeroComprobantes");

            var nroComp = nlNroCompAuth[0] != null ? nlNroCompAuth[0].InnerText : string.Empty;

            responseSri.Estado = nroComp == "0" ? "NO AUTORIZADO" : estadoComp[0] != null ? estadoComp[0].InnerText : string.Empty;

            switch (responseSri.Estado)
            {
                case "AUTORIZADO":
                    {
                        var codAutorizacion = doc.GetElementsByTagName("numeroAutorizacion");
                        responseSri.CodigoAutorizacion = codAutorizacion[0]?.InnerText;
                        var xFecha = doc.GetElementsByTagName("fechaAutorizacion");
                        responseSri.FechaAutorizacion = xFecha[0]?.InnerText;
                        responseSri.Message = "EL COMPROBANTE FUE AUTORIZADO CON ÉXITO";
                        break;
                    }

                case "EN PROCESO":
                    {
                        responseSri.Message = "EL COMPROBANTE ESTA EN PROCESO";
                        break;
                    }

                default:
                    {
                        var xmessage = doc.GetElementsByTagName("mensaje");
                        if (xmessage.Count > 0)
                        {
                            var nodos = ((XmlElement)xmessage[0])?.ChildNodes;
                            if (nodos != null)
                                foreach (XmlElement nodo in nodos)
                                    switch (nodo.Name)
                                    {
                                        case "identificador":
                                            responseSri.Identificador = nodo.InnerText;
                                            break;
                                        case "mensaje":
                                            responseSri.Message = nodo.InnerText;
                                            break;
                                        case "informacionAdicional":
                                            responseSri.InformacionAdicional = nodo.InnerText;
                                            break;
                                        case "tipo":
                                            responseSri.Tipo = nodo.InnerText;
                                            break;
                                    }
                        }

                        break;
                    }
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
            responseSri.Estado = "ERROR";
            responseSri.Message = "No se pudo autorizar el comprobante";
        }

        return responseSri;
    }

    //SRI AUTORIZACION POR LOTE
    public async Task<ResponseSri> AuthorizationLoteAsync(string claveAcceso, string url)
    {
        var responseSri = new ResponseSri();
        try
        {
            var xml =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">                         
                            <soapenv:Body>
                                <ec:autorizacionComprobanteLote>
                                   <claveAccesoLote>{claveAcceso}</claveAccesoLote>
                                </ec:autorizacionComprobanteLote>
                             </soapenv:Body>
                             </soapenv:Envelope>";

            using var sriService = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            using var response = await sriService.PostAsync(url, new StringContent(xml, Encoding.UTF8, "text/xml"));
            await using var streamResponse = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(streamResponse);
            responseSri.XmlSri = await streamReader.ReadToEndAsync();

            var doc = new XmlDocument();
            doc.LoadXml(responseSri.XmlSri);
        }
        catch (Exception ex)
        {
            _ = ex.Message;
            responseSri.Estado = "ERROR";
            responseSri.Message = "No se pudo al autorizar el lote";
        }

        return responseSri;
    }

    //VERIFICA SI YA EXISTE EL COMPROBANTE EN EL SRI
    public async Task<ResponseSri> CheckExistenceAsync(string claveAcceso, string url)
    {
        var responseSri = new ResponseSri();
        try
        {
            var xml =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">                         
                            <soapenv:Body>
                                <ec:autorizacionComprobante>
                                   <claveAccesoComprobante>{claveAcceso}</claveAccesoComprobante>
                                </ec:autorizacionComprobante>
                             </soapenv:Body>
                             </soapenv:Envelope>";

            using var sriService = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            using var response =
                await sriService.PostAsync(url, new StringContent(xml, Encoding.UTF8, "text/xml"));
            await using var streamResponse = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(streamResponse);
            responseSri.XmlSri = await streamReader.ReadToEndAsync();

            var doc = new XmlDocument();
            doc.LoadXml(responseSri.XmlSri);

            var numeroComprobantes = doc.GetElementsByTagName("numeroComprobantes");
            if (numeroComprobantes.Count > 0)
            {
                if (numeroComprobantes[0]?.InnerText == "1")
                {
                    var xEstado = doc.GetElementsByTagName("estado");

                    switch (xEstado[0]?.InnerText)
                    {
                        case "AUTORIZADO":
                            {
                                responseSri.Estado = xEstado[0].InnerText;
                                responseSri.Message = "EL COMPROBANTE  YA FUE AUTORIZADO";
                                var xNumAuto = doc.GetElementsByTagName("numeroAutorizacion");
                                responseSri.CodigoAutorizacion = xNumAuto[0]?.InnerText;
                                var xFecha = doc.GetElementsByTagName("fechaAutorizacion");
                                responseSri.FechaAutorizacion = xFecha[0]?.InnerText;
                                break;
                            }
                        default:
                            {
                                responseSri.Estado = xEstado[0]?.InnerText;
                                var xmessage = doc.GetElementsByTagName("mensaje");
                                if (xmessage.Count > 0)
                                {
                                    var nodos = ((XmlElement)xmessage[0])?.ChildNodes;
                                    if (nodos != null)
                                        foreach (XmlElement nodo in nodos)
                                            switch (nodo.Name)
                                            {
                                                case "identificador":
                                                    responseSri.Identificador = nodo.InnerText;
                                                    break;
                                                case "mensaje":
                                                    responseSri.Message = nodo.InnerText;
                                                    break;
                                                case "informacionAdicional":
                                                    responseSri.InformacionAdicional = nodo.InnerText;
                                                    break;
                                                case "tipo":
                                                    responseSri.Tipo = nodo.InnerText;
                                                    break;
                                            }
                                }

                                break;
                            }
                    }
                }
                else
                {
                    responseSri.Estado = "NO EXISTE";
                }
            }
            else
            {
                var estadoNoAuth = doc.GetElementsByTagName("estado");
                if (estadoNoAuth.Count > 0) responseSri.Estado = estadoNoAuth[0]?.InnerText;
            }
        }
        catch (Exception ex)
        {
            responseSri.Estado = "ERROR";
            responseSri.Message = "No se pudo verificar la existencia del comprobante: " + ex;
        }

        return responseSri;
    }

    //DESCARGAR XML DESDE EL SRI
    public async Task<string> GetXmlAsync(string claveAcceso, string url)
    {
        var xmlSri = string.Empty;
        try
        {
            var xmlRequest = string.Format(
                @"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">                         
                            <soapenv:Body>
                                <ec:autorizacionComprobante>
                                   <claveAccesoComprobante>{0}</claveAccesoComprobante>
                                </ec:autorizacionComprobante>
                             </soapenv:Body>
                             </soapenv:Envelope>", claveAcceso);

            using var sriLClient = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            using var response =
                await sriLClient.PostAsync(url, new StringContent(xmlRequest, Encoding.UTF8, "text/xml"));
            await using var streamResponse = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(streamResponse);
            xmlSri = await streamReader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }

        return xmlSri;
    }

    //SRI RECIBE EL XML DE LOS COMPROBANTES 
    public async Task<ResponseSri> ReceptionAsync(string xmlSigned, string url)
    {
        var responseSri = new ResponseSri();
        try
        {
            var xml =
                $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:ec=""http://ec.gob.sri.ws.recepcion"">                         
                            <soapenv:Header/>                            
                            <soapenv:Body>
                               <ec:validarComprobante>
                                   <xml>{Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlSigned))}</xml>
                                </ec:validarComprobante>
                             </soapenv:Body>
                             </soapenv:Envelope>";


            using var sriService = new HttpClient(new HttpClientHandler { UseDefaultCredentials = true });
            using var response =
                await sriService.PostAsync(url, new StringContent(xml, Encoding.UTF8, "text/xml"));
            await using var streamResponse = await response.Content.ReadAsStreamAsync();
            using var streamReader = new StreamReader(streamResponse);
            responseSri.XmlSri = await streamReader.ReadToEndAsync();

            if (DataValidation.IsXml(responseSri.XmlSri))
            {
                //OBTIENE DATO DEL XML RESPONSE                    
                var xdoc = new XmlDocument();
                xdoc.LoadXml(responseSri.XmlSri);

                var xEstado = xdoc.GetElementsByTagName("estado");
                responseSri.Estado = xEstado[0] != null ? xEstado[0].InnerText : string.Empty;

                var identificador = xdoc.GetElementsByTagName("identificador");
                responseSri.Identificador = identificador[0] != null ? identificador[0].InnerText : string.Empty;

                if (responseSri.Estado == "DEVUELTA")
                {
                    xEstado = xdoc.GetElementsByTagName("mensaje");
                    var nodos = ((XmlElement)xEstado[0])?.ChildNodes;
                    if (nodos != null)
                        foreach (XmlElement nodo in nodos)
                            switch (nodo.Name)
                            {
                                case "identificador":
                                    responseSri.Identificador = nodo.InnerText;
                                    break;
                                case "mensaje":
                                    responseSri.Message = nodo.InnerText;
                                    break;
                                case "informacionAdicional":
                                    responseSri.InformacionAdicional = nodo.InnerText;
                                    break;
                                case "tipo":
                                    responseSri.Tipo = nodo.InnerText;
                                    break;
                            }
                }
            }
            else
            {
                responseSri.Estado = "ERROR";
                responseSri.Message = "SRI no se encuentra en línea";
            }
        }
        catch (Exception)
        {
            responseSri.Estado = "ERROR";
            responseSri.Message = "SRI no se encuentra en línea";
        }

        return responseSri;
    }
}