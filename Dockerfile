ARG MAIN_PROJECT=AddictedProxy
ARG DATA_DIRECTORY="/data"
ARG PYRSCOPE_AGENT_VERSION="v0.8.4-pyroscope"

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
ARG PYRSCOPE_AGENT_VERSION
WORKDIR /app
RUN wget -qO- "https://github.com/grafana/pyroscope-dotnet/releases/download/${PYRSCOPE_AGENT_VERSION}/pyroscope.musl.tar.gz" | tar xvz -C /lib/
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
ENV PYROSCOPE_APPLICATION_NAME=${MAIN_PROJECT}
ENV PYROSCOPE_APPLICATION_TAGS="env:prod"
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
ENV CORECLR_PROFILER_PATH=/lib/Pyroscope.Profiler.Native.so 
ENV LD_PRELOAD=/lib/Pyroscope.Linux.ApiWrapper.x64.so
RUN ln -s ${MAIN_PROJECT}.dll app.dll

ENTRYPOINT ["dotnet", "app.dll"]
