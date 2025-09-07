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
        [Route("/Cadastrar/Cartao")]
        public async Task<IActionResult> CadastrarCartao([FromBody] CadastrarCartaoDTO cartao)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cartaoCadastrado = await _cartaoService.CadastrarCartao(cartao);
                return Ok(new { 
                    mensagem = "Cartão cadastrado com sucesso!", 
                    cartao = cartaoCadastrado 
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor. Tente novamente mais tarde." });
            }
        }

        [HttpGet]
        [Route("cliente/{clienteId}")]
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

        [HttpGet]
        [Route("{cartaoId}")]
        public async Task<IActionResult> ObterCartaoPorId(int cartaoId)
        {
            try
            {
                var cartao = await _cartaoService.ObterCartaoPorId(cartaoId);
                
                if (cartao == null)
                {
                    return NotFound(new { erro = "Cartão não encontrado." });
                }

                return Ok(cartao);
            }
            catch (Exception)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor. Tente novamente mais tarde." });
            }
        }

    }
}
