using CrudCliente.Domain.Enumerators;

namespace EcommerceSports.Models
{
    public class CartaoCredito
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string NumCartao { get; set; }
        public string NomeImpresso { get; set; }
        public int Cvc { get; set; }
        public BandeiraCartao Bandeira { get; set; }
        public Boolean Preferencial { get; set; }
    }
}
