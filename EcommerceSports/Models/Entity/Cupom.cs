namespace EcommerceSports.Models.Entity
{
    public class Cupom
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public decimal Desconto { get; set; }
        public bool Utilizado { get; set; } = false;
        public DateTime? DataUtilizacao { get; set; }
    }
}


