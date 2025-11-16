using System.Collections.Generic;

namespace EcommerceSports.Applications.DTO
{
    public class ChatbotRespostaDTO
    {
        public string Tipo { get; set; } = "texto"; // "lista", "texto", "pergunta", "produto", "erro"
        public string Layout { get; set; } = "texto"; // "texto", "cards", "lista"
        public string Mensagem { get; set; } = string.Empty; // texto a exibir para o usuário
        public ChatbotAcaoDTO? AcaoPadrao { get; set; } // ação aplicada a todos os produtos (ex: adicionar ao carrinho)
        public List<ProdutoDTO>? Produtos { get; set; } // quando Tipo == "lista" ou "produto"
    }

    public class ProdutoDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal? Preco { get; set; }
        public string? Categoria { get; set; }
        public string? ImagemUrl { get; set; }
        public string? LinkProduto { get; set; }
        public string? DescricaoCurta { get; set; }
        public ChatbotAcaoDTO? Acao { get; set; }
    }

    public class ChatbotAcaoDTO
    {
        public string Tipo { get; set; } = string.Empty; // ex: comprar
        public string Label { get; set; } = string.Empty; // texto do botão exibido
        public string Metodo { get; set; } = "POST";
        public string Endpoint { get; set; } = string.Empty;
        public object? Payload { get; set; }
        public Dictionary<string, string>? Headers { get; set; }
    }
}

