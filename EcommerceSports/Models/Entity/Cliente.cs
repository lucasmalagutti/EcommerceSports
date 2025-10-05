namespace EcommerceSports.Models.Entity
{
    public class Cliente : Pessoa
    {
        public required string Email { get; set; }
        public required string Senha { get; set; }
        public bool CadastroAtivo { get; set; }
        public List<Endereco> Endereco { get; set; } = new List<Endereco>();
        public List<Telefone> Telefones { get; set; } = new List<Telefone>();
        public List<CartaoCredito> Cartoes { get; set; } = new List<CartaoCredito>();
        public List<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
