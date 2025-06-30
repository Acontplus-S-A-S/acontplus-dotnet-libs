using Acontplus.Core.DTOs;

namespace Acontplus.Core.Validation;

public class ValidacionesExternas

{
    private readonly ApiResponse _gr = new();

    internal ApiResponse VerificarDatos(string numeroIdentificacion, string tipoValidacion)
    {
        _ = new char[13];
        try
        {
            switch (tipoValidacion)
            {
                case "IDENTI":

                    if (numeroIdentificacion.Length >= 10)
                    {
                        var valced = numeroIdentificacion.Trim().ToCharArray();
                        var provincia = int.Parse(valced[0] + valced[1].ToString());

                        if (provincia is > 0 and < 25)
                        {
                            if (int.Parse(valced[2].ToString()) < 6 && numeroIdentificacion.Length <= 10)
                            {
                                VerificarCedula(valced);
                            }

                            if (numeroIdentificacion.Length == 13)
                            {
                                var digito = int.Parse(valced[10].ToString()) + int.Parse(valced[11].ToString()) +
                                             int.Parse(valced[12].ToString());

                                if (digito == 001)
                                {
                                    switch (int.Parse(valced[2].ToString()))
                                    {
                                        case < 6:
                                            VerificarPersonaNarutal(valced);
                                            break;
                                        case 6:
                                            VerificarSectorPublico(valced);
                                            break;
                                        case 9:
                                            VerificarPersonaJuridica(valced);
                                            break;
                                    }
                                }
                                else
                                {
                                    _gr.Code = "0";

                                    _gr.Message = "Los últimos tres dígitos de un Ruc siempre serán: 001";
                                }
                            }
                        }
                        else
                        {
                            _gr.Code = "0";

                            _gr.Message = "Numero de identificacion fuera de los codigos de la Provincia";
                        }
                    }
                    else
                    {
                        _gr.Code = "0";

                        _gr.Message = "Cantidad de numeros no es correcta";
                    }

                    break;
            }
        }
        catch (Exception ex)
        {
            _gr.Code = "0";
            _gr.Message = ex.Message;
        }

        return _gr;
    }

    private ApiResponse VerificarCedula(char[] validarCedula)
    {
        try
        {
            int aux = 0, par = 0, impar = 0, verifi;
            for (var i = 0; i < 9; i += 2)
            {
                aux = 2 * int.Parse(validarCedula[i].ToString());
                if (aux > 9)
                {
                    aux -= 9;
                }

                par += aux;
            }

            for (var i = 1; i < 9; i += 2)
            {
                impar += int.Parse(validarCedula[i].ToString());
            }

            aux = par + impar;
            if (aux % 10 != 0)
            {
                verifi = 10 - aux % 10;
            }
            else
            {
                verifi = 0;
            }

            if (verifi == int.Parse(validarCedula[9].ToString()))
            {
                _gr.Code = "1";

                _gr.Message = "Cedula de Identidicación";
            }
            else
            {
                _gr.Code = "0";

                _gr.Message = "Numero de Cedula ingresado incorrectamente";
            }
        }
        catch (Exception ex)
        {
            _gr.Code = "0";
            _gr.Message = ex.Message;
        }

        return _gr;
    }

    private ApiResponse VerificarPersonaNarutal(char[] validarCedula)
    {
        try
        {
            int aux = 0, par = 0, impar = 0, verifi;
            for (var i = 0; i < 9; i += 2)
            {
                aux = 2 * int.Parse(validarCedula[i].ToString());
                if (aux > 9)
                {
                    aux -= 9;
                }

                par += aux;
            }

            for (var i = 1; i < 9; i += 2)
            {
                impar += int.Parse(validarCedula[i].ToString());
            }

            aux = par + impar;
            if (aux % 10 != 0)
            {
                verifi = 10 - aux % 10;
            }
            else
            {
                verifi = 0;
            }

            if (verifi == int.Parse(validarCedula[9].ToString()))
            {
                _gr.Code = "1";

                _gr.Message = "Ruc Persona Natural";
            }
            else
            {
                _gr.Code = "0";

                _gr.Message = "Ruc de Persona Natural ingresado incorrectamente";
            }
        }
        catch (Exception ex)
        {
            _gr.Code = "0";
            _gr.Message = ex.Message;
        }

        return _gr;
    }

    private ApiResponse VerificarPersonaJuridica(char[] validarCedula)
    {
        try
        {
            var aux = 0;
            var veri = int.Parse(validarCedula[10].ToString()) + int.Parse(validarCedula[11].ToString()) +
                       int.Parse(validarCedula[12].ToString());
            if (veri > 0)
            {
                var coeficiente = new[] { 4, 3, 2, 7, 6, 5, 4, 3, 2 };
                for (var i = 0; i < 9; i++)
                {
                    var prod = int.Parse(validarCedula[i].ToString()) * coeficiente[i];
                    aux += prod;
                }

                if (aux % 11 == 0)
                {
                    veri = 0;
                }
                else if (aux % 11 == 1)
                {
                    _gr.Code = "0";

                    _gr.Message = "Numero de identificacion Persona Juridica ingresado incorrectamente";
                }
                else
                {
                    aux %= 11;
                    veri = 11 - aux;
                }

                if (veri == int.Parse(validarCedula[9].ToString()))
                {
                    _gr.Code = "1";

                    _gr.Message = "Ruc De Persona Juridica";
                }
                else
                {
                    _gr.Code = "0";

                    _gr.Message = "Numero de identificacion Persona Juridica ingresado ingresado incorrectamente";
                }
            }
            else
            {
                _gr.Code = "0";

                _gr.Message = "Numero de identificacion Persona Juridica ingresado ingresado incorrectamente";
            }
        }
        catch (Exception ex)
        {
            _gr.Code = "0";
            _gr.Message = ex.Message;
        }

        return _gr;
    }

    private ApiResponse VerificarSectorPublico(char[] validarCedula)
    {
        try
        {
            var aux = 0;
            var veri = int.Parse(validarCedula[9].ToString()) + int.Parse(validarCedula[10].ToString()) +
                       int.Parse(validarCedula[11].ToString()) + int.Parse(validarCedula[12].ToString());
            if (veri > 0)
            {
                var coeficiente = new[] { 3, 2, 7, 6, 5, 4, 3, 2 };
                for (var i = 0; i < 8; i++)
                {
                    var prod = int.Parse(validarCedula[i].ToString()) * coeficiente[i];
                    aux += prod;
                }

                switch (aux % 11)
                {
                    case 0:
                        veri = 0;
                        break;
                    case 1:
                        _gr.Code = "0";

                        _gr.Message = "Numero de identificacion Sector Publico ingresado incorrectamente";
                        break;
                    default:
                        aux %= 11;
                        veri = 11 - aux;
                        break;
                }

                if (veri == int.Parse(validarCedula[8].ToString()))
                {
                    _gr.Code = "1";

                    _gr.Message = "Ruc Del Sector Publico";
                }
                else
                {
                    _gr.Code = "0";

                    _gr.Message = "Numero de identificacion Sector Publico ingresado incorrectamente";
                }
            }
            else
            {
                _gr.Code = "0";

                _gr.Message = "Numero de identificacion Sector Publico ingresado incorrectamente";
            }
        }
        catch (Exception ex)
        {
            _gr.Code = "0";
            _gr.Message = ex.Message;
        }

        return _gr;
    }
}
