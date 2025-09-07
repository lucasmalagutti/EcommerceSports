using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class ResponseEnderecoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public TipoEndereco TipoEndereco { get; set; }
        public TipoResidencia TipoResidencia { get; set; }
        public TipoLogradouro TipoLogradouro { get; set; }
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string? Observacoes { get; set; }
        public int ClienteId { get; set; }
        public string TipoEnderecoNome => TipoEndereco.ToString();
        public string TipoResidenciaNome => TipoResidencia.ToString();
        public string TipoLogradouroNome => TipoLogradouro.ToString();
    }
}
