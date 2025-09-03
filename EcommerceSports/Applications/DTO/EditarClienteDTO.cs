using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class EditarClienteDTO
    {
        public string? Cpf { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string? Email { get; set; }
        public Genero? Genero { get; set; }
        public string? Nome { get; set; }
        public TipoTelefone TipoTelefone { get; set; }
        public string Ddd { get; set; }
        public string NumeroTelefone { get; set; }
    }
}
