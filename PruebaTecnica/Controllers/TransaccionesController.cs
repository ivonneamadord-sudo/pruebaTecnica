using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.Services;

namespace PruebaTecnica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransaccionesController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransaccionesController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("depositar")]
        public IActionResult Depositar(string numeroCuenta, decimal monto)
        {
            var transaccion = _transactionService.Depositar(numeroCuenta, monto);
            return Ok(transaccion);
        }

        [HttpPost("retirar")]
        public IActionResult Retirar(string numeroCuenta, decimal monto)
        {
            try
            {
                var transaccion = _transactionService.Retirar(numeroCuenta, monto);
                return Ok(transaccion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("historial/{numeroCuenta}")]
        public IActionResult Historial(string numeroCuenta)
        {
            var historial = _transactionService.ObtenerHistorial(numeroCuenta);
            return Ok(historial);
        }
    }

}
