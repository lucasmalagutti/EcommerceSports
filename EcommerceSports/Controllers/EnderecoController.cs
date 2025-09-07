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

                if (clienteId <= 0)
                {
                    return BadRequest(new { message = "ID do cliente deve ser maior que zero" });
                }

                // Validações básicas
                if (string.IsNullOrWhiteSpace(enderecoDTO.Nome))
                {
                    return BadRequest(new { message = "Nome do endereço é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Logradouro))
                {
                    return BadRequest(new { message = "Logradouro é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Numero))
                {
                    return BadRequest(new { message = "Número é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Cep))
                {
                    return BadRequest(new { message = "CEP é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Bairro))
                {
                    return BadRequest(new { message = "Bairro é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Cidade))
                {
                    return BadRequest(new { message = "Cidade é obrigatória" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Estado))
                {
                    return BadRequest(new { message = "Estado é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(enderecoDTO.Pais))
                {
                    return BadRequest(new { message = "País é obrigatório" });
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
        [HttpPut("editar/{enderecoId}")]
        public async Task<IActionResult> EditarEndereco(int enderecoId, [FromBody] EditarEnderecoDTO enderecoDTO)
        {
            try
            {
                if (enderecoDTO == null)
                {
                    return BadRequest(new { message = "Dados do endereço são obrigatórios" });
                }

                if (enderecoId <= 0)
                {
                    return BadRequest(new { message = "ID do endereço deve ser maior que zero" });
                }

                // Verifica se pelo menos um campo foi fornecido para atualização
                if (string.IsNullOrWhiteSpace(enderecoDTO.Nome) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Logradouro) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Numero) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Cep) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Bairro) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Cidade) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Estado) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Pais) &&
                    string.IsNullOrWhiteSpace(enderecoDTO.Observacoes) &&
                    !enderecoDTO.TipoLogradouro.HasValue &&
                    !enderecoDTO.TipoResidencia.HasValue)
                {
                    return BadRequest(new { message = "Pelo menos um campo deve ser fornecido para atualização" });
                }

                await _enderecoService.EditarEndereco(enderecoId, enderecoDTO);

                return Ok(new { message = "Endereço atualizado com sucesso" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno do servidor", error = ex.Message });
            }
        }
    }
}
