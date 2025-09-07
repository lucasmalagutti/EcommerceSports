using System.ComponentModel.DataAnnotations;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class CadastrarCartaoDTO
    {
        [Required(ErrorMessage = "O número do cartão é obrigatório")]
        [StringLength(19, MinimumLength = 13, ErrorMessage = "O número do cartão deve ter entre 13 e 19 caracteres")]
        public string NumCartao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome impresso no cartão é obrigatório")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O nome impresso deve ter entre 2 e 50 caracteres")]
        public string NomeImpresso { get; set; } = string.Empty;

        [Required(ErrorMessage = "A bandeira do cartão é obrigatória")]
        [Range(1, 6, ErrorMessage = "Bandeira inválida. Valores permitidos: 1-Visa, 2-Mastercard, 3-American Express, 4-Elo, 5-HiperCard, 6-Aura")]
        public BandeiraCartao Bandeira { get; set; }

        [Required(ErrorMessage = "O código de segurança (CVC) é obrigatório")]
        [Range(100, 9999, ErrorMessage = "O CVC deve ter entre 3 e 4 dígitos")]
        public int Cvc { get; set; }

        [Required(ErrorMessage = "O ID do cliente é obrigatório")]
        public int ClienteId { get; set; }

        public bool Preferencial { get; set; } = false;
    }
}
