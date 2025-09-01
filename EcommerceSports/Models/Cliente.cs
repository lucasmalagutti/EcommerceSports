namespace EcommerceSports.Models
{
    public class Cliente
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public int? NumRanking { get; set; }
        public bool CadastroAtivo { get; set; }
        public List<Telefone> Telefones { get; set; } = new List<Telefone>();
    }
}
