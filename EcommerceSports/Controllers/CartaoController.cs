using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartaoController : Controller
    {
        private readonly ICartaoService _cartaoService;

        public CartaoController(ICartaoService cartaoService)
        {
            _cartaoService = cartaoService;
        }

        [HttpPost]
        [Route("/Cartao/Cadastrar/{clienteId}")]
        public async Task<IActionResult> CadastrarCartao(int clienteId, [FromBody] CadastrarCartaoDTO cartao)
        {
            try
            {
                var cartaoCadastrado = await _cartaoService.CadastrarCartao(clienteId, cartao);
                return Ok(new { 
                    mensagem = "Cartão cadastrado com sucesso!", 
                    cartao = cartaoCadastrado 
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor. Tente novamente mais tarde." });
            }
        }

        [HttpGet]
        [Route("/Cartao/Listar/{clienteId}")]
        public async Task<IActionResult> ListarCartoesPorCliente(int clienteId)
        {
            try
            {
                var cartoes = await _cartaoService.ListarCartoesPorCliente(clienteId);
                return Ok(cartoes);
            }
            catch (Exception)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor. Tente novamente mais tarde." });
            }
        }


    }
}
