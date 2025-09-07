using EcommerceSports.Applications.Services.Interfaces;
using EcommerceSports.Data.Infra.Interfaces;
using EcommerceSports.Data.Repository;
using System.Text.RegularExpressions;

namespace EcommerceSports.Applications.Services
{
    public class Validators : IValidators
    {
        private readonly IClienteRepository _clienteRepository;
        public Validators(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public void ValidarSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha) || senha.Length < 8)
            {
                throw new Exception("A senha deve ter pelo menos 8 caracteres.");
            }

            if (!Regex.IsMatch(senha, "[a-z]"))
            {
                throw new Exception("A senha deve conter pelo menos uma letra minúscula.");
            }

            if (!Regex.IsMatch(senha, "[A-Z]"))
            {
                throw new Exception("A senha deve conter pelo menos uma letra maiúscula.");
            }

            if (!Regex.IsMatch(senha, "[^a-zA-Z0-9]"))
            {
                throw new Exception("A senha deve conter pelo menos um caractere especial.");
            }

        }

    }
}
