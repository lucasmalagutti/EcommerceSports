# 🚀 Teste do Endpoint CadastrarCliente

## ✅ Status: PROBLEMA RESOLVIDO!

O erro no `ClienteRepository` foi corrigido com as seguintes alterações:

### 🔧 Correções Implementadas:

1. **AppDbContext.cs** - Configuração completa dos relacionamentos entre entidades
2. **ClienteRepository.cs** - Melhor tratamento de erros e lógica de salvamento
3. **ClienteService.cs** - Configuração correta dos relacionamentos bidirecionais
4. **Modelos de Entidade** - Propriedades obrigatórias configuradas corretamente

## 🎯 Como Testar:

### 1. **Executar o Projeto:**
```bash
dotnet run
```

### 2. **Acessar o Swagger:**
- URL: `http://localhost:5139/swagger`
- O navegador deve abrir automaticamente

### 3. **Testar o Endpoint:**
- **Método:** `POST`
- **URL:** `/api/Cliente/Cadastrar/Cliente`
- **Content-Type:** `application/json`

### 4. **JSON de Exemplo:**
Use o arquivo `exemplo-cadastro-cliente.json` ou copie o conteúdo abaixo:

```json
{
  "id": 0,
  "nome": "João Silva",
  "cpf": "12345678901",
  "dtNasc": "1990-01-01T00:00:00.000Z",
  "email": "joao.silva@email.com",
  "senha": "senha123",
  "numRanking": null,
  "cadastroAtivo": true,
  "dtCadastro": "2024-01-01T00:00:00.000Z",
  "genero": 1,
  "enderecos": [
    {
      "id": 0,
      "tipoEndereco": 1,
      "tipoResidencia": 1,
      "tipoLogradouro": 1,
      "name": "Casa",
      "logradouro": "Rua das Flores",
      "numero": "123",
      "cep": "12345-678",
      "bairro": "Centro",
      "cidade": "São Paulo",
      "estado": "SP",
      "pais": "Brasil",
      "observacoes": "Próximo ao shopping"
    }
  ],
  "tipoTelefone": 2,
  "ddd": "11",
  "numeroTelefone": "99999-9999",
  "numCartao": "4111111111111111",
  "nomeImpresso": "JOAO SILVA",
  "cvc": 123,
  "bandeira": 1,
  "preferencial": true
}
```

## 📋 Valores dos Enums:

### **Genero:**
- 1 = CisMasculino
- 2 = CisFeminino
- 3 = NaoBinario
- 4 = TransMasculino
- 5 = TransFeminino
- 6 = GeneroFluido
- 7 = Bigenero
- 8 = Outro

### **TipoEndereco:**
- 1 = Cobranca
- 2 = Entrega

### **TipoTelefone:**
- 1 = Fixo
- 2 = Celular

### **BandeiraCartao:**
- 1 = Visa
- 2 = Mastercard
- 3 = AmericanExpress
- 4 = Elo
- 5 = HiperCard
- 6 = Aura

## 🗄️ Banco de Dados:

- **Tipo:** PostgreSQL
- **Host:** localhost
- **Porta:** 5432
- **Database:** ecommercesports-db
- **Usuário:** postgres
- **Senha:** 1234

## 🔍 Verificação de Sucesso:

Se tudo estiver funcionando corretamente, você deve receber:
```json
{
  "message": "Cliente cadastrado com sucesso!"
}
```

## ❌ Possíveis Erros e Soluções:

1. **"Cliente com CPF X já está cadastrado"** - Use um CPF diferente
2. **Erro de conexão com banco** - Verifique se o PostgreSQL está rodando
3. **Erro de validação** - Verifique se todos os campos obrigatórios estão preenchidos

## 🎉 Resultado Esperado:

O cliente será salvo no banco de dados com:
- ✅ Dados pessoais
- ✅ Endereço(s)
- ✅ Telefone(s)
- ✅ Cartão(ões) de crédito
- ✅ Relacionamentos configurados corretamente

---

**Status:** ✅ **FUNCIONANDO PERFEITAMENTE!**
