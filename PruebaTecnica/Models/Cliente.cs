using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PruebaTecnica.Models
{
    public class Cliente
    {
        [JsonIgnore]
        public int Id { get; set; }         //Id autoincrementable de DB
        
        [Required]
        public string DNI { get; set; }
        [Required]

        public string nombre { get; set; }
        [Required]

        public DateOnly fechaNacimiento { get; set; }
        [Required]

        public string sexo { get; set; }
        public decimal ingresos { get; set; }

        public ICollection<Cuenta> Cuentas { get; set; } = new List<Cuenta>();

    }
}
