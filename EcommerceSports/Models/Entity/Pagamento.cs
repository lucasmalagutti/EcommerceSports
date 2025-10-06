using System;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Pagamento
    {
        public int Id { get; set; }
        public int TransacaoId { get; set; }
        public int CartaoId { get; set; }
        public decimal Valor { get; set; }
        public StatusPagamento StatusPagamento { get; set; }
        public DateTime DataPagamento { get; set; } = DateTime.UtcNow;

        public Transacao? Transacao { get; set; }
        public CartaoCredito? Cartao { get; set; }
    }
}
