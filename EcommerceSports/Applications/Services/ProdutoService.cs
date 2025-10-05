using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Repository.Interfaces;

namespace EcommerceSports.Applications.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _produtoRepository;
        public ProdutoService(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }
        public async Task<List<ListarProdutosDTO>> ListarProdutos()
        {
            var produtos = await _produtoRepository.ListarTodos();
            return produtos.Select(p => new ListarProdutosDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descricao = p.Descricao,
                Preco = p.Preco,
                Categoria = p.Categoria
            }).ToList();
        }
    }
}
