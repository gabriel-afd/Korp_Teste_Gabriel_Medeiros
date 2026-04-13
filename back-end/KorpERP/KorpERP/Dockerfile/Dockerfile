FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["KorpERP/Estoque.Api.csproj", "KorpERP/"]
COPY ["Estoque.Application/Estoque.Application.csproj", "Estoque.Application/"]
COPY ["Estoque.Domain/Estoque.Domain.csproj", "Estoque.Domain/"]
COPY ["Estoque.Infra.Data/Estoque.Infra.Data.csproj", "Estoque.Infra.Data/"]
COPY ["Estoque.Infra.IoC/Estoque.Infra.IoC.csproj", "Estoque.Infra.IoC/"]

RUN dotnet restore "KorpERP/Estoque.Api.csproj"

COPY . .
WORKDIR "/src/KorpERP"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Estoque.Api.dll"]