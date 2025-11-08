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

        [HttpGet("/Transacao/ListarPorId/{id}")]
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
        [HttpGet("/Transacao/ListarPorCliente/{clienteId}")]
        public async Task<ActionResult<IEnumerable<ResponseTransacaoDTO>>> ListarTransacoesPorCliente(int clienteId)
        {
            try
            {
                var transacoes = await _transacaoService.ObterTransacoesPorCliente(clienteId);

                if (transacoes == null || !transacoes.Any())
                    return NotFound(new { Mensagem = "Nenhuma transa��o encontrada para este cliente." });

                return Ok(transacoes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Mensagem = "Erro interno do servidor: " + ex.Message });
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

        [HttpPut("/Transacao/AtualizarStatus/{pedidoId}")]
        public async Task<ActionResult<ResponseTransacaoDTO>> AtualizarStatusPedido(int pedidoId, [FromBody] AtualizarStatusPedidoRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.StatusPedido))
                {
                    return BadRequest(new ResponseTransacaoDTO
                    {
                        Mensagem = "O campo 'statusPedido' é obrigatório."
                    });
                }
                
                // Converter string para enum (case-insensitive)
                if (!Enum.TryParse<Models.Enums.StatusPedido>(request.StatusPedido, true, out var novoStatus))
                {
                    return BadRequest(new ResponseTransacaoDTO
                    {
                        Mensagem = $"Status inválido: '{request.StatusPedido}'. Valores aceitos: EmProcessamento, EmTransporte, Entregue, EmTroca, Trocado, AguardandoConfirmacao"
                    });
                }
                
                var transacao = await _transacaoService.AtualizarStatusPedidoAsync(pedidoId, novoStatus);
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

        [HttpPut("/Transacao/AtualizarStatusTransacao/{transacaoId}")]
        public async Task<ActionResult<ResponseTransacaoDTO>> AtualizarStatusTransacao(int transacaoId, [FromBody] AtualizarStatusTransacaoRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.StatusTransacao))
                {
                    return BadRequest(new ResponseTransacaoDTO
                    {
                        Mensagem = "O campo 'statusTransacao' é obrigatório."
                    });
                }
                
                // Converter string para enum (case-insensitive)
                if (!Enum.TryParse<Models.Enums.StatusTransacao>(request.StatusTransacao, true, out var novoStatus))
                {
                    return BadRequest(new ResponseTransacaoDTO
                    {
                        Mensagem = $"Status inválido: '{request.StatusTransacao}'. Valores aceitos: Aprovado, Reprovado, Pendente"
                    });
                }
                
                var transacao = await _transacaoService.AtualizarStatusTransacaoAsync(transacaoId, novoStatus);
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

        [HttpGet("/Transacao/GraficoPorVenda")]
        public async Task<IActionResult> GetVendasPorPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            if (dataInicio > dataFim)
                return BadRequest("A data inicial não pode ser maior que a final.");

            var resultado = await _transacaoService.ObterVolumeVendasPorPeriodo(dataInicio, dataFim);
            return Ok(resultado);
        }

        [HttpGet("/Transacao/GraficoPorVendaCategoria")]
        public async Task<IActionResult> GetVendasPorCategoria([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            if (dataInicio > dataFim)
                return BadRequest("A data inicial não pode ser maior que a final.");

            var resultado = await _transacaoService.ObterVolumeVendasPorCategoria(dataInicio, dataFim);
            return Ok(resultado);
        }
    }
}
