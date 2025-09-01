namespace EcommerceSports.Models.Entity
{
    public class Cliente : Pessoa
    {
        public string Email { get; set; }
        public string Senha { get; set; }
        public bool CadastroAtivo { get; set; }
        public List<Endereco> Endereco {  get; set; }
        public List<Telefone> Telefones { get; set; } = new List<Telefone>();
        public List<CartaoCredito> Cartoes { get; set; } = new List<CartaoCredito>();

    }
}
