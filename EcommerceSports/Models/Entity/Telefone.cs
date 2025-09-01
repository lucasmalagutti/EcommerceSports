using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Telefone
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public TipoTelefone TipoTelefone { get; set; }
        public required string Ddd { get; set; }
        public required string Numero { get; set; }
        public Cliente? Cliente { get; set; }
    }
}
