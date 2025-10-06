using Microsoft.AspNetCore.Mvc;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(ITransacaoService transacaoService)
        {
            _transacaoService = transacaoService;
        }

        [HttpPost]
        public async Task<ActionResult<ResponseTransacaoDTO>> CriarTransacao([FromBody] CriarTransacaoDTO criarDto)
        {
            try
            {
                var transacao = await _transacaoService.CriarTransacaoAsync(criarDto);
                return Ok(transacao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseTransacaoDTO
                {
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseTransacaoDTO
                {
                    Mensagem = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        [HttpGet("/Transacao/Listar/{id}")]
        public async Task<ActionResult<ResponseTransacaoDTO>> ObterTransacaoPorId(int id)
        {
            try
            {
                var transacao = await _transacaoService.ObterTransacaoPorIdAsync(id);
                return Ok(transacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseTransacaoDTO
                {
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseTransacaoDTO
                {
                    Mensagem = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        [HttpGet("pedido/{pedidoId}")]
        public async Task<ActionResult<ResponseTransacaoDTO>> ObterTransacaoPorPedidoId(int pedidoId)
        {
            try
            {
                var transacao = await _transacaoService.ObterTransacaoPorPedidoIdAsync(pedidoId);
                return Ok(transacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseTransacaoDTO
                {
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseTransacaoDTO
                {
                    Mensagem = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }
}
