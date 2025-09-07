using Microsoft.AspNetCore.Mvc;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EnderecoController : ControllerBase
    {
        private readonly IEnderecoService _enderecoService;

        public EnderecoController(IEnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        [HttpPost("/Endereco/Cadastrar/{clienteId}")]
        public async Task<IActionResult> CadastrarEndereco(int clienteId, [FromBody] EnderecoDTO enderecoDTO)
        {
            try
            {
                if (enderecoDTO == null)
                {
                    return BadRequest(new { message = "Dados do endereço são obrigatórios" });
                }

                await _enderecoService.CadastrarEndereco(clienteId, enderecoDTO);

                return Ok(new { message = "Endereço cadastrado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpPut("/Endereco/Editar/{enderecoId}")]
        public async Task<IActionResult> EditarEndereco(int enderecoId, [FromBody] EditarEnderecoDTO enderecoDTO)
        {
            try
            {

                await _enderecoService.EditarEndereco(enderecoId, enderecoDTO);

                return Ok(new { message = "Endereço atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("Listar/{clienteId}")]
        public async Task<IActionResult> ListarEnderecosPorCliente(int clienteId)
        {
            try
            {
                var enderecos = await _enderecoService.ListarEnderecosPorCliente(clienteId);
                return Ok(enderecos);
            }
            catch (Exception)
            {
                return StatusCode(500, new { erro = "Erro interno do servidor. Tente novamente mais tarde." });
            }
        }
    }
}
