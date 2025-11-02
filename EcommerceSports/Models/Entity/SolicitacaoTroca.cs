using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class SolicitacaoTroca
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int? ItemPedidoId { get; set; } // Null se for devolução completa
        public int ClienteId { get; set; }
        public TipoSolicitacao TipoSolicitacao { get; set; }
        public StatusSolicitacaoTroca Status { get; set; } = StatusSolicitacaoTroca.Pendente;
        public string Motivo { get; set; } = string.Empty;
        public string? ObservacoesAdm { get; set; }
        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAprovacao { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public int? CupomId { get; set; } // Cupom gerado para troca
        public decimal? ValorCupom { get; set; } // Valor do cupom gerado

        // Navegação
        public Pedido? Pedido { get; set; }
        public ItemPedido? ItemPedido { get; set; }
        public Cliente? Cliente { get; set; }
        public Cupom? Cupom { get; set; }
    }
}

