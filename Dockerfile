ARG MAIN_PROJECT=AddictedProxy
ARG DATA_DIRECTORY="/data"
ARG RELEASE_VERSION="1.0.0"

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
RUN  adduser -D dotnetuser

RUN apk add --no-cache curl dumb-init
USER dotnetuser

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS restore
ARG MAIN_PROJECT
WORKDIR /src
COPY . .
WORKDIR "/src/${MAIN_PROJECT}"
RUN dotnet restore --locked-mode "${MAIN_PROJECT}.csproj"

FROM restore AS publish
ARG MAIN_PROJECT
ARG RELEASE_VERSION
RUN dotnet publish "${MAIN_PROJECT}.csproj" -p:Version=$RELEASE_VERSION -p:AssemblyVersion=$RELEASE_VERSION  -c Release -o /app/publish


FROM base AS final
ARG MAIN_PROJECT

WORKDIR /app
COPY --from=publish /app/publish .

RUN ln -s ${MAIN_PROJECT}.dll app.dll

HEALTHCHECK --interval=5s --timeout=3s \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dumb-init","dotnet", "app.dll"]
