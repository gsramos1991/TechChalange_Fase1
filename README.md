# FCG Web API

Uma API moderna e robusta desenvolvida em **.NET 8** seguindo os princípios de **Clean Architecture**, implementando as melhores práticas de desenvolvimento empresarial e alta performance.

## 🚀 Features Implementadas

### 🎮 **Funcionalidades de Negócio**
- **Cadastro de Jogos** - CRUD completo para gerenciamento de jogos
- **Cadastro de Usuários** - Registro e autenticação
- **Sistema de Permissões** - Controle de acesso baseado em roles e claims
- **Soft Delete** - Exclusão lógica de registros mantendo histórico
- **Versionamento de Endpoints** - Suporte a múltiplas versões da API

### 🏗️ **Arquitetura & Padrões**
- **Clean Architecture** com separação clara de responsabilidades
- **Repository Pattern** para abstração de acesso a dados
- **DTO Pattern** para transferência segura de dados
- **Dependency Injection** com IoC Container nativo

## 🛠️ Bibliotecas e Componentes Implementados

### ⚡ **Sistema de Cache**
```csharp
// Implementação de cache em memória para alta performance
services.AddMemoryCache();
services.AddSingleton<ICacheService, MemCacheService>();
```
- **Cache inteligente** nos endpoints mais utilizados

### 🔍 **Correlation ID**
```csharp
// Rastreamento de requisições end-to-end
services.AddSingleton<ICorrelationIdGenerator, CorrelationIdGenerator>();
```
- **Rastreamento único** para cada requisição
- **Debugging facilitado** em ambiente de produção
- **Header personalizado**: `X-Correlation-ID`

### 📝 **Sistema de Logging**
```csharp
// Logging estruturado com Serilog
services.AddSerilog();
```
- **Serilog** para logging estruturado e flexível
- **Contextual logging** com Correlation ID integrado

### 🛡️ **Global Exception Handler**
```csharp
// Tratamento centralizado de exceções
services.AddExceptionHandler<GlobalExceptionHandler>();
services.AddProblemDetails();
```
- **Captura global** de exceções não tratadas
- **Logging automático** de erros críticos

### 🔐 **Autenticação & Autorização JWT**
```csharp
// Sistema completo de autenticação
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(/* configurações */);
```
- **JWT Bearer Tokens** para autenticação stateless
- **Claims-based authorization** para controle granular

### 📱 **Versionamento de API**
```csharp
// Suporte a múltiplas versões
services.AddApiVersioning();
```
- **Versionamento por URL**: `/api/v1/`, `/api/v2/`
- **Backward compatibility** mantida
- **Documentação separada** por versão no Swagger
- **Deprecation warnings** para versões antigas

### 📖 **Documentação Interativa**
```csharp
// Swagger/OpenAPI com documentação rica
services.AddSwaggerGen(c => {
    c.IncludeXmlComments(xmlPath);
    c.SwaggerDoc("v1", new OpenApiInfo { ... });
    c.SwaggerDoc("v2", new OpenApiInfo { ... });
});
```
- **Swagger UI** interativo para testes
- **OpenAPI 3.0** compliant
- **Documentação XML** integrada
- **Exemplos de requisições** incluídos
- **Schemas detalhados** para todos os DTOs

## 🏗️ Arquitetura da Solução

```
FCG/
├── 📁 FCG.Api/                     # 🎯 Camada de Apresentação
│   ├── 📁 Configs/                 # Configurações dos middleware
│   ├── 📁 Controllers/             # Controllers organizados por versão
│   │   ├── 📁 v1/                 # 📌 Endpoints versão 1
│   │   └── 📁 v2/                 # 🚀 Endpoints versão 2 (com cache)
│   ├── 📁 Dto/                    # Data Transfer Objects
│   ├── 📁 Extensions/             # 🔧 Configurações dos Middleware
│   ├── 📁 MappingDtos/            # 🔄 Mapeamento Entidade ↔ DTO
│   ├── 📁 Middleware/             # 🛡️ Middlewares personalizados
│
├── 📁 FCG.Business/               # 💼 Camada de Negócio
│   ├── 📁 Models/                 # 🎯 Entidades de domínio
│   └── 📁 Services/               # 🧠 Regras de negócio
│       ├── 📁 Interfaces/         # Contratos de negócio
│
├── 📁 FCG.Infra/                  # 🔧 Camada de Infraestrutura
│   ├── 📁 Cache/                  # ⚡ Implementações de cache
│   ├── 📁 Data/                   # 🗄️ Entity Framework & Contexto
│   ├── 📁 IoC/                    # 📦 Extensões de DI organizadas
│   └── 📁 Token/                  # 🗃️ Implementação do Token
│
└── 📁 FCG.Core/                   # 🌐 Utilitários Compartilhados
│   └── 📁 Interfaces/             # 🔧 Interfaces gerais
│   └── 📁 Services/               # 🔧 Serviços transversais
│
└── 📁 FCG.Tests/                  # 🌐 Testes da Solução
    └── 📁 Unitarios/              # 🔧 Testes Unitários
    └── 📁 Integracao/             # 🔧 Testes de Integração


```


## 🚀 Como Executar

### **Pré-requisitos**
- ✅ .NET 8 SDK ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
- ✅ SQL Server ou SQL Server LocalDB
- ✅ Git instalado
- ⚡ Visual Studio 2022 ou VS Code (recomendado)

### **1. Clonar o Repositório**
```bash
git clone https://github.com/seu-usuario/fcg-web-api.git
cd fcg
```

### **2. Configurar Banco de Dados**

**Edite `FCG.Api/appsettings.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FCG_DB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-super-segura-aqui",
    "Issuer": "FCG.Api",
    "Audience": "FCG.Client",
    "ExpiryMinutes": 60
  }
}
```

### **3. Executar Migrations**
```bash
cd FCG.Api
dotnet ef database update --project .\FCG.Api --context ApplicationDbContext
update-database
```

### **4. Restaurar Dependências & Executar**
```bash
# Restaurar pacotes
dotnet restore

# Executar aplicação
dotnet run --project FCG.Api
```

### **5. Acessar a Aplicação**
- 🌐 **API Base**: `https://localhost:7001`
- 📖 **Swagger Documentation**: `https://localhost:7001/swagger`
- 💚 **Health Check**: `https://localhost:7001/health`

## 💻 Idealizadores do projeto (Discord name)
- 👨‍💻Clovis Alceu Cassaro (cloves_93258)
- 👨‍💻Gabriel Santos Ramos (_gsramos)
- 👨‍💻Júlio César de Carvalho (cesarsoft)
- 👨‍💻Marco Antonio Araujo (_marcoaz)
- 👩‍💻Yasmim Muniz Da Silva Caraça (yasmimcaraca)

---

