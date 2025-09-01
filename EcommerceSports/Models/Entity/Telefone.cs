using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Telefone
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public TipoTelefone TipoTelefone { get; set; }
        public string Ddd { get; set; }
        public string Numero { get; set; }

        public Cliente Cliente { get; set; }
    }
}
