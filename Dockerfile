FROM node:20-alpine AS web-build
WORKDIR /web
COPY spa/package.json spa/package-lock.json* ./
RUN npm install
COPY spa .
RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src src
RUN dotnet restore src/UniversalSimulation.Api/UniversalSimulation.Api.csproj
COPY --from=web-build /src/UniversalSimulation.Api/wwwroot src/UniversalSimulation.Api/wwwroot
RUN dotnet publish src/UniversalSimulation.Api/UniversalSimulation.Api.csproj -c Release -o /app --no-restore
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet","UniversalSimulation.Api.dll"]
