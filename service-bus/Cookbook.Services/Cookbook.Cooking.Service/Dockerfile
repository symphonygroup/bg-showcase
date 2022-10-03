FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cookbook.Cooking.Service/Cookbook.Cooking.Service.csproj", "Cookbook.Cooking.Service/"]
RUN dotnet restore "Cookbook.Cooking.Service/Cookbook.Cooking.Service.csproj"
COPY . .
WORKDIR "/src/Cookbook.Cooking.Service"
RUN dotnet build "Cookbook.Cooking.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cookbook.Cooking.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Cooking.Service.dll"]
