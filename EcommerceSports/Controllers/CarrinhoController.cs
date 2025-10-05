using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CarrinhoController : ControllerBase
    {
        private readonly ICarrinhoService _carrinhoService;

        public CarrinhoController(ICarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
        }

        /// <summary>
        /// Obter carrinho do cliente
        /// </summary>
        [HttpGet("{clienteId}")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> ObterCarrinho(int clienteId)
        {
            try
            {
                var carrinho = await _carrinhoService.ObterCarrinhoAsync(clienteId);
                return Ok(carrinho);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Adicionar item ao carrinho
        /// </summary>
        [HttpPost("{clienteId}/adicionar")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> AdicionarItem(int clienteId, [FromBody] ItemCarrinhoDTO itemDto)
        {
            try
            {
                var carrinho = await _carrinhoService.AdicionarItemAsync(clienteId, itemDto.ProdutoId, itemDto.Quantidade);
                return Ok(carrinho);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualizar quantidade de um item no carrinho
        /// </summary>
        [HttpPut("{clienteId}/atualizar")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> AtualizarItem(int clienteId, [FromBody] AtualizarCarrinhoDTO atualizarDto)
        {
            try
            {
                var carrinho = await _carrinhoService.AtualizarItemAsync(clienteId, atualizarDto.ProdutoId, atualizarDto.Quantidade);
                return Ok(carrinho);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remover item do carrinho
        /// </summary>
        [HttpDelete("{clienteId}/remover/{produtoId}")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> RemoverItem(int clienteId, int produtoId)
        {
            try
            {
                var carrinho = await _carrinhoService.RemoverItemAsync(clienteId, produtoId);
                return Ok(carrinho);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Limpar carrinho
        /// </summary>
        [HttpDelete("{clienteId}/limpar")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> LimparCarrinho(int clienteId)
        {
            try
            {
                var carrinho = await _carrinhoService.LimparCarrinhoAsync(clienteId);
                return Ok(carrinho);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Finalizar carrinho (converter em pedido)
        /// </summary>
        [HttpPost("{clienteId}/finalizar")]
        public async Task<ActionResult> FinalizarCarrinho(int clienteId)
        {
            try
            {
                var sucesso = await _carrinhoService.FinalizarCarrinhoAsync(clienteId);
                if (sucesso)
                {
                    return Ok(new { message = "Carrinho finalizado com sucesso" });
                }
                return BadRequest(new { message = "Não foi possível finalizar o carrinho" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Adicionar quantidade de um item no carrinho
        /// </summary>
        [HttpPost("{clienteId}/adicionar-qtd")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> AdicionarQuantidade(int clienteId, [FromBody] AlterarQuantidadeDTO alterarDto)
        {
            try
            {
                var carrinho = await _carrinhoService.AdicionarQuantidadeAsync(clienteId, alterarDto.ProdutoId);
                return Ok(carrinho);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Diminuir quantidade de um item no carrinho
        /// </summary>
        [HttpPost("{clienteId}/diminuir-qtd")]
        public async Task<ActionResult<ResponseCarrinhoDTO>> DiminuirQuantidade(int clienteId, [FromBody] AlterarQuantidadeDTO alterarDto)
        {
            try
            {
                var carrinho = await _carrinhoService.DiminuirQuantidadeAsync(clienteId, alterarDto.ProdutoId);
                return Ok(carrinho);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
