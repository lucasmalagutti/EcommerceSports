using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EcommerceSports.Applications.DTO
{
    public class ChatbotFiltrosDTO
    {
        [JsonPropertyName("categoria")]
        public string? Categoria { get; set; }

        [JsonPropertyName("termosDeBusca")]
        public List<string>? TermosDeBusca { get; set; }
    }
}

