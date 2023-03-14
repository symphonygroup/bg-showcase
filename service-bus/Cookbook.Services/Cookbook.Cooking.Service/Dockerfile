FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 7169
EXPOSE 5192

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cookbook.Cooking.Service/Cookbook.Cooking.Service.csproj", "./Cookbook.Cooking.Service/"]
COPY ["Cookbook.Cooking.Components/Cookbook.Cooking.Components.csproj", "./Cookbook.Cooking.Components/"]
COPY ["Cookbook.Cooking.Application/Cookbook.Cooking.Application.csproj", "./Cookbook.Cooking.Application/"]
COPY ["Cookbook.Shared/Cookbook.Shared.csproj", "./Cookbook.Shared/"]
COPY ["Cookbook.Contracts/Cookbook.Contracts.csproj", "./Cookbook.Contracts/"]
RUN dotnet restore "Cookbook.Cooking.Service/Cookbook.Cooking.Service.csproj"
COPY . .
WORKDIR "/src/Cookbook.Cooking.Service"
RUN dotnet build "/src/Cookbook.Cooking.Service/Cookbook.Cooking.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/Cookbook.Cooking.Service/Cookbook.Cooking.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Cooking.Service.dll"]