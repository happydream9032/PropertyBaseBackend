# syntax = docker/dockerfile:1.2

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
RUN dotnet dev-certs https
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
#RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
#USER appuser

RUN --mount=type=secret,id=_env,dst=/etc/secrets/.env cat /etc/secrets/.env


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PropertyBase.csproj", "./"]
RUN dotnet restore "PropertyBase.csproj"


COPY . .
WORKDIR "/src/."
RUN dotnet build "PropertyBase.csproj" -c Release -o /app/build
FROM build AS publish
RUN dotnet publish "PropertyBase.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropertyBase.dll"]
