# API de Cadastro de Cartões de Crédito

## Visão Geral

Este documento descreve o fluxo de cadastro de cartões de crédito implementado seguindo as regras de negócio RN0024 e RN0025.

## Regras de Negócio Implementadas

### RN0024 - Composição do registro de cartões de crédito
Todo cartão de crédito associado a um cliente deve ser composto pelos seguintes campos:
- **Nº do Cartão**: Número do cartão (13-19 dígitos)
- **Nome impresso no Cartão**: Nome como aparece no cartão
- **Bandeira do Cartão**: Bandeira do cartão (enum)
- **Código de Segurança**: CVC (3-4 dígitos dependendo da bandeira)

### RN0025 - Bandeiras permitidas para registro de cartões de crédito
Todo cartão de crédito deve ser de alguma bandeira registrada no sistema:
- 1 - Visa
- 2 - Mastercard
- 3 - American Express
- 4 - Elo
- 5 - HiperCard
- 6 - Aura

## Endpoints Disponíveis

### 1. Cadastrar Cartão
**POST** `/api/Cartao/Cadastrar/Cartao/{clienteId}`

**Parâmetros da URL:**
- `clienteId` (int): ID do cliente

**Body:**
```json
{
  "numCartao": "4532 1234 5678 9012",
  "nomeImpresso": "JOAO DA SILVA",
  "bandeira": 1,
  "cvc": 123,
  "preferencial": true
}
```

**Resposta de Sucesso (200):**
```json
{
  "mensagem": "Cartão cadastrado com sucesso!",
  "cartao": {
    "id": 1,
    "numCartao": "4532 1234 5678 9012",
    "nomeImpresso": "JOAO DA SILVA",
    "bandeira": 1,
    "preferencial": true,
    "clienteId": 1,
    "bandeiraNome": "Visa"
  }
}
```

### 2. Listar Cartões por Cliente
**GET** `/api/Cartao/cliente/{clienteId}`

**Resposta (200):**
```json
[
  {
    "id": 1,
    "numCartao": "4532 1234 5678 9012",
    "nomeImpresso": "JOAO DA SILVA",
    "bandeira": 1,
    "preferencial": true,
    "clienteId": 1,
    "bandeiraNome": "Visa"
  }
]
```



## Validações Implementadas

**Nota**: Todas as validações foram removidas conforme solicitado. O fluxo agora aceita qualquer valor nos campos sem validação.

## Códigos de Erro

### 500 - Internal Server Error
- Erro interno do servidor

## Exemplo de Uso

```bash
# Cadastrar um cartão
curl -X POST "https://localhost:7000/api/Cartao/Cadastrar/Cartao/1" \
  -H "Content-Type: application/json" \
  -d '{
    "numCartao": "4532 1234 5678 9012",
    "nomeImpresso": "JOAO DA SILVA",
    "bandeira": 1,
    "cvc": 123,
    "preferencial": true
  }'

# Listar cartões de um cliente
curl -X GET "https://localhost:7000/api/Cartao/cliente/1"
```

## Arquivos Criados/Modificados

### Novos Arquivos
- `Applications/DTO/CadastrarCartaoDTO.cs` - DTO para cadastro
- `Applications/DTO/ResponseCartaoDTO.cs` - DTO para resposta
- `Controllers/CartaoController.cs` - Controller da API
- `exemplo-cadastro-cartao.json` - Exemplo de uso

### Arquivos Modificados
- `Applications/Services/Interfaces/ICartaoService.cs` - Interface do service
- `Applications/Services/CartaoService.cs` - Implementação do service
- `Data/Repository/Interfaces/ICartaoRepository.cs` - Interface do repository
- `Data/Repository/CartaoRepository.cs` - Implementação do repository

## Estrutura do Banco de Dados

A tabela `Cartoes` já existe e possui os seguintes campos:
- `Id` (PK)
- `NumCartao` (string)
- `NomeImpresso` (string)
- `Cvc` (int)
- `Bandeira` (enum)
- `Preferencial` (bool)
- `ClienteId` (FK)

## Considerações de Segurança

⚠️ **IMPORTANTE**: Em um ambiente de produção, considere:
- Criptografar dados sensíveis (número do cartão, CVC)
- Implementar autenticação e autorização
- Usar HTTPS
- Implementar rate limiting
- Logs de auditoria para operações sensíveis
