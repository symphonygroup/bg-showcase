FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7168
EXPOSE 5191

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cookbook.Inventory.Service/Cookbook.Inventory.Service.csproj", "./Cookbook.Inventory.Service/"]
COPY ["Cookbook.Inventory.Components/Cookbook.Inventory.Components.csproj", "./Cookbook.Inventory.Components/"]
COPY ["Cookbook.Inventory.Application/Cookbook.Inventory.Application.csproj", "./Cookbook.Inventory.Application/"]
COPY ["Cookbook.Shared/Cookbook.Shared.csproj", "./Cookbook.Shared/"]
COPY ["Cookbook.Contracts/Cookbook.Contracts.csproj", "./Cookbook.Contracts/"]
RUN dotnet restore "Cookbook.Inventory.Service/Cookbook.Inventory.Service.csproj"
COPY . .
WORKDIR "/src/Cookbook.Inventory.Service"
RUN dotnet build "/src/Cookbook.Inventory.Service/Cookbook.Inventory.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/Cookbook.Inventory.Service/Cookbook.Inventory.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Inventory.Service.dll"]