using EcommerceSports.Applications.DTO;
using EcommerceSports.Data.Repository.Interfaces;
using EcommerceSports.Models.Entity;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface ICarrinhoService
    {
        Task<ResponseCarrinhoDTO> ObterCarrinhoAsync(int clienteId);
        Task<ResponseCarrinhoDTO> AdicionarItemAsync(int clienteId, int produtoId, int quantidade);
        Task<ResponseCarrinhoDTO> AtualizarItemAsync(int clienteId, int produtoId, int quantidade);
        Task<ResponseCarrinhoDTO> RemoverItemAsync(int clienteId, int produtoId);
        Task<ResponseCarrinhoDTO> LimparCarrinhoAsync(int clienteId);
        Task<bool> FinalizarCarrinhoAsync(int clienteId);
        Task<ResponseCarrinhoDTO> AdicionarQuantidadeAsync(int clienteId, int produtoId);
        Task<ResponseCarrinhoDTO> DiminuirQuantidadeAsync(int clienteId, int produtoId);
        Task<bool> AtualizarStatusPedidoAsync(int pedidoId, int status);
        Task LimparCarrinhoPorPedidoAsync(int pedidoId);
    }
}
