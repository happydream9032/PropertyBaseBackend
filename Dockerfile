# syntax = docker/dockerfile:1.2

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_Kestrel__Certificates__Development__Password=dummyPass

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
#RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
#USER appuser

RUN --mount=type=secret,id=_env,dst=/etc/secrets/.env cat /etc/secrets/.env


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["PropertyBase.csproj", "./"]
RUN dotnet restore "PropertyBase.csproj"

RUN dotnet dev-certs https --clear
RUN dotnet dev-certs https

COPY . .
WORKDIR "/src/."
RUN dotnet build "PropertyBase.csproj" -c Release -o /app/build
FROM build AS publish
#RUN dotnet publish "PropertyBase.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN dotnet publish "PropertyBase.csproj" -c Release -o /app/publish
#RUN dotnet dev-certs https -ep ${HOME}/.aspnet/https/PropertyBase.pfx -p dummyPass
#RUN dotnet user-secrets init
#RUN dotnet user-secrets -p ./PropertyBase.csproj set "Kestrel:Certificates:Development:Password" "dummyPass"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PropertyBase.dll"]
