# Dashboard Gestao Casal

Aplicacao full stack para controle financeiro pessoal e familiar, com dashboard, categorias, fluxo de caixa e automacoes ligadas a OCR.

## Stack

- Frontend em Blazor WebAssembly
- Backend em ASP.NET Core Web API
- UI com MudBlazor
- OCR com Azure AI Vision
- Persistencia local e estrutura pronta para evolucao

## Estrutura

- `MinhaVidaDashboard`: interface web
- `MinhaVidaAPI`: API, regras de negocio e persistencia
- `Dashboard.sln`: solucao principal

## Como rodar

1. Instale o SDK do .NET compativel com a solucao.
2. Configure os segredos locais fora do repositorio.
3. Ajuste `MinhaVidaAPI/appsettings.Development.json` ou User Secrets.
4. Rode a API e o frontend a partir da solucao.

## Seguranca

Este repositorio nao versiona mais credenciais reais.
Use apenas configuracoes locais para banco, Twilio e servicos externos.

## Status

Projeto mantido como versao Blazor do produto. A versao React deve ser tratada como a evolucao principal de portifolio.
