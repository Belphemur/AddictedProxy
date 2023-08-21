ARG MAIN_PROJECT=AddictedProxy
ARG DATA_DIRECTORY="/data"

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine	 AS base
RUN apk add --no-cache zstd-libs
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
ARG MAIN_PROJECT
WORKDIR /src
COPY . .
WORKDIR "/src/${MAIN_PROJECT}"
RUN dotnet build "${MAIN_PROJECT}.csproj" -c Release -o /app/build

FROM build AS publish
ARG MAIN_PROJECT
RUN dotnet publish "${MAIN_PROJECT}.csproj" -c Release -o /app/publish


FROM base AS final
ARG MAIN_PROJECT
ARG NEW_RELIC_KEY

WORKDIR /app
COPY --from=publish /app/publish .

RUN ln -s ${MAIN_PROJECT}.dll app.dll

HEALTHCHECK --interval=15s --timeout=3s \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "app.dll"]
