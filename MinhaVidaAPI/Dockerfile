# Etapa 1: Build [cite: 1]
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copia e restaura dependências [cite: 1]
COPY *.csproj ./
RUN dotnet restore

# Copia o restante dos arquivos e publica [cite: 1, 2]
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa 2: Runtime [cite: 1]
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Copia os arquivos publicados da etapa de build [cite: 2]
COPY --from=build /app/out .

# Configuração de porta para o Render detectar corretamente
# Alterado de 10000 para 8080 (padrão do Render) [cite: 3]
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "MinhaVidaAPI.dll"]