using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Infra.Interfaces
{
    public interface IClienteRepository
    {
        public void CadastrarCliente(Cliente cliente, List<Endereco> enderecos, Telefone telefone, CartaoCredito cartao);
    }
}