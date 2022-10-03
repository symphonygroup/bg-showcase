FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cookbook.Inventory.Service/Cookbook.Inventory.Service.csproj", "Cookbook.Inventory.Service/"]
RUN dotnet restore "Cookbook.Inventory.Service/Cookbook.Inventory.Service.csproj"
COPY . .
WORKDIR "/src/Cookbook.Inventory.Service"
RUN dotnet build "Cookbook.Inventory.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cookbook.Inventory.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Inventory.Service.dll"]
