namespace EcommerceSports.Applications.DTO
{
    public class CartaoDTO
    {
        public int Id { get; set; }
        public int Bandeira { get; set; }
        public int ClienteId { get; set; }
        public int Cvc { get; set; }
        public string NomeImpresso { get; set; }
        public string NumCartao { get; set; }
        public Boolean Preferencial { get; set; }
    }
}
