# Fase 1: Costruzione (Build)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia i file del progetto
COPY . .

# Ripristina le dipendenze e compila
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# Fase 2: Esecuzione (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copia il programma compilato dalla fase 1
COPY --from=build /app/publish .

# Copia anche il database attuale per avere i dati di partenza
COPY banca.db . 

# Espone la porta 8080
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Avvia la banca
ENTRYPOINT ["dotnet", "WebBank.Domain.dll"]