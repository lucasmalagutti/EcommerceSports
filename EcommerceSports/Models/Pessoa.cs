using CrudCliente.Domain.Enumerators;

namespace EcommerceSports.Models
{
    public class Pessoa
    {
        public int Id { get; set; }
        public DateTime DtCadastro { get; set; }
        public string Cpf { get; set; }
        public string Nome { get; set; }

        public DateTime DtNasc { get; set; }
        public Genero Genero { get; set; }

    }
}
