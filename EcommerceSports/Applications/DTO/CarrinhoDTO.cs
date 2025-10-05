using System.ComponentModel.DataAnnotations;

namespace EcommerceSports.Applications.DTO
{
    public class CarrinhoDTO
    {
        [Required]
        public int ClienteId { get; set; }
        
        [Required]
        public List<ItemCarrinhoDTO> Itens { get; set; } = new List<ItemCarrinhoDTO>();
    }

    public class ItemCarrinhoDTO
    {
        [Required]
        public int ProdutoId { get; set; }
        
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
        public decimal PrecoUnitario { get; set; }
    }

    public class AtualizarCarrinhoDTO
    {
        [Required]
        public int ClienteId { get; set; }
        
        [Required]
        public int ProdutoId { get; set; }
        
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade deve ser maior ou igual a zero")]
        public int Quantidade { get; set; }
    }

    public class AlterarQuantidadeDTO
    {
        [Required]
        public int ProdutoId { get; set; }
    }

    public class ResponseCarrinhoDTO
    {
        public int ClienteId { get; set; }
        public List<ResponseItemCarrinhoDTO> Itens { get; set; } = new List<ResponseItemCarrinhoDTO>();
        public decimal ValorTotal { get; set; }
        public int TotalItens { get; set; }
    }

    public class ResponseItemCarrinhoDTO
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string ImagemProduto { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
