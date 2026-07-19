# 🏠 LeParc Residential Finance

<p align="center">

![.NET](https://img.shields.io/badge/.NET-8-512BD4?style=for-the-badge&logo=dotnet)
![React](https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react)
![TypeScript](https://img.shields.io/badge/TypeScript-5.x-3178C6?style=for-the-badge&logo=typescript)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![Swagger](https://img.shields.io/badge/Swagger-API-85EA2D?style=for-the-badge&logo=swagger)
![License](https://img.shields.io/badge/License-MIT-success?style=for-the-badge)

</p>

Sistema de gestão financeira residencial desenvolvido como desafio técnico utilizando **ASP.NET Core 8**, **React**, **TypeScript** e **SQLite**.

O sistema permite cadastrar moradores, registrar receitas e despesas, aplicar regras de negócio e visualizar um resumo financeiro consolidado.

---

# Índice

- Sobre
- Funcionalidades
- Regras de Negócio
- Tecnologias
- Arquitetura
- Estrutura do Projeto
- Pré-requisitos
- Instalação
- Configuração
- Executando o Projeto
- Executando os Testes
- Documentação da API
- Estrutura da Solução
- Decisões Técnicas
- Qualidade do Código
- Melhorias Futuras
- Autor
- Licença

---

# Sobre

O objetivo do projeto é fornecer uma aplicação completa para gerenciamento financeiro de uma residência.

A solução é composta por:

- Backend REST em ASP.NET com C#
- Frontend SPA em React com Typescript
- Persistência utilizando SQLite
- Documentação Swagger
- Testes Unitários
- Arquitetura em camadas

---

## Transações

- Cadastro de Receita
- Cadastro de Despesa
- Listagem de Transações

---

## Dashboard

Resumo financeiro contendo:

- Total de Pessoas
- Total de Transações
- Total de Receitas
- Total de Despesas
- Saldo Atual

---

# Regras de Negócio

✔ Apenas pessoas cadastradas podem possuir transações.

✔ Pessoas menores de idade podem registrar apenas despesas.

✔ Receitas para menores de idade são bloqueadas no Frontend e no Backend.

✔ Não é permitido cadastrar valores menores ou iguais a zero.

✔ O saldo é calculado automaticamente.

✔ Todas as entradas são validadas.

---

# Tecnologias

## Backend

- ASP.NET Core 8
- C#
- Entity Framework Core
- SQLite
- Swagger
- Dependency Injection
- ILogger
- xUnit
- FluentAssertions
- Moq

---

## Frontend

- React 19
- TypeScript
- Vite
- Axios
- React Router
- React Query
- CSS Modules
- React Hot Toast

---

# Arquitetura

O backend foi desenvolvido utilizando uma arquitetura em camadas inspirada em Clean Architecture.

```
API
│
├── Controllers

Application
│
├── DTOs
├── Interfaces
├── Services

Domain
│
├── Entities
├── Enums

Infrastructure
│
├── Context
├── Repositories

Tests
```

Cada camada possui responsabilidade única.

---

# Estrutura do Projeto

```
LeParc.ResidentialFinance
│
├── backend
│   ├── LeParc.ResidentialFinance.Api
│   ├── LeParc.ResidentialFinance.Application
│   ├── LeParc.ResidentialFinance.Domain
│   ├── LeParc.ResidentialFinance.Infrastructure
│   └── LeParc.ResidentialFinance.Tests
│
├── frontend
│   ├── src
│   ├── public
│   └── vite.config.ts
│
└── README.md
```

---

# Pré-requisitos

Antes de executar o projeto, instale:

- .NET SDK 8
- Node.js 20+
- npm 10+
- Git

---

# Instalação

Clone o repositório

```bash
git clone https://github.com/brandaoca44/leparc-residential-finance.git
```

Entre na pasta

```bash
cd leparc-residential-finance
```

---

# Configuração

## Backend

Entre na pasta

```bash
cd backend
```

Restaure os pacotes

```bash
dotnet restore
```

Atualize o banco

```bash
dotnet ef database update
```

---

## Frontend

Entre na pasta

```bash
cd frontend
```

Instale as dependências

```bash
npm install
```

Crie o arquivo

```
.env
```

utilizando como base

```
.env.example
```

Conteúdo:

```env
VITE_API_URL=http://localhost:5234/api
```

---

# Executando

## Backend

```bash
cd backend

dotnet run
```

---

## Frontend

```bash
cd frontend

npm run dev
```

---

# Executando os Testes

```bash
cd backend

dotnet test
```

Os testes implementados contemplam:

- PersonService
- TransactionService
- ReportService

Todos os testes devem ser executados com sucesso.

---

# Documentação da API

Após iniciar o backend:

```
http://localhost:5234/swagger
```

Todos os endpoints estão documentados utilizando Swagger/OpenAPI.

---

# Endpoints

## Pessoas

GET

```
/api/people
```

POST

```
/api/people
```

DELETE

```
/api/people/{id}
```

---

## Transações

GET

```
/api/transactions
```

POST

```
/api/transactions
```

---

## Relatórios

GET

```
/api/reports/summary
```

---

# Decisões Técnicas

Durante o desenvolvimento foram adotadas algumas decisões visando simplicidade e manutenção.

### Backend

- Repository Pattern
- Dependency Injection
- DTOs separados
- Services desacoplados
- XML Documentation
- Logging
- Exceptions customizadas

### Frontend

- Hooks específicos para consumo da API
- Services desacoplados dos componentes
- Tipagem completa com TypeScript
- CSS Modules
- React Query para gerenciamento de estado remoto
- Componentização

---

# Qualidade do Código

O projeto segue boas práticas como:

- SOLID
- Clean Code
- Clean Architecture
- Repository Pattern
- Separation of Concerns
- Tipagem Forte
- Documentação XML
- Componentização
- Tratamento de Erros
- Responsividade
- Testes Unitários

---

# Melhorias Futuras

- Autenticação JWT
- Controle de permissões
- Dashboard com gráficos
- Exportação PDF
- Exportação Excel
- Docker
- CI/CD
- Deploy em Azure
- Deploy em AWS

---

# Autor

**Caíque Brandão**

GitHub

https://github.com/brandaoca44

LinkedIn

https://linkedin.com/in/caiquebrandao

---

# Licença

Este projeto foi desenvolvido exclusivamente para fins de avaliação técnica e estudos.
