using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PruebaTecnica.DTOs;
using PruebaTecnica.Models;
using PruebaTecnica.Services;

namespace PruebaTecnica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CuentasController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public CuentasController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("Crear_Cuenta_Por_DNI")]
        public IActionResult CrearCuentaPorDNI(string dni, decimal saldoInicial)
        {
            try
            {
                var cuenta = _accountService.CrearCuenta(dni, saldoInicial);

                return Ok(new
                {
                    clienteId = cuenta.clienteId,
                    NumeroCuenta = cuenta.numeroCuenta,
                    Saldo = cuenta.saldo,
                    ClienteDNI = cuenta.cliente.DNI,
                    ClienteNombre = cuenta.cliente.nombre
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }



        [HttpGet("Consulta_Saldo_Por_Numero_Cuenta")]
        public IActionResult ConsultarSaldo(string numeroCuenta)
        {
            var saldo = _accountService.ConsultarSaldo(numeroCuenta);
            return Ok("Saldo: " + saldo);
        }


        [HttpGet("Consulta_Cuentas_Por_IDCliente")]
        public IActionResult ObtenerCuentasPorClienteId(long clienteId)
        {
            try
            {
                var cuentas = _accountService.ObtenerCuentasPorClienteId(clienteId);

                if (cuentas == null || !cuentas.Any())
                    return NotFound($"El cliente con Id {clienteId} no tiene cuentas registradas");

                var response = cuentas.Select(c => new
                {
                    NumeroCuenta = c.numeroCuenta,
                    Saldo = c.saldo,
                    ClienteId = c.clienteId,
                    ClienteDNI = c.cliente.DNI,
                    ClienteNombre = c.cliente.nombre
                });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }



        [HttpGet("Consulta_Cuentas_Por_DNI")]
        public IActionResult ObtenerCuentasPorDNI(string dni)
        {
            try
            {
                var cuentas = _accountService.ObtenerCuentasPorDNI(dni);

                if (cuentas == null || !cuentas.Any())
                    return NotFound($"El cliente con DNI {dni} no tiene cuentas registradas");

                var response = cuentas.Select(c => new
                {
                    NumeroCuenta = c.numeroCuenta,
                    Saldo = c.saldo,
                    ClienteId = c.clienteId,
                    ClienteDNI = c.cliente.DNI,
                    ClienteNombre = c.cliente.nombre
                });

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
        }


        [HttpPost("Aplicar_Intereses")]
        public IActionResult AplicarIntereses(string numeroCuenta, decimal tasaPorcentaje)
        {
            try
            {
                var resultado = _accountService.AplicarIntereses(numeroCuenta, tasaPorcentaje);

                return Ok(new
                {
                    NumeroCuenta = numeroCuenta,
                    ClienteId = resultado.clienteId,
                    TasaAplicada = $"{tasaPorcentaje}%",
                    SaldoAnterior = resultado.SaldoAnterior,
                    SaldoFinal = resultado.SaldoFinal
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

    }
}
