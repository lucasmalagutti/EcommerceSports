using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class ResponseCartaoDTO
    {
        public int Id { get; set; }
        public string NumCartao { get; set; } = string.Empty;
        public string NomeImpresso { get; set; } = string.Empty;
        public BandeiraCartao Bandeira { get; set; }
        public bool Preferencial { get; set; }
        public int ClienteId { get; set; }
        public string BandeiraNome => Bandeira.ToString();
    }
}
