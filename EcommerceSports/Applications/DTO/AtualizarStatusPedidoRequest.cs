using System.Text.Json.Serialization;

namespace EcommerceSports.Applications.DTO
{
    public class AtualizarStatusPedidoRequest
    {
        [JsonPropertyName("statusPedido")]
        public string StatusPedido { get; set; } = string.Empty;
    }
}

