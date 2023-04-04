# syntax = docker/dockerfile:1.2

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:\$PORT
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_ENVIRONMENT=Development

RUN --mount=type=secret,id=_env,dst=/etc/secrets/.env cat /etc/secrets/.env

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PropertyBase.csproj", "./"]
RUN dotnet restore "PropertyBase.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "PropertyBase.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "PropertyBase.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropertyBase.dll"]
