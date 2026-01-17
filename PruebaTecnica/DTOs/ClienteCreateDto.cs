namespace PruebaTecnica.DTOs
{
    public class ClienteCreateDto
    {
        public string DNI { get; set; }
        public string nombre { get; set; }
        public DateOnly fechaNacimiento { get; set; }
        public string sexo { get; set; }
        public decimal ingresos { get; set; }
    }

}
