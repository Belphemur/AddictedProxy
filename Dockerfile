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


WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -s ${MAIN_PROJECT}.dll app.dll && mkdir $DATA_DIRECTORY
VOLUME $DATA_DIRECTORY
ENTRYPOINT ["dotnet", "app.dll"]
