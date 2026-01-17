using PruebaTecnica.Data;
using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    public class ClientService : IClientService
    {
        private readonly PDbContext _context;

        public ClientService(PDbContext context)
        {
            _context = context;
        }

        public Cliente CrearCliente(Cliente cliente)
        {
            // Validar si ya existe un cliente con el mismo DNI
            var existe = _context.Clientes.Any(c => c.DNI == cliente.DNI);
            if (existe)
            {
                throw new InvalidOperationException($"Ya existe un cliente con el DNI {cliente.DNI}");
            }

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return cliente;
        }


        public Cliente ObtenerClientePorDNI(string dni)
        {
            return _context.Clientes.FirstOrDefault(c => c.DNI == dni);
        }

    }

}
