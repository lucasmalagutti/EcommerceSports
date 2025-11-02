using System.ComponentModel.DataAnnotations;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class AprovarSolicitacaoTrocaDTO
    {
        [Required(ErrorMessage = "A decisão é obrigatória")]
        public bool Aprovar { get; set; }

        public string? Observacoes { get; set; }
    }
}

