using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EcommerceSports.Applications.DTO;
using System.Threading.Tasks;

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
            if (requisicao == null || string.IsNullOrWhiteSpace(requisicao.MensagemUsuario))
            {
                return BadRequest("A mensagem não pode estar vazia.");
            }

            var sugestoes = await _chatbotService.ProcessarMensagem(requisicao.MensagemUsuario);
            return Ok(sugestoes);
        }
    }
}

