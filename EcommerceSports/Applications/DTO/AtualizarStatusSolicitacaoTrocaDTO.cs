using System.ComponentModel.DataAnnotations;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class AtualizarStatusSolicitacaoTrocaDTO
    {
        [Required(ErrorMessage = "O novo status é obrigatório")]
        public StatusSolicitacaoTroca NovoStatus { get; set; }

        public string? Observacoes { get; set; }
    }
}

