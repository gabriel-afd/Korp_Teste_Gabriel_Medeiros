# KorpERP - Backend

Sistema ERP desenvolvido com arquitetura **Clean Architecture** e **Domain-Driven Design (DDD)**, implementando dois bounded contexts: **Estoque** e **Faturamento**. O sistema utiliza comunicação assíncrona via **RabbitMQ** para integração entre contextos e implementa padrões de concorrência otimista e idempotência para garantir integridade dos dados.

---

## 📋 Sumário

- [Entidades e Modelos](#-entidades-e-modelos)
- [Tecnologias Utilizadas](#-tecnologias-utilizadas)
- [Como Rodar o Projeto](#-como-rodar-o-projeto)
- [Endpoints Principais](#-endpoints-principais)
- [Mensageria (RabbitMQ)](#-mensageria-rabbitmq)
- [Arquitetura](#-arquitetura)
- [Estrutura de Pastas](#-estrutura-de-pastas)
- [Padrões e Boas Práticas](#-padrões-e-boas-práticas)

---

## 📦 Entidades e Modelos

### Bounded Context: Estoque

#### **Produto**
| Atributo | Tipo | Descrição |
|----------|------|-----------|
| `Id` | `Guid` | Identificador único |
| `Codigo` | `string` | Código alfanumérico do produto (único) |
| `Descricao` | `string` | Descrição do produto |
| `Saldo` | `int` | Quantidade em estoque |
| `RowVersion` | `byte[]` | Token de concorrência otimista |

#### **MensagemProcessada**
| Atributo | Tipo | Descrição |
|----------|------|-----------|
| `MessageId` | `string` | ID da mensagem RabbitMQ (PK) |
| `ProcessadaEm` | `DateTime` | Data/hora do processamento |

---

### Bounded Context: Faturamento

#### **NotaFiscal**
| Atributo | Tipo | Descrição |
|----------|------|-----------|
| `Id` | `Guid` | Identificador único |
| `Numero` | `int` | Número sequencial da nota |
| `Status` | `StatusNota` | Enum: `Aberta` ou `Fechada` |
| `Data` | `DateTime` | Data de criação |
| `Itens` | `List<ItemNota>` | Coleção de itens |

#### **ItemNota**
| Atributo | Tipo | Descrição |
|----------|------|-----------|
| `Id` | `Guid` | Identificador único |
| `NotaFiscalId` | `Guid` | Chave estrangeira |
| `CodigoProduto` | `string` | Código do produto (referência) |
| `DescricaoProduto` | `string` | Descrição do produto |
| `Quantidade` | `int` | Quantidade do item |

---

## 🛠️ Tecnologias Utilizadas

### **Estoque.Api**
- **.NET 8.0** — Framework principal
- **Microsoft.EntityFrameworkCore.Design 8.0** — Migrations e Code-First
- **RabbitMQ.Client 7.2.1** — Consumer assíncrono para débito de estoque
- **Swashbuckle.AspNetCore 6.4.0** — Documentação Swagger

### **Estoque.Infra.Data**
- **Microsoft.EntityFrameworkCore 8.0** — ORM
- **Microsoft.EntityFrameworkCore.SqlServer 8.0** — Provider SQL Server
- **Microsoft.EntityFrameworkCore.Tools 8.0** — CLI para migrations

### **Faturamento.Api**
- **.NET 8.0**
- **Microsoft.EntityFrameworkCore.Design 8.0**
- **Swashbuckle.AspNetCore 6.4.0**

### **Faturamento.Infra.Data**
- **Microsoft.EntityFrameworkCore 8.0**
- **Microsoft.EntityFrameworkCore.SqlServer 8.0**
- **RabbitMQ.Client 7.2.1** — Publisher de eventos
- **Microsoft.Extensions.Http.Polly 8.0** — Retry + Circuit Breaker para chamadas HTTP

### **Faturamento.Infra.IoC**
- **Polly 8.0** — Políticas de resiliência (retry exponencial + circuit breaker)

---

## 🚀 Como Rodar o Projeto

### **Pré-requisitos**
- [Docker Desktop](https://www.docker.com/products/docker-desktop) instalado e rodando
- Porta **1434** (SQL Server), **5672** e **15672** (RabbitMQ), **5001** e **5002** (APIs) disponíveis

> ⚠️ **Importante**: Se você tiver SQL Server instalado localmente, ele geralmente usa a porta **1433**. O docker-compose está configurado para usar a porta **1434** para evitar conflitos.

---

### **Executando com Docker Compose**

1. **Clone o repositório e navegue até a pasta do projeto:**
   ```bash
   cd F:\KorpERP\back-end\KorpERP
   ```

2. **Execute o Docker Compose:**
   ```bash
   docker compose up --build
   ```

3. **Aguarde a inicialização** (SQL Server, RabbitMQ e ambas as APIs):
   - SQL Server estará disponível em `localhost:1434`
   - RabbitMQ Management em `http://localhost:15672` (user: `guest`, password: `guest`)
   - Estoque API em `http://localhost:5001`
   - Faturamento API em `http://localhost:5002`

4. **As migrations são aplicadas automaticamente** via `Database.EnsureCreated()` no `Program.cs` de cada API.

---

### **Parando os serviços**

```bash
docker compose down
```

Para limpar volumes (banco de dados será recriado):
```bash
docker compose down -v
```

---

### **Executando localmente (sem Docker)**

1. **Certifique-se de ter:**
   - SQL Server rodando localmente (porta padrão 1433)
   - RabbitMQ rodando em `localhost:5672`

2. **Atualize as connection strings em `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost,1433;Database=EstoqueDb;User Id=sa;Password=SuaSenha;TrustServerCertificate=True;"
   },
   "RabbitMQ": {
     "Host": "localhost"
   }
   ```

3. **Execute as migrations:**
   ```bash
   cd Estoque.Infra.Data
   dotnet ef database update --startup-project ../KorpERP
   
   cd ../Faturamento.Infra.Data
   dotnet ef database update --startup-project ../Faturamento.Api
   ```

4. **Execute as APIs:**
   ```bash
   # Terminal 1
   cd KorpERP
   dotnet run
   
   # Terminal 2
   cd Faturamento.Api
   dotnet run
   ```

---

## 📡 Endpoints Principais

### **Estoque API** (`http://localhost:5001`)

#### **Swagger UI**
```
http://localhost:5001/swagger
```

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/produtos` | Lista produtos paginados |
| `GET` | `/api/produtos/{codigo}` | Busca produto por código |
| `POST` | `/api/produtos` | Cria novo produto |

**Exemplo de criação de produto:**
```json
POST /api/produtos
{
  "codigo": "PROD-001",
  "descricao": "Notebook Dell",
  "saldo": 10
}
```

---

### **Faturamento API** (`http://localhost:5002`)

#### **Swagger UI**
```
http://localhost:5002/swagger
```

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/api/notasfiscais` | Lista notas fiscais paginadas |
| `GET` | `/api/notasfiscais/{id}` | Busca nota por ID |
| `POST` | `/api/notasfiscais` | Cria nova nota fiscal |
| `POST` | `/api/notasfiscais/{id}/imprimir` | Fecha nota e debita estoque |

**Exemplo de criação de nota:**
```json
POST /api/notasfiscais
{
  "itens": [
    {
      "codigoProduto": "PROD-001",
      "descricao": "Notebook Dell",
      "quantidade": 2
    }
  ]
}
```

**Impressão de nota:**
```
POST /api/notasfiscais/{id}/imprimir
```
✅ Valida saldo disponível (síncrono)  
✅ Fecha a nota  
✅ Publica evento no RabbitMQ  
✅ Consumer do Estoque debita o saldo (assíncrono)

---

## 🐰 Mensageria (RabbitMQ)

### **Fluxo de Impressão de Nota Fiscal**

```
┌─────────────────┐
│ Faturamento API │
└────────┬────────┘
         │
         │ 1. POST /imprimir
         │
         ├─► Valida estoque (HTTP síncrono)
         │   ├─ GET Estoque API /produtos/{codigo}
         │   └─ Verifica saldo >= quantidade
         │
         ├─► Fecha nota (Status = Fechada)
         │
         ├─► Publica evento: NotaImpressaEvent
         │   └─ Fila: "nota-impressa"
         │       {
         │         "notaId": "guid",
         │         "itens": [
         │           {
         │             "codigoProduto": "PROD-001",
         │             "quantidade": 2
         │           }
         │         ]
         │       }
         ▼
┌─────────────────┐
│    RabbitMQ     │
│  (porta 5672)   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Estoque API    │
│  (Consumer)     │
└─────────────────┘
         │
         ├─► Verifica idempotência (MessageId)
         ├─► Debita saldo (com retry em caso de concorrência)
         ├─► Salva MessageId processado
         └─► Confirma mensagem (BasicAckAsync)
```

### **Configurações RabbitMQ**

- **Fila:** `nota-impressa`
- **Durable:** `true` (mensagens persistem em disco)
- **AutoAck:** `false` (confirmação manual para garantir processamento)
- **Prefetch:** `1` (processa uma mensagem por vez)

### **Acesso ao Painel de Gerenciamento**
```
http://localhost:15672
Usuário: guest
Senha: guest
```

---

## 🏗️ Arquitetura

O projeto segue **Clean Architecture** com **Domain-Driven Design (DDD)**, separando cada bounded context em camadas independentes.

### **Princípios aplicados:**

| Camada | Responsabilidade | Dependências |
|--------|------------------|--------------|
| **Domain** | Entidades, regras de negócio, exceções | Nenhuma (núcleo isolado) |
| **Application** | DTOs, Services, Interfaces, Mappers | Domain |
| **Infra.Data** | DbContext, Repositories, Migrations, Clients | Application, Domain, EF Core |
| **Infra.IoC** | Configuração de DI (Dependency Injection) | Application, Infra.Data |
| **API** | Controllers, Middlewares | Application, Infra.IoC |

### **Regra de dependência:**
```
API → Infra.IoC → Infra.Data → Application → Domain
       ↑                                        ↑
       └────────────────────────────────────────┘
              (Domain não depende de nada)
```

### **Padrões implementados:**

#### **1. Rich Domain Model**
Entidades encapsulam suas próprias regras de negócio:
```csharp
public class Produto
{
    public int Saldo { get; private set; } // Setter privado
    
    public void Debitar(int quantidade)
    {
        if (Saldo < quantidade)
            throw new SaldoInsuficienteException();
        
        Saldo -= quantidade;
    }
}
```

#### **2. Repository Pattern**
Abstração de acesso a dados com interfaces no Application:
```csharp
public interface IProdutoRepository
{
    Task<Produto?> GetByCodigoAsync(string codigo);
    Task UpdateAsync(Produto produto);
}
```

#### **3. Static Mappers**
Conversão entre entidades e DTOs sem dependências externas (sem AutoMapper):
```csharp
public static class ProdutoMapper
{
    public static ProdutoResponseDto ToResponseDto(Produto produto);
    public static PagedResult<ProdutoResponseDto> ToPagedResult(PagedResult<Produto> result);
}
```

#### **4. Event-Driven Architecture**
Integração assíncrona via eventos de domínio publicados no RabbitMQ.

#### **5. Global Error Handling**
Middleware centralizado que converte exceções em respostas HTTP padronizadas:
- `DomainException` → 400 Bad Request
- `DbUpdateConcurrencyException` → 409 Conflict
- `HttpRequestException` / `BrokenCircuitException` → 503 Service Unavailable

---

## 📁 Estrutura de Pastas

```
KorpERP/
│
├── Estoque.Domain/
│   ├── Entities/
│   │   ├── Produto.cs
│   │   └── MensagemProcessada.cs
│   ├── Exceptions/
│   │   ├── DomainException.cs
│   │   ├── ProdutoNaoEncontradoException.cs
│   │   ├── SaldoInsuficienteException.cs
│   │   └── ConcorrenciaException.cs
│   └── Common/
│       └── PagedResult.cs
│
├── Estoque.Application/
│   ├── DTOs/
│   │   ├── Create/
│   │   │   └── ProdutoCreateDto.cs
│   │   └── Response/
│   │       └── ProdutoResponseDto.cs
│   ├── Interfaces/
│   │   └── IProdutoRepository.cs
│   ├── Mappers/
│   │   └── ProdutoMapper.cs
│   └── Services/
│       └── ProdutoService.cs
│
├── Estoque.Infra.Data/
│   ├── Data/
│   │   └── EstoqueDbContext.cs
│   ├── Repositories/
│   │   └── ProdutoRepository.cs
│   └── Migrations/
│       └── (geradas via EF Core CLI)
│
├── Estoque.Infra.IoC/
│   └── DependencyInjection.cs
│
├── Estoque.Api/ (KorpERP)
│   ├── Controllers/
│   │   └── ProdutosController.cs
│   ├── Consumers/
│   │   ├── NotaImpressaConsumer.cs
│   │   ├── NotaImpressaEvent.cs
│   │   └── ItemNotaEvent.cs
│   ├── Middlewares/
│   │   └── ErrorHandlingMiddleware.cs
│   ├── Dockerfile/
│   │   └── Dockerfile
│   └── Program.cs
│
├── Faturamento.Domain/
│   ├── Entities/
│   │   ├── NotaFiscal.cs
│   │   └── ItemNota.cs
│   ├── Enums/
│   │   └── StatusNota.cs
│   ├── Exceptions/
│   │   ├── DomainException.cs
│   │   ├── NotaFiscalNaoEncontradaException.cs
│   │   └── NotaFiscalJaFechadaException.cs
│   └── Common/
│       └── PagedResult.cs
│
├── Faturamento.Application/
│   ├── DTOs/
│   │   ├── Create/
│   │   │   ├── NotaFiscalCreateDto.cs
│   │   │   └── ItemNotaCreateDto.cs
│   │   └── Response/
│   │       ├── NotaFiscalResponseDto.cs
│   │       └── ItemNotaResponseDto.cs
│   ├── Interfaces/
│   │   ├── INotaFiscalRepository.cs
│   │   ├── IEstoqueClient.cs
│   │   └── IEventPublisher.cs
│   ├── Mappers/
│   │   ├── NotaFiscalMapper.cs
│   │   └── ItemNotaMapper.cs
│   ├── Messaging/
│   │   ├── NotaImpressaEvent.cs
│   │   └── ItemNotaEvent.cs
│   └── Services/
│       └── NotaFiscalService.cs
│
├── Faturamento.Infra.Data/
│   ├── Data/
│   │   └── FaturamentoDbContext.cs
│   ├── Repositories/
│   │   └── NotaFiscalRepository.cs
│   ├── Clients/
│   │   └── EstoqueClient.cs (HTTP + Polly)
│   ├── Messaging/
│   │   └── RabbitMqPublisher.cs
│   └── Migrations/
│       └── (geradas via EF Core CLI)
│
├── Faturamento.Infra.IoC/
│   └── DependencyInjection.cs
│
├── Faturamento.Api/
│   ├── Controllers/
│   │   └── NotasFiscaisController.cs
│   ├── Middlewares/
│   │   └── ErrorHandlingMiddleware.cs
│   ├── Dockerfile/
│   │   └── Dockerfile
│   └── Program.cs
│
└── docker-compose.yml
```

---

## ✅ Padrões e Boas Práticas

### **1. Concorrência Otimista (`RowVersion`)**
Protege contra race conditions em operações simultâneas:
- SQL Server gera `rowversion` automaticamente
- `DbUpdateConcurrencyException` → `ConcorrenciaException`
- Consumer faz até 3 tentativas com backoff exponencial

**Teste:** Imprimir 2 notas com produto de saldo 1 simultaneamente.

---

### **2. Idempotência (MessageId)**
Garante que mensagens duplicadas não causem efeitos colaterais:
- Tabela `MensagensProcessadas` armazena IDs processados
- Consumer verifica antes de processar
- `autoAck: false` para confirmação manual

**Teste:** Republicar mensagem com mesmo `MessageId` no RabbitMQ Management.

---

### **3. Resiliência com Polly**
EstoqueClient (HTTP) aplica:
- **Retry:** 3 tentativas com exponential backoff (2s, 4s, 8s)
- **Circuit Breaker:** Abre após 3 falhas consecutivas, aguarda 30s

---

### **4. Validação em Duas Camadas**
- **Camada 1 (Síncrona):** Valida estoque antes de fechar nota (99% dos casos)
- **Camada 2 (Assíncrona):** `RowVersion` protege race conditions verdadeiras

---

### **5. LINQ (Language Integrated Query)**

O projeto utiliza LINQ extensivamente para consultas, projeções e transformações de dados. Principais métodos aplicados:

#### **Métodos utilizados:**

| Método | Onde é usado | Finalidade |
|--------|--------------|------------|
| **`Select`** | Mappers, Repositories | Projeta coleções de entidades em DTOs ou eventos |
| **`ToList`** | Mappers, Repositories | Materializa `IEnumerable` em `List<T>` |
| **`FirstOrDefault`** | Repositories | Busca o primeiro elemento ou `null` se não existir |
| **`Include`** | Repositories | Eager loading de propriedades de navegação (EF Core) |
| **`Where`** | Repositories | Filtragem de entidades por condição |
| **`OrderByDescending`** | Repositories | Ordenação decrescente (ex: notas por número) |
| **`Skip` / `Take`** | Repositories | Paginação de resultados |
| **`CountAsync`** | Repositories | Contagem assíncrona para total de registros |
| **`AnyAsync`** | Consumer | Verifica existência de mensagem processada (idempotência) |

#### **Exemplos práticos:**

**1. Projeção com Select + ToList (Mappers):**
```csharp
// ProdutoMapper.cs
public static List<ProdutoResponseDto> ToResponseDtoList(IEnumerable<Produto> produtos)
{
    return produtos.Select(ToResponseDto).ToList();
}

// NotaFiscalMapper.cs
Itens = nota.Itens.Select(ItemNotaMapper.ToResponseDto).ToList()
```

**2. Busca com FirstOrDefault (Repositories):**
```csharp
// ProdutoRepository.cs
public async Task<Produto?> GetByCodigoAsync(string codigo)
{
    return await _context.Produtos
        .FirstOrDefaultAsync(p => p.Codigo == codigo);
}
```

**3. Eager Loading com Include (Repositories):**
```csharp
// NotaFiscalRepository.cs
public async Task<NotaFiscal?> GetByIdAsync(Guid id)
{
    return await _context.NotasFiscais
        .Include(n => n.Itens)  // Carrega os itens junto com a nota
        .FirstOrDefaultAsync(n => n.Id == id);
}
```

**4. Paginação com Skip, Take e OrderByDescending:**
```csharp
// NotaFiscalRepository.cs
var items = await query
    .OrderByDescending(n => n.Numero)
    .Skip((pagina - 1) * tamanhoPagina)
    .Take(tamanhoPagina)
    .ToListAsync();
```

**5. Verificação de existência com AnyAsync:**
```csharp
// NotaImpressaConsumer.cs
if (await context.MensagensProcessadas.AnyAsync(m => m.MessageId == messageId))
{
    // Mensagem já foi processada (idempotência)
    return;
}
```

#### **LINQ to Objects vs LINQ to Entities:**
- **LINQ to Entities (EF Core):** `Include`, `FirstOrDefaultAsync`, `CountAsync` — traduzidos para SQL
- **LINQ to Objects (memória):** `Select`, `ToList` nos mappers — trabalham com dados já carregados

---

### **6. Tratamento de Erros e Exceções**

O sistema implementa uma estratégia em camadas para tratamento de erros, garantindo que exceções sejam convertidas em respostas HTTP apropriadas.

#### **Hierarquia de Exceções:**

```
DomainException (base abstrata)
    ├── ProdutoNaoEncontradoException
    ├── ProdutoInvalidoException
    ├── SaldoInsuficienteException
    ├── ConcorrenciaException
    ├── NotaFiscalNaoEncontradaException
    └── NotaFiscalJaFechadaException
```

Todas herdam de `DomainException`, permitindo tratamento centralizado.

#### **Camadas de Tratamento:**

**1. Domain Layer — Lançamento de exceções:**
```csharp
// Produto.cs
public void Debitar(int quantidade)
{
    if (quantidade <= 0)
        throw new ProdutoInvalidoException("Quantidade inválida");

    if (Saldo < quantidade)
        throw new SaldoInsuficienteException();

    Saldo -= quantidade;
}
```

**2. Repository Layer — Conversão de exceções de infraestrutura:**
```csharp
// ProdutoRepository.cs
public async Task UpdateAsync(Produto produto)
{
    try
    {
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        throw new ConcorrenciaException(); // Converte para exceção de domínio
    }
}
```

**3. API Layer — Middleware global:**
```csharp
// ErrorHandlingMiddleware.cs
public class ErrorHandlingMiddleware
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(new { error = ex.Message });
        }
        catch (BrokenCircuitException ex)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsJsonAsync(new { error = "Serviço temporariamente indisponível" });
        }
        catch (HttpRequestException ex)
        {
            context.Response.StatusCode = 503;
            await context.Response.WriteAsJsonAsync(new { error = "Falha na comunicação com serviço externo" });
        }
        catch (Exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Erro interno do servidor" });
        }
    }
}
```

#### **Mapeamento de Exceções → Status HTTP:**

| Exceção | Status | Significado |
|---------|--------|-------------|
| `DomainException` (e derivadas) | 400 Bad Request | Regra de negócio violada |
| `ConcorrenciaException` | 409 Conflict¹ | Conflito de concorrência detectado |
| `BrokenCircuitException` (Polly) | 503 Service Unavailable | Circuit breaker aberto |
| `HttpRequestException` | 503 Service Unavailable | Falha na comunicação entre APIs |
| `Exception` (genérica) | 500 Internal Server Error | Erro não tratado |

¹ *No Estoque API, `ConcorrenciaException` retorna 400 como `DomainException`. No contexto de retry do consumer, é tratada internamente.*

#### **Tratamento Específico no Consumer:**

```csharp
// NotaImpressaConsumer.cs
private static async Task DebitarComRetryAsync(ProdutoService service, string codigo, int quantidade, int maxRetries = 3)
{
    for (var tentativa = 1; tentativa <= maxRetries; tentativa++)
    {
        try
        {
            await service.DebitarAsync(codigo, quantidade);
            return; // Sucesso
        }
        catch (ConcorrenciaException) when (tentativa < maxRetries)
        {
            // Retry com backoff exponencial
            await Task.Delay(100 * tentativa);
        }
        // Se chegar na última tentativa, a exceção sobe e o Ack é enviado
    }
}
```

**Importante:** O consumer envia `BasicAckAsync` mesmo em caso de erro para evitar loop infinito — erros de negócio (ex: saldo insuficiente) são registrados e a mensagem é descartada.

#### **Validação em Cascata:**

**Exemplo: Imprimir Nota Fiscal**
```
1. Controller → Service
2. Service valida estoque (EstoqueClient com Polly)
   ├─ Retry até 3x em caso de falha HTTP
   └─ Circuit breaker abre após 3 falhas consecutivas
3. Service fecha nota
4. Publica mensagem no RabbitMQ
5. Consumer debita estoque
   ├─ Retry até 3x em caso de ConcorrenciaException
   └─ Salva MessageId (idempotência)
```

Cada camada trata apenas as exceções relevantes ao seu contexto, propagando erros de domínio para cima e convertendo erros de infraestrutura quando necessário.

---

## 📄 Licença

Este projeto foi desenvolvido como teste técnico para a **Korp by Viasoft**.

---

## 👤 Autor

**Gabriel Medeiros**   
🔗 [GitHub](https://github.com/gabriel-afd)
