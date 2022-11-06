ARG MAIN_PROJECT=AddictedProxy
ARG DATA_DIRECTORY="/data"

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
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
ARG DATA_DIRECTORY
ARG NEW_RELIC_KEY
ENV DB_PATH=$DATA_DIRECTORY


# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y newrelic-dotnet-agent \
&& rm -rf /var/lib/apt/lists/* 

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=$NEW_RELIC_KEY \
NEW_RELIC_APP_NAME=$MAIN_PROJECT


WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -s ${MAIN_PROJECT}.dll app.dll && mkdir $DATA_DIRECTORY
VOLUME $DATA_DIRECTORY
ENTRYPOINT ["dotnet", "app.dll"]
