using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Pessoa
    {
        public int Id { get; set; }
        public DateTime DtCadastro { get; set; }
        public required string Cpf { get; set; }
        public required string Nome { get; set; }
        public DateTime DtNasc { get; set; }
        public Genero Genero { get; set; }
    }
}
