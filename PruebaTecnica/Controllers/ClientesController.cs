using Microsoft.AspNetCore.Mvc;
using PruebaTecnica.DTOs;
using PruebaTecnica.Models;
using PruebaTecnica.Services;

namespace PruebaTecnica.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientesController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // POST: api/Crear_Clientes
        [HttpPost("Crear_Clientes")]
        public IActionResult CrearCliente([FromBody] ClienteCreateDto dto)
        {
            try
            {

                // Mapear DTO a entidad
                var cliente = new Cliente
                {
                    DNI = dto.DNI.Trim(), //incluimos el DNI
                    nombre = dto.nombre,
                    fechaNacimiento = dto.fechaNacimiento,
                    sexo = dto.sexo,
                    ingresos = dto.ingresos
                };

                var nuevoCliente = _clientService.CrearCliente(cliente);

                // Mapear entidad a DTO de respuesta
                var response = new ClienteResponseDto
                {
                    Id = nuevoCliente.Id,
                    DNI = nuevoCliente.DNI, 
                    nombre = nuevoCliente.nombre,
                    fechaNacimiento = nuevoCliente.fechaNacimiento,
                    sexo = nuevoCliente.sexo,
                    ingresos = nuevoCliente.ingresos
                };

                return Ok(response);
            }

            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }


        [HttpGet("Consulta_Clientes_Por_DNI")]
        public IActionResult ObtenerClientePorDNI(string dni)
        {
            var dniLimpio = dni.Trim();
            var cliente = _clientService.ObtenerClientePorDNI(dniLimpio);
            if (cliente == null) return NotFound($"No se encontró un cliente con el DNI {dni}");

            // Mapear entidad a DTO de respuesta
            var response = new ClienteResponseDto
            {
                Id = cliente.Id,
                DNI = cliente.DNI,
                nombre = cliente.nombre,
                fechaNacimiento = cliente.fechaNacimiento,
                sexo = cliente.sexo,
                ingresos = cliente.ingresos
            };

            return Ok(response);
        }
    }
}
