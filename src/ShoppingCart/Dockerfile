# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos .csproj y restaurar las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el c�digo fuente y construir el proyecto
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos compilados desde la etapa de construcci�n
COPY --from=build /app/out ./

# Exponer el puerto en el que la aplicaci�n escuchar�
EXPOSE 8080

# Configurar el entrypoint
ENTRYPOINT ["dotnet", "ShoppingCart.dll"]