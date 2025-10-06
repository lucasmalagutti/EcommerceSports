using System;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Transacao
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public decimal ValorTotal { get; set; }
        public float ValorFrete { get; set; }
        public int EnderecoId { get; set; }
        public int CartaoId { get; set; }
        public StatusTransacao StatusTransacao { get; set; }
        public DateTime DataTransacao { get; set; } = DateTime.UtcNow;

        public Pedido? Pedido { get; set; }
        public Endereco? Endereco { get; set; }
        public CartaoCredito? Cartao { get; set; }
    }
}


