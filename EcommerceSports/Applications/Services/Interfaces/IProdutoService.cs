using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Applications.Services.Interfaces
{
    public interface IProdutoService
    {


        Task<List<ListarProdutosDTO>> ListarProdutos();

    }
}
