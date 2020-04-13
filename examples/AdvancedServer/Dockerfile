#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["examples/AdvancedServer/AdvancedServer.csproj", "examples/AdvancedServer/"]
COPY ["src/Argo/Argo.csproj", "src/Argo/"]
RUN dotnet restore "examples/AdvancedServer/AdvancedServer.csproj"
COPY . .
WORKDIR "/src/examples/AdvancedServer"
RUN dotnet build "AdvancedServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdvancedServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdvancedServer.dll"]