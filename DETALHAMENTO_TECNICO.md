# Detalhamento Técnico - Sistema de Emissão de Notas Fiscais

Este documento responde aos questionamentos técnicos exigidos no escopo do teste.

### 1. Quais ciclos de vida do Angular foram utilizados?
Foi utilizado primariamente o **`ngOnInit`**. Ele foi implementado nos componentes principais (Produtos e Notas Fiscais) para disparar as requisições HTTP e carregar os dados iniciais na tela assim que os componentes são renderizados, garantindo que o usuário veja a tabela preenchida ao acessar a página.

### 2. Foi feito uso da biblioteca RxJS? Como?
**Sim**. O projeto utiliza o RxJS através de **`Observables`** nos arquivos de serviço (`stock.service.ts` e `invoicing.service.ts`). Os métodos que realizam chamadas HTTP (GET, POST) retornam Observables, e os componentes fazem o **`subscribe`** nessas chamadas. Isso garante que a interface (UI) continue responsiva de forma assíncrona enquanto aguarda a resposta dos microsserviços.

### 3. Quais outras bibliotecas foram utilizadas e para qual finalidade?
No Backend (C#):
- **Polly**: Utilizada para implementar resiliência e tratamento de falhas na arquitetura de microsserviços. Foi configurada uma política de retentativa (Retry Policy) na API de Faturamento para que ela tente se comunicar novamente com a API de Estoque caso ocorra uma instabilidade de rede temporária.
- **Entity Framework Core (SQL Server)**: Utilizado como ORM para mapeamento e persistência física dos dados de forma segura.

### 4. Para componentes visuais, quais bibliotecas foram utilizadas?
- **Bootstrap 5**: Utilizado para todo o sistema de grid responsivo, estilização de formulários, botões, tabelas e cards.
- **Bootstrap Icons**: Utilizado para a iconografia do sistema.
- **SweetAlert2**: Utilizado para substituir os alertas padrões do navegador por modais bonitos, amigáveis, e para exibir indicadores de processamento ("loading") durante requisições demoradas e respostas de erros.

### 5. Gerenciamento de dependências no Golang
*Não aplicável (O projeto foi desenvolvido 100% em C# e Angular).*

### 6. Quais frameworks foram utilizados no C#?
O backend foi construído utilizando o framework **.NET 8 (ASP.NET Core Web API)** em conjunto com o **Entity Framework Core**.

### 7. Como foram tratados os erros e exceções no backend?
- **Tratamento de Negócio**: Erros como "produto não encontrado" ou "saldo insuficiente" foram encapsulados pela camada de `AppServices` usando padrões de retorno que envelopam a mensagem. A `Controller` lê esse resultado e devolve o Status Code HTTP adequado (`400 BadRequest`, `404 NotFound`, ou `409 Conflict`).
- **Tratamento de Banco (Concorrência)**: Operações críticas (como a baixa de estoque) estão suscetíveis à `DbUpdateConcurrencyException`. Ela é tratada em um bloco `try-catch`, retornando um feedback amigável ao usuário caso duas notas tentem baixar o mesmo produto simultaneamente.
- **Falha de Comunicação**: Caso o microsserviço de Estoque esteja fora do ar, o `try-catch` na API de Faturamento intercepta a exceção do `HttpClient` e devolve um erro formatado para o Angular, que por sua vez renderiza um SweetAlert avisando o usuário.

### 8. Foi utilizado LINQ? De que forma?
**Sim, o LINQ foi amplamente utilizado**. Exemplos de aplicação no código:
- **`.Select()`**: Utilizado para transformar (mapear) listas de Entidades do banco de dados em listas de DTOs antes de devolver o JSON para o frontend.
- **`.AnyAsync()`**: Utilizado nas validações de negócio, como por exemplo, verificar se já existe um produto com o mesmo Código (SKU) cadastrado no banco, otimizando a consulta.
- **`.ToList()` e `.FirstOrDefaultAsync()`**: Utilizados para materializar coleções e buscar registros únicos no banco de dados.
