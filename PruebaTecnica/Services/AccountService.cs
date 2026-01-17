using Microsoft.EntityFrameworkCore;
using PruebaTecnica.Data;
using PruebaTecnica.DTOs;
using PruebaTecnica.Models;
using System;

namespace PruebaTecnica.Services
{
    public class AccountService : IAccountService
    {
        private readonly PDbContext _context;

        public AccountService(PDbContext context)
        {
            _context = context;
        }

        //Crear cuenta por DNI
        public Cuenta CrearCuenta(string dni, decimal saldoInicial)
        {
            var dniLimpio = dni.Trim();

            //Validar que el saldo inicial sea mayor que cero
            if (saldoInicial <= 0)
                throw new InvalidOperationException("El saldo inicial debe ser mayor que cero");

            //Buscar el cliente por DNI
            var cliente = _context.Clientes.FirstOrDefault(c => c.DNI == dniLimpio);
            if (cliente == null)
                throw new InvalidOperationException($"No existe un cliente con el DNI {dniLimpio}");

            //Crear la cuenta asociada al cliente
            var cuenta = new Cuenta
            {
                clienteId = cliente.Id,
                numeroCuenta = GenerarNumeroCuenta(),
                saldo = saldoInicial,
                cliente = cliente
            };

            _context.Cuentas.Add(cuenta);
            _context.SaveChanges();

            //Retornar la cuenta con el cliente incluido
            return _context.Cuentas
                .Include(c => c.cliente)
                .FirstOrDefault(c => c.Id == cuenta.Id);
        }



        //Consultar saldo por número de cuenta
        public decimal ConsultarSaldo(string numeroCuenta)
        {
            var cuenta = _context.Cuentas.FirstOrDefault(c => c.numeroCuenta == numeroCuenta);
            return cuenta?.saldo ?? 0;
        }

        //Generar número de cuenta único
        private string GenerarNumeroCuenta()
        {
            var random = new Random();
            string numero;

            do
            {
                numero = random.Next(1000000000, int.MaxValue).ToString();
            }
            while (_context.Cuentas.Any(c => c.numeroCuenta == numero)); //verifica si ya existe

            return numero;
        }

        //Obtener cuentas por ClienteId (si aún lo necesitas)
        public List<Cuenta> ObtenerCuentasPorClienteId(long clienteId)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Id == clienteId);
            if (cliente == null)
                throw new InvalidOperationException($"No existe un cliente con Id {clienteId}");

            return _context.Cuentas
                .Include(c => c.cliente)
                .Where(c => c.clienteId == clienteId)
                .ToList();
        }


        //Obtener cuentas por DNI
        public List<Cuenta> ObtenerCuentasPorDNI(string dni)
        {
            var dniLimpio = dni.Trim();

            var cliente = _context.Clientes.FirstOrDefault(c => c.DNI == dniLimpio);
            if (cliente == null)
                throw new InvalidOperationException($"No existe un cliente con el DNI {dniLimpio}");

            return _context.Cuentas
                .Include(c => c.cliente)
                .Where(c => c.cliente.DNI == dniLimpio)
                .ToList();
        }

        //Aplicación de interes

        public InteresResultadoDTO AplicarIntereses(string numeroCuenta, decimal tasaPorcentaje)
        {
            if (tasaPorcentaje <= 0)
                throw new InvalidOperationException("La tasa de interés debe ser mayor que cero y expresada en porcentaje");

            // Buscar la cuenta por número de cuenta
            var cuenta = _context.Cuentas
                .Include(c => c.cliente) // incluir datos del cliente
                .FirstOrDefault(c => c.numeroCuenta == numeroCuenta);

            if (cuenta == null)
                throw new InvalidOperationException($"No existe una cuenta con el número {numeroCuenta}");

            var saldoAnterior = cuenta.saldo;
            var tasaDecimal = tasaPorcentaje / 100;
            var interes = saldoAnterior * tasaDecimal;

            cuenta.saldo += interes;

            _context.Transacciones.Add(new Transaccion
            {
                cuentaId = cuenta.Id,
                monto = interes,
                tipoTransaccion = "Interes",
                fecha = DateTime.Now
            });

            _context.SaveChanges();

            return new InteresResultadoDTO
            {
                clienteId = cuenta.clienteId,
                SaldoAnterior = saldoAnterior,
                SaldoFinal = cuenta.saldo
            };
        }



    }
}
