FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7167
EXPOSE 5190

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cookbook.Api/Cookbook.Api.csproj", "./Cookbook.Api/"]
COPY ["Cookbook.Api.Components/Cookbook.Api.Components.csproj", "./Cookbook.Api.Components/"]
COPY ["Cookbook.Api.Application/Cookbook.Api.Application.csproj", "./Cookbook.Api.Application/"]
COPY ["Cookbook.Shared/Cookbook.Shared.csproj", "./Cookbook.Shared/"]
COPY ["Cookbook.Contracts/Cookbook.Contracts.csproj", "./Cookbook.Contracts/"]
RUN dotnet restore "Cookbook.Api/Cookbook.Api.csproj"
COPY . .
WORKDIR "/src/Cookbook.Api"
RUN dotnet build "/src/Cookbook.Api/Cookbook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/Cookbook.Api/Cookbook.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Api.dll"]
