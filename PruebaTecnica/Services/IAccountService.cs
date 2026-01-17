using PruebaTecnica.DTOs;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    public interface IAccountService
    {
        Cuenta CrearCuenta(string dni, decimal saldoInicial);
        decimal ConsultarSaldo(string numeroCuenta);
        List<Cuenta> ObtenerCuentasPorClienteId(long clienteId);
        List<Cuenta> ObtenerCuentasPorDNI(string dni);
        InteresResultadoDTO AplicarIntereses(string numeroCuenta, decimal tasaPorcentaje);

    }

}
