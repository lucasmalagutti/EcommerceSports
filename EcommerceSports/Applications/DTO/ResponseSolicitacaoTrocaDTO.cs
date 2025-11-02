using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class ResponseSolicitacaoTrocaDTO
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int? ItemPedidoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public string ClienteNome { get; set; } = string.Empty;
        public TipoSolicitacao TipoSolicitacao { get; set; }
        public string TipoSolicitacaoNome => TipoSolicitacao == TipoSolicitacao.Troca ? "Troca" : "Devolução";
        public StatusSolicitacaoTroca Status { get; set; }
        public string StatusNome => Status switch
        {
            StatusSolicitacaoTroca.Pendente => "Pendente",
            StatusSolicitacaoTroca.Aprovada => "Aprovada",
            StatusSolicitacaoTroca.Negada => "Negada",
            StatusSolicitacaoTroca.EmTransporte => "Em Transporte",
            StatusSolicitacaoTroca.ProdutoRecebido => "Produto Recebido",
            StatusSolicitacaoTroca.CupomGerado => "Cupom Gerado",
            StatusSolicitacaoTroca.Finalizada => "Finalizada",
            _ => "Desconhecido"
        };
        public string Motivo { get; set; } = string.Empty;
        public string? ObservacoesAdm { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public DateTime? DataAprovacao { get; set; }
        public DateTime? DataRecebimento { get; set; }
        public int? CupomId { get; set; }
        public string? CupomNome { get; set; }
        public decimal? ValorCupom { get; set; }
        public string Mensagem { get; set; } = string.Empty;
    }
}

