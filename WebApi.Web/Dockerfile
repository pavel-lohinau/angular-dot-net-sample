# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/aspnet AS base
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_13.x | bash \
    && apt-get install nodejs -yq
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk AS build
RUN apt-get update -yq \
    && apt-get install curl gnupg -yq \
    && curl -sL https://deb.nodesource.com/setup_13.x | bash \
    && apt-get install nodejs -yq
WORKDIR /src
COPY WebApi.Web.sln ./
COPY ["WebApi.Core/WebApi.Core.csproj", "WebApi.Core/"]
COPY ["WebApi.Infrastructure/WebApi.Infrastructure.csproj", "WebApi.Infrastructure/"]
COPY ["WebApi.Tests/WebApi.Tests.csproj", "WebApi.Tests/"]
COPY ["WebApi.Web/WebApi.Web.csproj", "WebApi.Web/"]
RUN dotnet restore "WebApi.Tests/WebApi.Tests.csproj"
COPY . .
WORKDIR "/src/WebApi.Web"
RUN dotnet build "WebApi.Web.csproj" -c Release -o /app/build

FROM build AS test
WORKDIR "/src/WebApi.Tests"

FROM build AS publish
RUN dotnet publish "WebApi.Web.csproj" -c Release -o /app/publish

## Build SPA files
#RUN curl -sL https://deb.nodesource.com/setup_10.x |  bash -
#RUN apt-get install -y nodejs
#
#RUN mkdir "/usr/src/app"
#WORKDIR "/usr/src/app"
#
#COPY ["WebApi.Web/ClientApp/package.json", "/usr/src/app/package.json"]
#RUN npm install
#
#COPY ["WebApi.Web/ClientApp/.", "/usr/src/app"]
#
#RUN npm run build

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.Web.dll"]