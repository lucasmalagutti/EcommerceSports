using EcommerceSports.Models.Enums;

namespace EcommerceSports.Models.Entity
{
    public class CartaoCredito
    {
        public int Id { get; set; }
        public string NumCartao { get; set; }
        public string NomeImpresso { get; set; }
        public int Cvc { get; set; }
        public BandeiraCartao Bandeira { get; set; }
        public bool Preferencial { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
