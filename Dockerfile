ARG MAIN_PROJECT=AddictedProxy

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
WORKDIR /app
COPY --from=publish /app/publish .
RUN ln -s ${MAIN_PROJECT}.dll app.dll 
ENTRYPOINT ["dotnet", "app.dll"]
