# 🚀 Estudo de Caso: O Impacto do Repository Pattern com EF Core no .NET 10

Este repositório foi criado para estudar, na prática, a real necessidade de aplicar o **Repository Pattern** em aplicações modernas que utilizam o **Entity Framework Core** e **Minimal APIs** no .NET 10.

O objetivo principal foi comparar a complexidade de código, performance e manutenibilidade entre duas abordagens de arquitetura de dados.

---

## 🧐 O Problema: A "Armadilha" do Repository Pattern

Por muito tempo, a criação de interfaces como `IRepository<T>` e classes genéricas base foi considerada uma "boa prática" incontestável no ecossistema C#. Os principais argumentos para utilizá-la sempre foram:

1. **Independência de Banco de Dados:** A promessa de poder trocar de banco (ex: SQLite para SQL Server) alterando apenas uma linha de código.
2. **Testabilidade:** Facilidade para buildar testes de unidade mockando as interfaces do repositório.

### A Realidade Prática

Durante o desenvolvimento deste projeto, ficou evidente que o **DbContext do EF Core já é, por si só, uma implementação do padrão Repository/Unit of Work**. Ao criar uma nova camada de repositório por cima dele, acabamos gerando uma **abstração sobre outra abstração**, resultando em:

- Código redundante (_Boilerplate_).
- Perda de flexibilidade nativa do LINQ nos endpoints.
- Complexidade desnecessária para operações simples de CRUD.

---

## 🛠️ O Experimento

O projeto foi dividido e desenvolvido em duas etapas distintas para análise:

### Parte 1: Abordagem com Repository Pattern (Para consulta verifique a branch repositorrypattern)

Foi implementada uma estrutura completa de abstração:

- `BaseEntity` e `IRepository<T>` genérico.
- `Repository<T>` implementando métodos base do EF Core.
- `IGamesRepository` e `SqliteGamesRepository` para regras específicas (como buscas complexas com `.Include()`).
- Injeção de dependência explícita de cada repositório no `Program.cs`.

### Parte 2: Refatoração para Uso Direto do DbContext (Abordagem Atual - Versionada na branch main)

Os endpoints da Minimal API foram refatorados para receber diretamente o `GameStoreContext`. As consultas passaram a utilizar o poder nativo do EF Core diretamente nas rotas, aproveitando recursos como carregamento explícito (`DbContext.Entry().Reference().LoadAsync()`) e projeções diretas para DTOs.

---

## 📊 Resultados: Menos Código, Mesma Eficiência

A maior prova do impacto dessa refatoração está no controle de versão. Ao eliminar a camada de repositório desnecessária, foi possível **deletar arquivos inteiros de infraestrutura**, tornando o projeto muito mais limpo e focado no que realmente importa: a regra de negócio.

Visualmente, a limpa no projeto ficou assim no painel do Source Control:

![Estrutura de arquivos modificados e deletados](./screenshots/source-control-deleted-files.jpg)

### 🔍 Conclusões do Estudo

1. **A mítica troca de Banco de Dados:** Em ambientes de produção reais, a troca completa de um motor de banco de dados é extremamente rara. Mesmo que ocorra, o Repository Pattern não resolve problemas críticos como migração de dados e dialetos SQL específicos. O EF Core já lida com a troca de provedores nativamente de forma muito eficiente.
2. **Estratégia de Testes:** Testar controllers ou endpoints de API com testes de unidade puramente mockados gera falso sentimento de segurança. Para a camada de exposição da API (Endpoints), **testes de integração** utilizando o banco de dados em memória (ou SQLite local) geram muito mais valor e cobrem o comportamento real da aplicação.
3. **Manutenibilidade:** Menos código escrito significa menos código para manter, debugar e atualizar no futuro.

---

## 💻 Tecnologias Utilizadas

- **.NET 10** (Minimal APIs)
- **Entity Framework Core 10**
- **SQLite** (Como banco de dados local em modo WAL)
- **VS Code** (Como IDE de desenvolvimento)
