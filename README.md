# Gestão de Tarefas API

API REST desenvolvida em .NET 8 para gerenciamento de tarefas, seguindo boas práticas de arquitetura, princípios SOLID e organização em camadas.

O projeto permite:

* Criar tarefas
* Editar tarefas
* Listar tarefas
* Filtrar tarefas
* Excluir tarefas logicamente
* Validar regras de negócio
* Documentar endpoints via Swagger
* Executar testes unitários e de integração

---

# Tecnologias Utilizadas

* .NET 8
* ASP.NET Core Web API
* Entity Framework Core InMemory
* Swagger / OpenAPI
* xUnit
* FluentAssertions
* ILogger
* Middleware global de tratamento de exceções

---

# Arquitetura do Projeto

O projeto foi estruturado em camadas para separar responsabilidades e facilitar manutenção, testes e evolução futura.

```text
src/
 └── GestaoTarefas.Api
      ├── Controllers
      ├── Services
      ├── Repositories
      ├── Models
      ├── DTOs
      ├── Data
      ├── Middlewares
      ├── Exceptions
      ├── Shared
      └── Enums

tests/
 ├── GestaoTarefas.Tests.Unitarios
 └── GestaoTarefas.Tests.Integracao
```

---

# Decisões Técnicas

## .NET 8

Foi utilizada a versão mais recente do .NET por questões de:

* performance
* suporte de longo prazo
* melhorias no minimal hosting
* melhor integração com observabilidade e cloud

---

## Entity Framework Core InMemory

Foi utilizado o provider InMemory para simplificar a execução do projeto sem necessidade de configuração de banco externo.

Isso facilita:

* execução local
* testes automatizados
* avaliação técnica

A estrutura foi preparada para permitir troca futura para SQL Server/PostgreSQL com baixo impacto.

---

## Separação em Camadas

A aplicação foi organizada em:

### Controller

Responsável apenas por:

* receber requisições
* retornar respostas HTTP
* delegar regras para o Service

### Service

Responsável pelas:

* regras de negócio
* validações adicionais
* orquestração da aplicação

### Repository

Responsável pelo:

* acesso aos dados
* abstração do Entity Framework

Essa separação reduz acoplamento e facilita testes unitários.

---

# SOLID Aplicado

## Single Responsibility Principle

Cada classe possui apenas uma responsabilidade.

Exemplo:

* Controller → HTTP
* Service → regra de negócio
* Repository → persistência

---

## Dependency Inversion Principle

As dependências são injetadas via interfaces utilizando Dependency Injection nativa do ASP.NET Core.

Exemplo:

```csharp
builder.Services.AddScoped<ITarefaService, TarefaService>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
```

---

## Open/Closed Principle

A estrutura permite expansão futura sem necessidade de alterar código existente.

Exemplo:

* novos filtros
* novos status
* autenticação
* paginação

---

# Funcionalidades

## Criar tarefa

Permite criar uma nova tarefa.

Campos:

* Título (obrigatório)
* Descrição (opcional)
* Data de vencimento (opcional)
* Status

---

## Listar tarefas

Permite:

* listar todas as tarefas
* filtrar por status
* filtrar por data de vencimento

---

## Atualizar tarefa

Permite atualizar:

* título
* descrição
* status
* data de vencimento

---

## Exclusão lógica

A exclusão foi implementada de forma lógica.

Ao excluir uma tarefa:

* ela não é removida fisicamente
* o campo `Excluida` é marcado como `true`
* a tarefa deixa de aparecer nas consultas

Essa abordagem foi escolhida por ser comum em sistemas corporativos, permitindo:

* auditoria
* rastreabilidade
* recuperação futura

---

# Tratamento Global de Erros

Foi implementado um middleware global para tratamento centralizado de exceções.

Exemplo:

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
```

Objetivos:

* padronizar respostas
* evitar exposição de erros internos
* melhorar manutenção
* melhorar experiência do consumidor da API

---

# Logging

Foi utilizado `ILogger` para registrar eventos importantes da aplicação.

Exemplos:

* criação de tarefas
* atualização
* exclusão lógica
* erros inesperados

---

# Validações

As validações foram divididas em duas camadas:

## DTOs

Validações simples utilizando DataAnnotations:

* Required
* MaxLength
* MinLength
* EnumDataType

## Services

Validações de regra de negócio e validações defensivas.

---

# Swagger

A documentação da API foi implementada utilizando Swagger/OpenAPI.

Ao executar o projeto:

```text
https://localhost:{porta}
```

---

# Como Executar o Projeto

## Clonar repositório

```bash
git clone <url-do-repositorio>
```

---

## Restaurar dependências

```bash
dotnet restore
```

---

## Executar aplicação

```bash
dotnet run --project src/GestaoTarefas.Api
```

---

## Executar testes

```bash
dotnet test
```

---

# Endpoints

## Criar tarefa

```http
POST /api/v1/tarefas
```

---

## Listar tarefas

```http
GET /api/v1/tarefas
```

Filtros opcionais:

```http
GET /api/v1/tarefas?status=Pendente
GET /api/v1/tarefas?dataVencimento=2026-05-25
```

---

## Buscar tarefa por ID

```http
GET /api/v1/tarefas/{id}
```

---

## Atualizar tarefa

```http
PUT /api/v1/tarefas/{id}
```

---

## Excluir tarefa

```http
DELETE /api/v1/tarefas/{id}
```

---

# Testes Automatizados

O projeto possui:

## Testes Unitários

Focados na camada de Service:

* validações
* regras de negócio
* fluxos de sucesso e erro

---

## Testes de Integração

Focados nos endpoints HTTP:

* status codes
* serialização
* integração entre camadas

---

# Possíveis Evoluções Futuras

* autenticação JWT
* paginação
* ordenação dinâmica
* banco relacional
* cache distribuído
* Docker
* CI/CD
* observabilidade
* métricas
* health checks
* versionamento avançado da API

---

# Autor

Desenvolvido como desafio técnico para avaliação de conhecimentos em:

* .NET
* arquitetura
* boas práticas
* APIs REST
* testes automatizados
* organização de código
* qualidade de software
