FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ServerApp/ServerApp.csproj", "ServerApp/"]
RUN dotnet restore "ServerApp/ServerApp.csproj"
COPY . .
WORKDIR "/src/ServerApp"
RUN dotnet build "ServerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ServerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ServerApp.dll"]
