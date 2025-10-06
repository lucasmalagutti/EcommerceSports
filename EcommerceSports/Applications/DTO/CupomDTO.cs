using System.ComponentModel.DataAnnotations;

namespace EcommerceSports.Applications.DTO
{
    public class CupomDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float Desconto { get; set; }
    }

    public class ValidarCupomDTO
    {
        [Required(ErrorMessage = "O nome do cupom é obrigatório")]
        public string Nome { get; set; } = string.Empty;
    }

    public class ResponseCupomDTO
    {
        public bool Valido { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float Desconto { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
