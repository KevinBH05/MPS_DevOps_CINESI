# --- Etapa 1: Build y Publicación ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiamos el archivo del proyecto web
COPY ["CINESI/CINESI.csproj", "CINESI/"]
RUN dotnet restore "CINESI/CINESI.csproj"

# Copiamos el resto del código
COPY . .
WORKDIR "/src/CINESI"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# --- Etapa 2: Imagen Final de Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CINESI.dll"]