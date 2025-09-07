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

        [HttpPut("/Cliente/Editar/{id}")]
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
        [HttpPut("/Cliente/AlterarStatus/{id}")]
        public async Task<IActionResult> AtualizarStatus(int id, [FromBody] EditarStatusClienteDTO status)
        {
            try
            {
                await _clienteService.AtualizarStatusCliente(id, status);
                return Ok("Status atualizado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPut("/Cliente/AlterarSenha/{id}")]
        public async Task<IActionResult> AtualizarSenha(int id, [FromBody] EditarSenhaDTO senha)
        {
            try
            {
                await _clienteService.AtualizarSenha(id, senha);
                return Ok("Senha atualizada com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        [HttpPost]
        [Route("/Cliente/Cadastrar")]
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
        [HttpGet]
        [Route("/Cliente/Listar")]
        public async Task<IActionResult> ListarClientes()
        {
            var clientes = await _clienteService.ListarDadosCliente();
            return Ok(clientes);
        }

    }
}




