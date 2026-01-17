using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Data;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly PDbContext _context;

        public TransactionService(PDbContext context)
        {
            _context = context;
        }

        public Transaccion Depositar(string numeroCuenta, decimal monto)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.numeroCuenta == numeroCuenta);
            cuenta.saldo += monto;

            var transaccion = new Transaccion
            {
                cuentaId = cuenta.Id,
                tipoTransaccion = "Deposito",
                monto = monto,
                saldoFinal = cuenta.saldo,
                fecha = DateTime.Now
            };

            _context.Transacciones.Add(transaccion);
            _context.SaveChanges();
            return transaccion;
        }

        public Transaccion Retirar(string numeroCuenta, decimal monto)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.numeroCuenta == numeroCuenta);
            if (cuenta.saldo < monto)
                throw new InvalidOperationException("Fondos insuficientes");

            cuenta.saldo -= monto;

            var transaccion = new Transaccion
            {
                cuentaId = cuenta.Id,
                tipoTransaccion = "Retiro",
                monto = monto,
                saldoFinal = cuenta.saldo,
                fecha = DateTime.Now
            };

            _context.Transacciones.Add(transaccion);
            _context.SaveChanges();
            return transaccion;
        }


        public List<Transaccion> ObtenerHistorial(string numeroCuenta)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.numeroCuenta == numeroCuenta);
            return _context.Transacciones
                .Where(t => t.cuentaId == cuenta.Id)
                .OrderByDescending(t => t.fecha)
                .ToList();
        }
    }

}
