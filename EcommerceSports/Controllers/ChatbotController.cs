using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EcommerceSports.Applications.DTO;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatbotController : ControllerBase
    {
        private readonly IChatbotService _chatbotService;

        public ChatbotController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost("sugerir")]
        public async Task<IActionResult> SugerirProduto([FromBody] ChatbotMensagemDTO requisicao)
        {
            try
            {
                if (requisicao == null)
                {
                    return BadRequest(new ChatbotRespostaDTO
                    {
                        Tipo = "erro",
                        Mensagem = "Requisição inválida."
                    });
                }

                // Obter userId se disponível (pode vir do header, query string ou body)
                int? usuarioId = null;
                if (requisicao.UsuarioId.HasValue)
                {
                    usuarioId = requisicao.UsuarioId.Value;
                }
                else if (Request.Headers.ContainsKey("X-Usuario-Id"))
                {
                    if (int.TryParse(Request.Headers["X-Usuario-Id"].FirstOrDefault(), out var id))
                    {
                        usuarioId = id;
                    }
                }

                var resposta = await _chatbotService.ProcessarMensagem(requisicao.MensagemUsuario ?? "", usuarioId);
                
                // Garantir que a resposta nunca seja nula
                if (resposta == null)
                {
                    resposta = new ChatbotRespostaDTO
                    {
                        Tipo = "erro",
                        Mensagem = "Erro ao processar sua solicitação. Tente novamente."
                    };
                }

                // Garantir que a mensagem nunca seja nula ou vazia
                if (string.IsNullOrWhiteSpace(resposta.Mensagem))
                {
                    resposta.Mensagem = "Não consegui processar sua solicitação. Pode reformular?";
                }

                return Ok(resposta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ChatbotRespostaDTO
                {
                    Tipo = "erro",
                    Mensagem = "Erro interno do servidor. Tente novamente mais tarde."
                });
            }
        }
    }
}

