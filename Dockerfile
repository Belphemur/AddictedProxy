ARG MAIN_PROJECT=AddictedProxy
ARG DATA_DIRECTORY="/data"
ARG RELEASE_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim AS base
RUN apt update && apt install -y curl && apt-get clean
RUN curl -O http://http.us.debian.org/debian/pool/main/libz/libzstd/libzstd1_1.5.5+dfsg2-2_amd64.deb && dpkg -i libzstd1_1.5.5+dfsg2-2_amd64.deb && rm libzstd1_1.5.5+dfsg2-2_amd64.deb
# Needed because the zstd lib is loaded as libzstd not libzstd.so.1
RUN ln -srf /lib/x86_64-linux-gnu/libzstd.so.1 /lib/x86_64-linux-gnu/libzstd.so
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS restore
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

HEALTHCHECK --interval=15s --timeout=3s \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "app.dll"]
