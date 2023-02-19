FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /app
COPY . .
RUN dotnet restore

# publish stage
FROM build AS publish
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release --no-restore --no-build -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine as runtime
WORKDIR /app
COPY --from=publish /app/published-app .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT [ "dotnet", "HelpDeskPortal.dll" ]