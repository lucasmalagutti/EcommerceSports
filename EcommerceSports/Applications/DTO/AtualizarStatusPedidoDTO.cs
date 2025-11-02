using System.Text.Json.Serialization;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.DTO
{
    public class AtualizarStatusPedidoDTO
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StatusPedido? StatusPedido { get; set; }
        
        // Propriedade para aceitar string e converter automaticamente
        [JsonPropertyName("statusPedido")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? StatusPedidoString 
        { 
            get => StatusPedido?.ToString();
            set 
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Enum.TryParse<StatusPedido>(value, true, out var status))
                    {
                        StatusPedido = status;
                    }
                }
            } 
        }
    }
}

