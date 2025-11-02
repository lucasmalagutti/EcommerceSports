using System.ComponentModel.DataAnnotations;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class CriarSolicitacaoTrocaDTO
    {
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        public int PedidoId { get; set; }

        public int? ItemPedidoId { get; set; } // Null para devolução completa do pedido

        [Required(ErrorMessage = "O tipo de solicitação é obrigatório")]
        public TipoSolicitacao TipoSolicitacao { get; set; }

        [Required(ErrorMessage = "O motivo da solicitação é obrigatório")]
        [StringLength(500, ErrorMessage = "O motivo deve ter no máximo 500 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }
}

