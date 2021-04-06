#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ms-rutas-backend/ms-rutas-backend.csproj", "ms-rutas-backend/"]
COPY ["ms-rutas-backend.Data/ms-rutas-backend.Data.csproj", "ms-rutas-backend.Data/"]
RUN dotnet restore "ms-rutas-backend/ms-rutas-backend.csproj"
COPY . .
WORKDIR "/src/ms-rutas-backend"
RUN dotnet build "ms-rutas-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ms-rutas-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ms-rutas-backend.dll"]