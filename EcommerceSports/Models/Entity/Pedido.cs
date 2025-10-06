
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class Pedido
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public DateTime DataPedido { get; set; } = DateTime.UtcNow;
        public decimal ValorTotal { get; set; }
        public StatusPedido StatusPedido { get; set; }

        public Cliente? Cliente { get; set; }
        public List<ItemPedido> Itens { get; set; } = new List<ItemPedido>();
        public Transacao? Transacao { get; set; }
    }
}


