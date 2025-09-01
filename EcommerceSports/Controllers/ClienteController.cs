using EcommerceSports.Applications.DTO;
using EcommerceSports.Services.Interfaces;
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



        [HttpPost]
        [Route("/Cadastrar/Cliente")]
        public async Task<IActionResult> CadastrarCliente([FromBody] ClienteDTO cliente)
        {
            try
            {
                await _clienteService.CadastrarCliente(cliente);
                return Ok("Cliente cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}




