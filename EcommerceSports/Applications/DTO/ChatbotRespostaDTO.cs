using System.Collections.Generic;

namespace EcommerceSports.Applications.DTO
{
    public class ChatbotRespostaDTO
    {
        public string Tipo { get; set; } = "texto"; // "lista", "texto", "pergunta", "produto", "erro"
        public string Mensagem { get; set; } = string.Empty; // texto a exibir para o usuário
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
    }
}

