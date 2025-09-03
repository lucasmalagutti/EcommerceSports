using EcommerceSports.Applications.DTO;
using EcommerceSports.Applications.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceSports.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCliente(int id, [FromBody] EditarClienteDTO cliente)
        {
            try
            {
                await _clienteService.AtualizarCliente(id, cliente);
                return Ok("Cliente atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [Route("/Cadastrar/Cliente")]
        public IActionResult CadastrarCliente([FromBody] ClienteDTO cliente)
        {
            try
            {
                _clienteService.CadastrarCliente(cliente);
                return Ok("Cliente cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}




