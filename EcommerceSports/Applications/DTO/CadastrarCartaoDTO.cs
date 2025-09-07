using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class CadastrarCartaoDTO
    {
        public string NumCartao { get; set; } = string.Empty;
        public string NomeImpresso { get; set; } = string.Empty;
        public BandeiraCartao Bandeira { get; set; }
        public int Cvc { get; set; }
        public bool Preferencial { get; set; } = false;
    }
}
