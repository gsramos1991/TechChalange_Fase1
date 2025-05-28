# 📘 Tech Challenge – Fase 1 – FIAP Pós-Tech (.NET)

## 🧾 Visão Geral

Este projeto é parte do Tech Challenge da Fase 1 da Pós-Graduação em Arquitetura de Software da FIAP. O objetivo é desenvolver uma API RESTful utilizando .NET para gerenciar o cadastro de usuários e jogos, aplicando os conceitos de Domain-Driven Design (DDD).

## 🎯 Objetivos

- Implementar uma API para cadastro e gerenciamento de usuários e jogos.
- Aplicar os princípios de DDD na estruturação do projeto.
- Utilizar boas práticas de desenvolvimento, incluindo testes automatizados.

## 🛠️ Tecnologias Utilizadas

- .NET 8.0
- ASP.NET Core WebApi
- Entity Framework Core
- SQL Server
- xUnit para testes unitários
- Swagger para documentação da API

## 📁 Estrutura do Projeto

```
TechChalange_Fase1/
├── FCG.Api/           # Camada de apresentação (controllers, endpoints)
├── FCG.Business/      # Lógica de negócio (serviços, regras de negócio)
├── FCG.Core/          # Entidades, interfaces e contratos
├── FCG.Data/          # Acesso a dados (repositórios, contexto do EF)
├── Tests/             # Projetos de teste automatizado
├── FCG.sln            # Solução do Visual Studio
└── README.md          # Documentação do projeto
```

## 🚀 Como Executar o Projeto

1. Clone o repositório:

   ```bash
   git clone https://github.com/gsramos1991/TechChalange_Fase1.git
   ```

2. Abra a solução `FCG.sln` no Visual Studio 2022.

3. Configure a string de conexão com o SQL Server no arquivo `appsettings.json` da camada `FCG.Api`.

4. Execute as migrações para criar o banco de dados:

   ```bash
   dotnet ef database update
   ```

5. Inicie o projeto `FCG.Api` para rodar a API.

6. Acesse a documentação Swagger em:

   ```bash
   https://localhost:{porta}/swagger
   ```

## ✅ Funcionalidades Implementadas

- Cadastro de usuários com validações de e-mail e senha.
- Autenticação de usuários com geração de token JWT.
- Cadastro, atualização e remoção de jogos.
- Listagem de jogos disponíveis.
- Associação de jogos aos usuários (biblioteca pessoal).

## 🧪 Testes Automatizados

Os testes estão localizados na pasta `Tests/` e cobrem:

- Validação de regras de negócio.
- Testes de repositórios e serviços.
- Testes de integração entre camadas.

Para executar os testes:

```bash
dotnet test
```

## Idealizadores do projeto
- Clovis Alceu Cassaro
- Gabriel Santos Ramos
- Júlio César de Carvalho
- Marco Antonio Araujo
- Yasmim Muniz Da Silva caraça

## 📌 Considerações Finais

Este projeto demonstra a aplicação prática dos conceitos de DDD em uma API RESTful utilizando .NET. A estrutura modular facilita a manutenção e escalabilidade da aplicação.

