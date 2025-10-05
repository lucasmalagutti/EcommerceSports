using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    public class ProdutoController : Controller
    {
        private readonly IProdutoService _produtoService;

        public ProdutoController(IProdutoService produtoService)
        {
            _produtoService = produtoService;
        }

        [HttpGet]
        [Route("/Produto/Listar")]
        public async Task<IActionResult> ListarProdutos()
        {
            var produtos = await _produtoService.ListarProdutos();
            return Ok(produtos);
        }
    }
}
