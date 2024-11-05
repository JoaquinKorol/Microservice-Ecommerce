# Etapa 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar los archivos .csproj y restaurar las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar todo el código fuente y construir el proyecto
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Etapa 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar los archivos compilados desde la etapa de construcción
COPY --from=build /app/out ./

# Exponer el puerto en el que la aplicación escuchará
EXPOSE 8080

# Configurar el entrypoint
ENTRYPOINT ["dotnet", "Payment.dll"]