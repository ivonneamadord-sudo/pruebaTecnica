using PruebaTecnica.Models;

namespace PruebaTecnica.Services
{
    public interface IClientService
    {
        Cliente CrearCliente(Cliente cliente);
        Cliente ObtenerClientePorDNI(string dni);

    }

}
