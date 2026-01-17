using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Models
{
    public class Cuenta
    {
        public int Id { get; set; }       //Identificador autoincrementable en BD
        public int clienteId { get; set; }
        public string numeroCuenta { get; set; }
        [Required]
        public decimal saldo { get; set; }
        public Cliente cliente { get; set; }

    }
}
