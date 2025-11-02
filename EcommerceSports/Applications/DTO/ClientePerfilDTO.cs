using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class ClientePerfilDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Genero Genero { get; set; }
        public string Ddd { get; set; } = null!;
        public string Telefone { get; set; } = null!;
        public TipoTelefone TipoTelefone { get; set; }
        public List<EnderecoDTO> Enderecos { get; set; } = new();
        public List<CartaoDTO> Cartoes { get; set; } = new();
    }
}
