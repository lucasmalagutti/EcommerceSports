using Microsoft.AspNetCore.Mvc;
using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CupomController : ControllerBase
    {
        private readonly ICupomService _cupomService;

        public CupomController(ICupomService cupomService)
        {
            _cupomService = cupomService;
        }

        [HttpPost("validar")]
        public async Task<ActionResult<ResponseCupomDTO>> ValidarCupom([FromBody] ValidarCupomDTO validarDto)
        {
            try
            {
                var resultado = await _cupomService.ValidarCupomAsync(validarDto.Nome);
                
                if (resultado.Valido)
                {
                    return Ok(resultado);
                }
                else
                {
                    return BadRequest(resultado);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseCupomDTO
                {
                    Valido = false,
                    Mensagem = "Erro interno do servidor: " + ex.Message
                });
            }
        }
    }
}
