using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Endereco
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public TipoEndereco TipoEndereco { get; set; }
        public TipoResidencia TipoResidencia { get; set; }
        public TipoLogradouro TipoLogradouro { get; set; }
        public required string Nome { get; set; }
        public required string Logradouro { get; set; }
        public required string Numero { get; set; }
        public required string Cep { get; set; }
        public required string Bairro { get; set; }
        public required string Cidade { get; set; }
        public required string Estado { get; set; }
        public required string Pais { get; set; }
        public string? Observacoes { get; set; }
        public Cliente? Cliente { get; set; }
    }
}
