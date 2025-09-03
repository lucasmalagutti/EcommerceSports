using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Infra.Interfaces
{
    public interface IClienteRepository
    {
        public void CadastrarCliente(Cliente cliente);

        Task<Cliente?> BuscarPorId(int id);
        Task AtualizarCliente(Cliente cliente);
    }
}