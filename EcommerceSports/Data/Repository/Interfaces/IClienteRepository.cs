using EcommerceSports.Models.Entity;

namespace EcommerceSports.Data.Infra.Interfaces
{
    public interface IClienteRepository
    {
        Task CadastrarCliente(Cliente cliente);

        Task<Cliente?> BuscarPorId(int id);
        Task<Cliente?> BuscarPorCpf(string cpf);
        Task AtualizarCliente(Cliente cliente);

        Task<List<Cliente>> ListarTodos();
    }
}