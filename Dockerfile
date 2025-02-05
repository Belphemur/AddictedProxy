ARG MAIN_PROJECT=AddictedProxy
ARG DATA_DIRECTORY="/data"
ARG RELEASE_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:9.0-bookworm-slim AS base
RUN  adduser --disabled-password --gecos '' dotnetuser
# Install zstd
RUN echo "deb http://httpredir.debian.org/debian trixie main" > /etc/apt/sources.list.d/trixie.list && \
    apt-get update && \
    apt-get -t trixie install -y --no-install-recommends libzstd1 && \
    apt-get clean && \
    cd /lib/*-linux-gnu/ && \
    ln -srf libzstd.so.1 libzstd.so

RUN apt update && apt install -y curl dumb-init && apt-get clean
USER dotnetuser

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0-bookworm-slim AS restore
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
