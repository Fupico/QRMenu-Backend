FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY QRMenuBackend/QRMenuBackend.csproj QRMenuBackend/
RUN dotnet restore QRMenuBackend/QRMenuBackend.csproj

COPY . .
RUN dotnet publish QRMenuBackend/QRMenuBackend.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV APP_WEB_ROOT_PATH=/app/data/wwwroot
ENV APP_DEFAULT_WEB_ROOT_PATH=/app/default-wwwroot
EXPOSE 8080
VOLUME ["/app/data"]

COPY --from=build /app/publish .
RUN mkdir -p /app/data/wwwroot /app/default-wwwroot \
    && if [ -d /app/wwwroot ]; then cp -a /app/wwwroot/. /app/default-wwwroot/; fi
ENTRYPOINT ["dotnet", "QRMenuBackend.dll"]
