using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IClienteService
    {
        Task CadastrarCliente(ClienteDTO clientedto);

        Task AtualizarCliente(int id, EditarClienteDTO cliente);

        Task AtualizarSenha(int id, EditarSenhaDTO senha);
        Task AtualizarStatusCliente(int id, EditarStatusClienteDTO status);

        Task<List<ListarClienteDTO>> ListarDadosCliente();
    }
}
