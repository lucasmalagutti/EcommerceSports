using Microsoft.AspNetCore.Mvc;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Models.Enums;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitacaoTrocaController : ControllerBase
    {
        private readonly ISolicitacaoTrocaService _solicitacaoTrocaService;

        public SolicitacaoTrocaController(ISolicitacaoTrocaService solicitacaoTrocaService)
        {
            _solicitacaoTrocaService = solicitacaoTrocaService;
        }

        // Criar solicitação de troca/devolução (usuário)
        [HttpPost("criar/{clienteId}")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> CriarSolicitacao(int clienteId, [FromBody] CriarSolicitacaoTrocaDTO criarDto)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.CriarSolicitacaoAsync(criarDto, clienteId);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Obter solicitação por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> ObterSolicitacao(int id)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.ObterSolicitacaoPorIdAsync(id);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Obter solicitações por cliente
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<ResponseSolicitacaoTrocaDTO>>> ObterSolicitacoesPorCliente(int clienteId)
        {
            try
            {
                var solicitacoes = await _solicitacaoTrocaService.ObterSolicitacoesPorClienteAsync(clienteId);
                return Ok(solicitacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Obter todas as solicitações (admin)
        [HttpGet("todas")]
        public async Task<ActionResult<IEnumerable<ResponseSolicitacaoTrocaDTO>>> ObterTodasSolicitacoes()
        {
            try
            {
                var solicitacoes = await _solicitacaoTrocaService.ObterTodasSolicitacoesAsync();
                return Ok(solicitacoes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Aprovar ou negar solicitação (admin)
        [HttpPut("{solicitacaoId}/aprovar")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> AprovarSolicitacao(int solicitacaoId, [FromBody] AprovarSolicitacaoTrocaDTO aprovarDto)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.AprovarSolicitacaoAsync(solicitacaoId, aprovarDto);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Definir como Em Transporte (admin)
        [HttpPut("{solicitacaoId}/em-transporte")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> DefinirEmTransporte(int solicitacaoId)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.DefinirEmTransporteAsync(solicitacaoId);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Confirmar recebimento do produto (admin)
        [HttpPut("{solicitacaoId}/confirmar-recebimento")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> ConfirmarRecebimento(int solicitacaoId)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.ConfirmarRecebimentoAsync(solicitacaoId);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Gerar cupom de troca (admin)
        [HttpPut("{solicitacaoId}/gerar-cupom")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> GerarCupomTroca(int solicitacaoId)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.GerarCupomTrocaAsync(solicitacaoId);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }

        // Finalizar troca - Confirmar que produto foi entregue (admin)
        [HttpPut("{solicitacaoId}/finalizar")]
        public async Task<ActionResult<ResponseSolicitacaoTrocaDTO>> FinalizarTroca(int solicitacaoId)
        {
            try
            {
                var solicitacao = await _solicitacaoTrocaService.FinalizarTrocaAsync(solicitacaoId);
                return Ok(solicitacao);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseSolicitacaoTrocaDTO { Mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseSolicitacaoTrocaDTO { Mensagem = "Erro interno do servidor: " + ex.Message });
            }
        }
    }
}

