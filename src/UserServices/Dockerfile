FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar el archivo .csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet nuget locals all --clear
RUN dotnet restore

# Copiar el resto del c�digo y construir
COPY . ./
RUN dotnet build -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Copiar los archivos compilados desde la etapa de construcci�n
COPY --from=build /app/out ./

# Exponer el puerto en el que la aplicaci�n escuchar�
EXPOSE 8080

# Configurar el entrypoint
ENTRYPOINT ["dotnet", "UserServices.dll"]