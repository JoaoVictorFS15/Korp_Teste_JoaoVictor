# 📦 Korp ERP - Teste Técnico (Faturamento & Estoque)

Este projeto consiste em uma solução de ERP baseada em arquitetura de microsserviços (.NET 8) e um frontend robusto (Angular 17), focado em consistência, resiliência e boas práticas de Engenharia de Software.

## 🚀 Arquitetura e Tecnologias

### Microsserviços (Backend)
- **.NET 8 Web API**
- **Entity Framework Core (In-Memory)**: Utilizado para persistência ágil, simulando um banco de dados real.
- **Padrão Repository & AppService**: O projeto não acessa o banco de dados diretamente pelas Controllers. Toda regra de negócio está encapsulada nos *AppServices*, respeitando os princípios de Clean Architecture.
- **Polly (Resiliência)**: A comunicação entre as APIs (`Faturamento` -> `Estoque`) utiliza a biblioteca Polly para políticas de *Retry* e controle de falhas de comunicação HTTP.
- **Transações Distribuídas (Resolução de Inconsistência)**: Ao invés de deduzir o estoque item a item (o que causaria inconsistência caso um produto do meio falhasse), foi implementado um **Endpoint de Lote (Bulk Deduct)** na API de Estoque. A dedução ocorre via `SaveChangesAsync()`, que funciona como uma transação atômica nativa do EF Core, garantindo que o estoque de múltiplos itens só seja debitado se todos tiverem saldo suficiente (All-or-Nothing).

### Frontend
- **Angular 17 (Standalone Components)**
- **RxJS**: Gerenciamento assíncrono para os fluxos HTTP.
- **Bootstrap 5 + SweetAlert2**: Interface limpa, responsiva e com feedback visual instantâneo para erros e sucessos do usuário.
- **Formulários (Template-Driven)**: Com validações em tempo real impedindo submissões incorretas de quantidade ou produtos.
- **Tradução Inteligente de Erros**: O frontend intercepta os erros do microsserviço (como a falta de saldo de um ProductId) e mescla com os dados da interface, avisando ao usuário de forma legível *qual* produto (Código/Nome) falhou, economizando chamadas desnecessárias no banco de dados.

## ⚙️ Como Executar

1. Inicie a **API de Estoque** (`Korp.Stock.API`) na porta `7229`.
2. Inicie a **API de Faturamento** (`Korp.Invoicing.API`) na porta `7285`.
3. Na pasta do Frontend (`korp-frontend`), execute `npm install` para instalar os pacotes, seguido de `npm start` (ou `ng serve`).
4. Acesse a interface web via `http://localhost:4200`.
