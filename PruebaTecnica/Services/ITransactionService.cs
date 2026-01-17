using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    public interface ITransactionService
    {
        Transaccion Depositar(string numeroCuenta, decimal monto);
        Transaccion Retirar(string numeroCuenta, decimal monto);
        List<Transaccion> ObtenerHistorial(string numeroCuenta);
    }

}
