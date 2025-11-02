using System.ComponentModel.DataAnnotations;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class PagamentoDTO
    {
        public int Id { get; set; }
        public int TransacaoId { get; set; }
        public int CartaoId { get; set; }
        public decimal Valor { get; set; }
        public StatusPagamento StatusPagamento { get; set; }
        public DateTime DataPagamento { get; set; }
    }

    public class CriarPagamentoDTO
    {
        [Required(ErrorMessage = "O ID do cartão é obrigatório")]
        public int CartaoId { get; set; }

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        public decimal Valor { get; set; }
    }

    public class CriarTransacaoComPagamentosDTO
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

        [Required(ErrorMessage = "Pelo menos um pagamento é obrigatório")]
        [MinLength(1, ErrorMessage = "Pelo menos um pagamento deve ser informado")]
        public List<CriarPagamentoDTO> Pagamentos { get; set; } = new List<CriarPagamentoDTO>();

        public StatusTransacao StatusTransacao { get; set; } = StatusTransacao.Pendente;

        public List<string> Cupons { get; set; } = new List<string>();
    }

    public class ResponseTransacaoComPagamentosDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public decimal ValorTotal { get; set; }
        public float ValorFrete { get; set; }
        public int EnderecoId { get; set; }
        public StatusPedido StatusTransacao { get; set; }
        public DateTime DataTransacao { get; set; }
        public List<PagamentoDTO> Pagamentos { get; set; } = new List<PagamentoDTO>();
        public string Mensagem { get; set; } = string.Empty;
    }
}
