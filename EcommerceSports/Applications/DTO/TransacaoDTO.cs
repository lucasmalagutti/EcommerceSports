using System.ComponentModel.DataAnnotations;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class CriarTransacaoDTO
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        public int PedidoId { get; set; }

        [Required(ErrorMessage = "O valor total é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor total deve ser maior que zero")]
        public decimal ValorTotal { get; set; }

        [Required(ErrorMessage = "O valor do frete é obrigatório")]
        [Range(0, double.MaxValue, ErrorMessage = "O valor do frete deve ser maior ou igual a zero")]
        public float ValorFrete { get; set; }

        [Required(ErrorMessage = "O ID do endereço é obrigatório")]
        public int EnderecoId { get; set; }

        [Required(ErrorMessage = "O ID do cartão é obrigatório")]
        public int CartaoId { get; set; }

        public StatusTransacao StatusTransacao { get; set; } = StatusTransacao.EmAberto;
    }

    public class ResponseTransacaoDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public decimal ValorTotal { get; set; }
        public float ValorFrete { get; set; }
        public int EnderecoId { get; set; }
        public int CartaoId { get; set; }
        public StatusTransacao StatusTransacao { get; set; }
        public DateTime DataTransacao { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}
