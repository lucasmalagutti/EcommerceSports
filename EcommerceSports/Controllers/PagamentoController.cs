using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : ControllerBase
    {
        private readonly IPagamentoService _pagamentoService;

        public PagamentoController(IPagamentoService pagamentoService)
        {
            _pagamentoService = pagamentoService;
        }

        [HttpPost("transacao")]
        public async Task<ActionResult<ResponseTransacaoComPagamentosDTO>> CriarTransacaoComPagamentos([FromBody] CriarTransacaoComPagamentosDTO criarDto)
        {
            try
            {
                var transacao = await _pagamentoService.CriarTransacaoComPagamentosAsync(criarDto);
                return Ok(transacao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseTransacaoComPagamentosDTO
                {
                    Mensagem = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseTransacaoComPagamentosDTO
                {
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseTransacaoComPagamentosDTO
                {
                    Mensagem = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        [HttpGet("transacao/{id}")]
        public async Task<ActionResult<ResponseTransacaoComPagamentosDTO>> ObterTransacaoComPagamentos(int id)
        {
            try
            {
                var transacao = await _pagamentoService.ObterTransacaoComPagamentosAsync(id);
                return Ok(transacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseTransacaoComPagamentosDTO
                {
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseTransacaoComPagamentosDTO
                {
                    Mensagem = "Erro interno do servidor: " + ex.Message
                });
            }
        }

        [HttpGet("transacao/{transacaoId}/pagamentos")]
        public async Task<ActionResult<List<PagamentoDTO>>> ObterPagamentosPorTransacao(int transacaoId)
        {
            try
            {
                var pagamentos = await _pagamentoService.ObterPagamentosPorTransacaoAsync(transacaoId);
                return Ok(pagamentos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }
    }
}
