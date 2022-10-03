FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Cookbook.Api/Cookbook.Api.csproj", "Cookbook.Api/"]
RUN dotnet restore "Cookbook.Api/Cookbook.Api.csproj"
COPY . .
WORKDIR "/src/Cookbook.Api"
RUN dotnet build "Cookbook.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cookbook.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cookbook.Api.dll"]
