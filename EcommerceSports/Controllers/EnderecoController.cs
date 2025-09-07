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

        /// <summary>
        /// Cadastra um novo endereço para um cliente
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <param name="enderecoDTO">Dados do endereço</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("cadastrar/{clienteId}")]
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

        /// <summary>
        /// Edita um endereço existente
        /// </summary>
        /// <param name="enderecoId">ID do endereço</param>
        /// <param name="enderecoDTO">Dados atualizados do endereço</param>
        /// <returns>Resultado da operação</returns>

    }
}
