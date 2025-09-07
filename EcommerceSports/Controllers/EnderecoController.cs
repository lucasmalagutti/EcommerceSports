using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : Controller
    {
        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpPost("{clienteId}/AdicionarEndereco")]
        public async Task<IActionResult> AdicionarEndereco(int clienteId, [FromBody] EnderecoDTO enderecoDTO)
        {
            try
            {
                await _enderecoService.AdicionarEndereco(clienteId, enderecoDTO);
                return Ok("Endereço adicionado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
