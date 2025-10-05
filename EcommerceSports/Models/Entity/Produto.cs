namespace EcommerceSports.Models.Entity
{
    public class Produto
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Categoria { get; set; }
        public float Preco { get; set; }
        public int QtdEstoque { get; set; }
        public required string Descricao { get; set; }
    }
}


