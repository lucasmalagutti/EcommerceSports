namespace EcommerceSports.Applications.DTO
{
    public class GraficoDTO
    {
        public class GraficoVendasDTO
        {
            public DateTime Data { get; set; }          
            public decimal ValorTotal { get; set; }    
            public int QuantidadeVendas { get; set; }   
        }
        public class GraficoVendasCategoriaDTO
        {
            public string CategoriaNome { get; set; } = string.Empty;
            public DateTime Data { get; set; }
            public decimal ValorTotal { get; set; }
        }
    }
}
