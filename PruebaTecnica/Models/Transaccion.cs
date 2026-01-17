using System.ComponentModel.DataAnnotations;

namespace PruebaTecnica.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public DateTime fecha { get; set; }
        public int cuentaId { get; set; }
        public string tipoTransaccion { get; set; } //Deposito, Retiro
        [Required]
        public decimal monto { get; set; }
        public decimal saldoFinal { get; set; }


    }
}
