# 💰 Dashboard Gestão Casal - MinhaVida v1.0

![.NET 10](https://img.shields.io/badge/.NET-10.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-WASM-purple)
![Azure](https://img.shields.io/badge/Azure-AI_Vision-blue)
![MudBlazor](https://img.shields.io/badge/UI-MudBlazor-pink)

Um ecossistema financeiro completo para casais e empreendedores, focado na separação inteligente de fluxos pessoais e empresariais. O sistema utiliza **Inteligência Artificial (OCR)** para automatizar o lançamento de gastos a partir de fotos de comprovantes.



## 🌟 Diferenciais do Projeto
- **Arquitetura Fullstack:** Separação clara entre o cliente (Blazor WASM) e a API (ASP.NET Core).
- **IA com Azure Vision:** Leitura automática de valor, data e descrição de comprovantes Pix e boletos.
- **Gestão 4-em-1:** Monitoramento de 4 caixas distintos (Eu Pessoal, Dela Pessoal, Minha Empresa, Studio Dela).
- **Cálculo Consolidado Inteligente:** Lógica matemática robusta que garante a subtração automática de saídas e soma de entradas nos dashboards.

## 🛠️ Tecnologias e Ferramentas
- **Frontend:** Blazor WebAssembly com .NET 10.
- **Backend:** ASP.NET Core Web API com .NET 10.
- **UI:** MudBlazor v7 (Componentes modernos e responsivos).
- **IA/Cloud:** Azure Computer Vision SDK (OCR).
- **Banco de Dados:** Entity Framework Core com SQL Server.

## 🧠 Lógica de Funcionamento
1. **Lançamento OCR:** O usuário envia uma imagem; a API processa via Azure e devolve os dados estruturados.
2. **Filtros por Mês:** Sistema de seleção de mês de referência que atualiza tabelas e gráficos em tempo real.
3. **Distribuição Automática:** Se o OCR detecta palavras como "STUDIO" ou "SECRET", o sistema sugere o destino "Business - Namorada" automaticamente.
4. **Visualização Patrimonial:** Gráficos de barras e rosca comparando a evolução e proporção do patrimônio de cada um.

## ⚙️ Como Rodar Localmente

### Pré-requisitos
- Visual Studio 2022 (v17.10 ou superior para .NET 10).
- SDK .NET 10.0.

### Passo a Passo
1. Clone este repositório:
   ```bash
   git clone [https://github.com/Tognolli0/Dashboard_Gestao_Casal.git](https://github.com/Tognolli0/Dashboard_Gestao_Casal.git)

Configure suas chaves no appsettings.json ou via User Secrets:

AzureAPIKey: Sua chave do Azure AI Vision.

AzureEndpoint: O endpoint do seu recurso na Azure.

Execute o script de Migrations para criar o banco de dados:

Bash
dotnet ef database update --project MinhaVidaAPI
Inicie o Backend e o Frontend simultaneamente.

🛡️ Segurança de Dados
Este projeto utiliza tratamento rigoroso de segredos. As chaves de API devem ser gerenciadas preferencialmente via User Secrets para evitar exposição no controle de versão.

Desenvolvido por Diogo Tognolli 🚀
