# Guia de Uso do Sistema

## Introducao e objetivo

Este projeto expoe uma API de vendas com cadastro de empresa, cliente, produto, preco, promocao e operacoes de venda. O objetivo deste guia e mostrar como subir a aplicacao e como usar o sistema ponta a ponta com exemplos reais baseados nos endpoints, contratos e regras atualmente implementados.

O backend foi construido em .NET com ASP.NET Core Web API, MediatR, AutoMapper, Entity Framework Core e PostgreSQL. Para exploracao manual da API, o caminho mais pratico e usar o Swagger em ambiente de desenvolvimento.

Este documento foi escrito a partir da implementacao atual do projeto. Sempre que houver diferenca entre uma expectativa funcional e o comportamento da API, este guia prioriza o que esta efetivamente codificado hoje.

## Como executar o projeto

### Pre-requisitos

- .NET SDK compativel com a solucao em `C:\Projetos\ambev\template\backend`
- Docker e Docker Compose
- PostgreSQL, caso voce prefira rodar fora do Docker

### Estrutura principal

- Solucao: `C:\Projetos\ambev\template\backend\Ambev.DeveloperEvaluation.sln`
- API: `C:\Projetos\ambev\template\backend\src\Ambev.DeveloperEvaluation.WebApi`
- Docker Compose: `C:\Projetos\ambev\template\backend\docker-compose.yml`


### URL da API e Swagger

Pelo arquivo `Program.cs`, o Swagger fica disponivel apenas em ambiente de desenvolvimento.

Ao rodar localmente com `dotnet run`, a URL exata pode variar conforme o perfil de execucao. Em geral, o proprio terminal do ASP.NET informa as portas. No projeto tambem existe um exemplo HTTP apontando para:

```text
http://localhost:5119
```

Se a API subir com essa porta, o Swagger normalmente ficara em:

```text
http://localhost:5119/swagger
```

Para testar endpoints protegidos no Swagger:

1. Execute `POST /api/Auth` e copie o valor de `data.token`.
2. Clique em `Authorize` no canto superior da interface.
3. Informe o JWT no campo exibido. Voce pode colar apenas o token ou usar `Bearer {token}`.
4. Execute os endpoints protegidos; o Swagger enviara automaticamente o header `Authorization`.

Se estiver usando Docker Compose, as portas HTTP e HTTPS do container sao 8080 e 8081, mas o mapeamento publicado pode ser dinamico. Verifique com:

```powershell
docker compose ps
```

### Configuracao de ambiente

O arquivo `appsettings.Development.json` contem configuracoes de desenvolvimento, e o `Program.cs` injeta o `DefaultConnection` para PostgreSQL. Se voce precisar ajustar a conexao, confira:

- `C:\Projetos\ambev\template\backend\src\Ambev.DeveloperEvaluation.WebApi\appsettings.json`
- `C:\Projetos\ambev\template\backend\src\Ambev.DeveloperEvaluation.WebApi\appsettings.Development.json`

Caso use EF Core migrations manualmente, um fluxo comum e:

```powershell
cd C:\Projetos\ambev\template\backend
dotnet ef database update --project .\src\Ambev.DeveloperEvaluation.ORM --startup-project .\src\Ambev.DeveloperEvaluation.WebApi
```

## Ordem recomendada de uso do sistema

Esta e a sequencia mais segura para testar a API sem depender de dados preexistentes:

1. Autenticar em `POST /api/Auth`
2. Criar ou localizar uma empresa em `POST /api/Companies` ou `GET /api/Companies`
3. Criar ou localizar um cliente em `POST /api/Customers` ou `GET /api/Customers`
4. Criar ou localizar um produto em `POST /api/Products` ou `GET /api/Products`
5. Cadastrar o preco do produto em `POST /api/products/{productId}/prices`
6. Opcionalmente adicionar codigo de barras em `POST /api/Products/{id}/barcodes`
7. Opcionalmente criar promocao em `POST /api/SalesPromotions`
8. Criar a venda em `POST /api/Sales`
9. Adicionar itens em `POST /api/Sales/{saleId}/items`
10. Aplicar desconto ou acrescimo manual, se necessario
11. Subtotalizar a venda em `POST /api/Sales/{saleId}/subtotalize`
12. Registrar pagamentos em `POST /api/Sales/{saleId}/payments`
13. Consultar a venda final em `GET /api/Sales/{saleId}`

## Regras do sistema

## Estados da venda

A venda trabalha com estes status:

- `Open`
- `Subtotalized`
- `PaymentCompleted`
- `EmittingNfce`
- `PrintingFiscalReceipt`
- `IntegratedWithErp`
- `Canceled`

### Regras principais de status

- Toda venda nasce em `Open`.
- Itens so podem ser alterados enquanto a venda estiver `Open`.
- A subtotalizacao so e permitida quando existir pelo menos um item ativo.
- A reabertura so e permitida quando a venda estiver `Subtotalized`.
- O cancelamento da venda so e permitido em `Open`, `Subtotalized` ou `PaymentCompleted`.
- Pagamentos so podem ser registrados em `Subtotalized` ou `PaymentCompleted`.
- A quantidade vendida por produto nao pode ultrapassar `MaxSaleQuantity` configurado no cadastro do produto.

### Regras de troco e pagamento

- O troco so existe quando o total pago ultrapassa o total da venda.
- Esse excedente so e aceito se o pagamento que gerou o excesso for do tipo `Cash`.
- Pagamento acima do total com `CreditCard`, `DebitCard` ou `Pix` deve falhar.
- O troco calculado e armazenado na colecao `Changes` da venda.

### Controle de concorrencia otimista

Algumas operacoes exigem o campo `Version`, que representa a versao atual da venda:

- `POST /api/Sales/{saleId}/subtotalize`
- `POST /api/Sales/{saleId}/reopen`
- `POST /api/Sales/{saleId}/payments`
- `POST /api/Sales/{saleId}/cancel`

Na pratica, o fluxo correto e:

1. Criar ou consultar a venda
2. Capturar o `id` e o `version` retornados
3. Usar esse `version` nas operacoes que exigem confirmacao do estado atual
4. Depois de cada operacao, atualizar o `version` local com o valor mais novo retornado pela API

## Fluxo de vendas detalhado

Esta secao mostra um cenario completo e narrativo usando IDs e valores representativos.

### Cenario base

- Empresa: `Loja Centro`
- Cliente: `Maria Oliveira`
- Produto: `Cerveja 600ml`
- Preco unitario vigente: `R$ 10,00`
- Venda inicial: 4 unidades

IDs ficticios usados nos exemplos:

- `companyId`: `11111111-1111-1111-1111-111111111111`
- `customerId`: `22222222-2222-2222-2222-222222222222`
- `productId`: `33333333-3333-3333-3333-333333333333`
- `saleId`: `44444444-4444-4444-4444-444444444444`
- `itemId`: `55555555-5555-5555-5555-555555555555`
- `authorizerId`: `66666666-6666-6666-6666-666666666666`
- `version`: `77777777-7777-7777-7777-777777777777`

### Quando usar `companyId`, `customerId`, `productId` e `ean`

- `companyId` identifica a filial ou empresa que esta emitindo a venda.
- `customerId` identifica o cliente da venda.
- `productId` deve ser usado quando o chamador ja conhece o produto internamente.
- `ean` pode ser usado ao adicionar item quando a origem da operacao e leitura de codigo de barras.

Ao adicionar item, o contrato aceita:

- `productId`
- `ean`
- `quantity`

Na pratica, voce pode informar o produto por um dos dois caminhos:

- pelo `productId`
- pelo `ean`

### Resultado esperado do fluxo

1. A venda e criada em `Open`, total zerado e sem itens.
2. Ao adicionar itens, `Subtotal` e `Total` passam a refletir os valores dos itens.
3. Ao aplicar desconto ou acrescimo, os totais do item e da venda sao recalculados.
4. Ao subtotalizar, a API primeiro reaplica as promocoes validas e depois muda a venda para `Subtotalized`.
5. Ao registrar pagamentos parciais, o total pago sobe progressivamente.
6. Quando o valor pago atinge ou ultrapassa o total:
- a venda passa por `PaymentCompleted`
- a implementacao atual ja avanca para `EmittingNfce`
- se houver excesso em dinheiro, o troco fica em `ChangeAmountTotal` e tambem em `Changes`

## Exemplos de operacoes

Os exemplos abaixo usam `curl` e assumem base URL `http://localhost:5119`.

### 1. Autenticacao

```bash
curl -X POST "http://localhost:5119/api/Auth" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@ambev.com",
    "password": "123456"
  }'
```

Exemplo de resposta esperada:

```json
{
  "success": true,
  "message": "User authenticated successfully",
  "data": {
    "token": "jwt-token-aqui",
    "email": "admin@ambev.com",
    "name": "Administrador",
    "role": "Admin"
  }
}
```

Se a API estiver protegida no seu ambiente, reutilize o token nas chamadas seguintes:

```text
Authorization: Bearer {token}
```

### 2. Criar empresa

```bash
curl -X POST "http://localhost:5119/api/Companies" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Loja Centro",
    "cnpj": "12345678000199",
    "status": 1,
    "address": {
      "street": "Rua Principal",
      "number": "100",
      "district": "Centro",
      "city": "Cuiaba",
      "state": "MT",
      "zipCode": "78000000",
      "country": "Brasil",
      "complement": "Loja 1"
    }
  }'
```

### 3. Criar cliente

```bash
curl -X POST "http://localhost:5119/api/Customers" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Maria Oliveira",
    "birthDate": "1990-05-15T00:00:00Z",
    "cpf": "12345678909",
    "status": 1
  }'
```

### 4. Criar produto

```bash
curl -X POST "http://localhost:5119/api/Products" \
  -H "Content-Type: application/json" \
  -d '{
    "description": "Cerveja 600ml",
    "unitMeasure": "UN",
    "brand": "Marca Exemplo",
    "model": "Garrafa",
    "productType": 1,
    "maxSaleQuantity": 20,
    "barcodes": []
  }'
```

### 5. Cadastrar preco do produto

```bash
curl -X POST "http://localhost:5119/api/products/33333333-3333-3333-3333-333333333333/prices" \
  -H "Content-Type: application/json" \
  -d '{
    "priceType": 1,
    "price": 10.00,
    "effectiveStartAt": "2026-06-26T00:00:00Z",
    "effectiveEndAt": "2026-12-31T23:59:59Z"
  }'
```

### 6. Adicionar codigo de barras

```bash
curl -X POST "http://localhost:5119/api/Products/33333333-3333-3333-3333-333333333333/barcodes" \
  -H "Content-Type: application/json" \
  -d '{
    "barcode": "7891234567890"
  }'
```

### 7. Criar promocao

```bash
curl -X POST "http://localhost:5119/api/SalesPromotions" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Promo Cerveja 4+",
    "description": "Desconto para compra em volume",
    "priority": 1,
    "startDate": "2026-06-01T00:00:00Z",
    "endDate": "2026-12-31T23:59:59Z",
    "productId": "33333333-3333-3333-3333-333333333333",
    "isActive": true,
    "items": [
      {
        "minimumQuantity": 4,
        "maximumQuantity": 9,
        "discountType": 1,
        "discountValue": 10
      }
    ]
  }'
```

Importante: pela implementacao atual, a promocao e aplicada no fluxo de subtotalizacao da venda. Ou seja, criar a promocao nao altera imediatamente itens ja existentes; o recalculo acontece quando a venda e subtotalizada.

### 8. Criar venda

```bash
curl -X POST "http://localhost:5119/api/Sales" \
  -H "Content-Type: application/json" \
  -d '{
    "companyId": "11111111-1111-1111-1111-111111111111",
    "customerId": "22222222-2222-2222-2222-222222222222"
  }'
```

Exemplo de leitura da resposta:

- `data.id`: identificador da venda
- `data.version`: versao atual
- `data.status`: deve vir como `Open`
- `data.total`: inicialmente `0`

### 9. Buscar venda atual por cliente

```bash
curl "http://localhost:5119/api/Sales/customers/22222222-2222-2222-2222-222222222222"
```

Esse endpoint e util quando o PDV precisa retomar a venda aberta do cliente.

### 10. Adicionar item por `productId`

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/items" \
  -H "Content-Type: application/json" \
  -d '{
    "productId": "33333333-3333-3333-3333-333333333333",
    "quantity": 4
  }'
```

### 11. Adicionar item por `ean`

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/items" \
  -H "Content-Type: application/json" \
  -d '{
    "ean": "7891234567890",
    "quantity": 2
  }'
```

### 12. Atualizar quantidade do item

```bash
curl -X PUT "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/items/55555555-5555-5555-5555-555555555555/quantity" \
  -H "Content-Type: application/json" \
  -d '{
    "quantity": 6
  }'
```

### 13. Aplicar desconto manual

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/items/55555555-5555-5555-5555-555555555555/discounts/manual" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 3.50,
    "authorizerId": "66666666-6666-6666-6666-666666666666",
    "reason": "Desconto de cortesia"
  }'
```

### 14. Aplicar acrescimo manual

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/items/55555555-5555-5555-5555-555555555555/additions/manual" \
  -H "Content-Type: application/json" \
  -d '{
    "amount": 2.00,
    "authorizerId": "66666666-6666-6666-6666-666666666666",
    "reason": "Servico adicional"
  }'
```

### 15. Subtotalizar a venda

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/subtotalize" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "77777777-7777-7777-7777-777777777777"
  }'
```

Depois dessa chamada:

- `status` deve mudar para `Subtotalized`
- o `version` retornado deve substituir o anterior no seu cliente

### 16. Registrar pagamento parcial

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/payments" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "88888888-8888-8888-8888-888888888888",
    "paymentType": 1,
    "value": 20.00
  }'
```

Leitura esperada:

- `paymentAmountTotal` sobe para `20.00`
- `changeAmountTotal` continua `0`
- o status passa por `PaymentCompleted`

### 17. Registrar pagamento com multiplos meios

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/payments" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "99999999-9999-9999-9999-999999999999",
    "paymentType": 2,
    "value": 15.00
  }'
```

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/payments" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "paymentType": 3,
    "value": 5.00
  }'
```

### 18. Registrar pagamento final em dinheiro com troco

Se a venda totalizar `R$ 40,00` e o valor acumulado pago estiver em `R$ 25,00`, um pagamento final de `R$ 20,00` em dinheiro gera troco de `R$ 5,00`.

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/payments" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "paymentType": 1,
    "value": 20.00
  }'
```

Leitura esperada:

- `paymentAmountTotal`: `45.00`
- `changeAmountTotal`: `5.00`
- `changes[0].value`: `5.00`
- status final da implementacao atual: `EmittingNfce`

### 19. Cancelar item

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/items/55555555-5555-5555-5555-555555555555/cancel" \
  -H "Content-Type: application/json" \
  -d '{
    "cancellationAuthorizerId": "66666666-6666-6666-6666-666666666666",
    "cancellationReason": "Produto danificado"
  }'
```

O item mantem seus valores historicos, mas deixa de compor os totais ativos da venda.

### 20. Reabrir venda

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/reopen" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "cccccccc-cccc-cccc-cccc-cccccccccccc"
  }'
```

Essa operacao so e valida para venda em `Subtotalized`.

### 21. Cancelar venda

```bash
curl -X POST "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444/cancel" \
  -H "Content-Type: application/json" \
  -d '{
    "version": "dddddddd-dddd-dddd-dddd-dddddddddddd",
    "cancellationAuthorizerId": "66666666-6666-6666-6666-666666666666",
    "cancellationReason": "Operacao desfeita"
  }'
```

### 22. Consultar venda

```bash
curl "http://localhost:5119/api/Sales/44444444-4444-4444-4444-444444444444"
```

Exemplo resumido de estrutura de resposta:

```json
{
  "success": true,
  "message": "Sale retrieved successfully",
  "data": {
    "id": "44444444-4444-4444-4444-444444444444",
    "saleNumber": 12345,
    "version": "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee",
    "subtotal": 40.00,
    "additionalAmountTotal": 2.00,
    "discountAmountTotal": 3.50,
    "total": 38.50,
    "paymentAmountTotal": 45.00,
    "changeAmountTotal": 6.50,
    "status": 4,
    "isCanceled": false,
    "companyId": "11111111-1111-1111-1111-111111111111",
    "customerId": "22222222-2222-2222-2222-222222222222",
    "items": [],
    "payments": [],
    "changes": []
  }
}
```

## Promocoes

## Endpoints disponiveis

- `POST /api/SalesPromotions`
- `GET /api/SalesPromotions`
- `GET /api/SalesPromotions/{id}`
- `PUT /api/SalesPromotions/{id}`
- `DELETE /api/SalesPromotions/{id}`

## Estrutura do payload

Uma promocao pode ser criada para um produto especifico usando `productId` e uma lista de faixas em `items`.

Cada item da promocao possui:

- `minimumQuantity`
- `maximumQuantity`
- `discountType`
- `discountValue`

### Tipos suportados

- `Percentage`
- `FixedAmount`
- `FixedPrice`

### Comportamento real implementado

- `Percentage`: calcula desconto sobre o subtotal do item
- `FixedAmount`: aplica desconto unitario multiplicado pela quantidade
- `FixedPrice` menor que o preco unitario: gera desconto
- `FixedPrice` maior que o preco unitario: gera acrescimo promocional

### Reaplicacao de promocoes

O servico promocional remove ajustes promocionais anteriores antes de recalcular a promocao da venda. Isso evita duplicacao quando a promocao e aplicada mais de uma vez.

## Acrescimos e descontos

O sistema trata ajustes em dois grupos:

- ajustes manuais
- ajustes promocionais

### Ajustes manuais

Os endpoints manuais sao:

- `POST /api/Sales/{saleId}/items/{itemId}/discounts/manual`
- `POST /api/Sales/{saleId}/items/{itemId}/additions/manual`

Campos relevantes:

- `amount`
- `authorizerId`
- `reason`

O `authorizerId` e obrigatorio para ajuste manual. O motivo e opcional.

Pela regra implementada no servico de ajustes:

- o autorizador precisa existir como usuario
- o autorizador precisa ter papel `Manager` ou `Admin`

### Impacto nos totais

Cada item possui estes valores principais:

- `Subtotal`: `Quantity * UnitPrice`
- `AdditionalAmountTotal`: soma dos acrescimos
- `DiscountAmountTotal`: soma dos descontos
- `Total`: `Subtotal + AdditionalAmountTotal - DiscountAmountTotal`

A venda recalcula:

- `Subtotal`
- `AdditionalAmountTotal`
- `DiscountAmountTotal`
- `Total`

com base apenas nos itens nao cancelados.

## Pagamento e troco

## Tipos de pagamento

O enum `PaymentType` possui:

- `Cash`
- `CreditCard`
- `DebitCard`
- `Pix`

## Pagamento parcial

Pagamentos parciais sao permitidos. O total pago vai sendo acumulado em `PaymentAmountTotal`.

## Pagamento com multiplos meios

Voce pode registrar varios pagamentos na mesma venda, por exemplo:

- parte em dinheiro
- parte em cartao de credito
- parte em cartao de debito
- parte em Pix

## Troco

Se o ultimo pagamento em dinheiro fizer o total pago ultrapassar o total da venda:

- o troco e calculado
- `ChangeAmountTotal` recebe esse valor
- um registro e criado em `Changes`

## Transicao de status observada

Na implementacao atual, quando a venda atinge o total pago:

1. ela sai de `Subtotalized`
2. passa por `PaymentCompleted`
3. e avanca para `EmittingNfce`

Esse detalhe e importante porque o nome `PaymentCompleted` nao significa necessariamente estado final persistido apos o ultimo pagamento.

## Cancelamentos e excecoes comuns

## Cancelamento de item

- Requer `cancellationAuthorizerId`
- Aceita `cancellationReason`
- Item cancelado nao pode ser alterado novamente
- Item cancelado deixa de compor os totais ativos da venda

## Cancelamento da venda

- Requer `version`
- Requer `cancellationAuthorizerId`
- Aceita `cancellationReason`
- So pode ocorrer em `Open`, `Subtotalized` ou `PaymentCompleted`

## Casos invalidos que devem ser esperados

- Tentar subtotalizar uma venda sem item ativo
- Tentar alterar item quando a venda nao estiver em `Open`
- Tentar ultrapassar `MaxSaleQuantity` do produto dentro da venda
- Tentar pagar acima do total com `CreditCard`, `DebitCard` ou `Pix`
- Tentar reabrir uma venda fora de `Subtotalized`
- Tentar cancelar um item que ja esta cancelado

### Observacao sobre pagamentos menores que o total

O sistema aceita pagamentos parciais e permite continuar registrando novos pagamentos depois. Portanto, o caso relevante de erro nao e "pagar valor menor e continuar pagando depois", e sim:

- tentar registrar pagamento fora dos status permitidos
- tentar gerar excesso com meio diferente de dinheiro

## Modelos de payload e respostas

## Payloads principais

### Criar venda

```json
{
  "companyId": "11111111-1111-1111-1111-111111111111",
  "customerId": "22222222-2222-2222-2222-222222222222"
}
```

### Adicionar item

```json
{
  "productId": "33333333-3333-3333-3333-333333333333",
  "quantity": 4
}
```

ou

```json
{
  "ean": "7891234567890",
  "quantity": 4
}
```

### Aplicar desconto manual

```json
{
  "amount": 3.50,
  "authorizerId": "66666666-6666-6666-6666-666666666666",
  "reason": "Desconto de cortesia"
}
```

### Aplicar acrescimo manual

```json
{
  "amount": 2.00,
  "authorizerId": "66666666-6666-6666-6666-666666666666",
  "reason": "Servico adicional"
}
```

### Subtotalizar

```json
{
  "version": "77777777-7777-7777-7777-777777777777"
}
```

### Registrar pagamento

```json
{
  "version": "88888888-8888-8888-8888-888888888888",
  "paymentType": 1,
  "value": 20.00
}
```

### Cancelar item

```json
{
  "cancellationAuthorizerId": "66666666-6666-6666-6666-666666666666",
  "cancellationReason": "Produto danificado"
}
```

### Cancelar venda

```json
{
  "version": "dddddddd-dddd-dddd-dddd-dddddddddddd",
  "cancellationAuthorizerId": "66666666-6666-6666-6666-666666666666",
  "cancellationReason": "Operacao desfeita"
}
```

## Campos importantes nas respostas

Ao longo do fluxo, capture sempre estes campos:

- `id`: identificador da venda
- `version`: versao atual da venda
- `items[*].id`: identificador do item
- `status`: status atual
- `paymentAmountTotal`: total pago
- `changeAmountTotal`: troco acumulado

## Quadro de enums uteis

### `PaymentType`

- `1 = Cash`
- `2 = CreditCard`
- `3 = DebitCard`
- `4 = Pix`

### `SaleStatus`

- `1 = Open`
- `2 = Subtotalized`
- `3 = PaymentCompleted`
- `4 = EmittingNfce`
- `5 = PrintingFiscalReceipt`
- `6 = IntegratedWithErp`
- `99 = Canceled`

### Outros enums usados com frequencia

- `CompanyStatus`: `1 = Active`, `2 = Blocked`, `3 = Inactive`
- `CustomerStatus`: `1 = Active`, `2 = Inactive`, `3 = Blocked`
- `ProductType`: `1 = Normal`, `2 = Weighable`
- `PriceType`: `1 = Cash`, `2 = Wholesale`
- `DiscountType`: `1 = Percentage`, `2 = FixedAmount`, `3 = FixedPrice`

## Resumo operacional

Se voce quiser um roteiro curto de teste funcional, use esta ordem:

1. Criar empresa
2. Criar cliente
3. Criar produto
4. Criar preco do produto
5. Opcionalmente criar promocao
6. Criar venda
7. Adicionar item
8. Aplicar ajustes, se necessario
9. Subtotalizar
10. Pagar
11. Consultar a venda final e conferir troco, status e historico

Com isso, voce cobre os principais cenarios do sistema: venda de item, promocao, desconto, acrescimo, subtotalizacao, pagamento parcial, pagamento com troco, cancelamento de item, cancelamento de venda e reabertura.
