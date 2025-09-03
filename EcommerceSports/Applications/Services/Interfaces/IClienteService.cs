using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IClienteService
    {
        public void CadastrarCliente(ClienteDTO clientedto);

        Task AtualizarCliente(int id, EditarClienteDTO cliente);

        Task AtualizarSenha(int id, EditarSenhaDTO senha);
    }
}
