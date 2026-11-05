FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish AppService.Api/AppService.Api.csproj -c Release -o /out --nologo
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /out/ ./
RUN mkdir -p /app/config && chown -R 1000:1000 /app
USER 1000:1000
ENV ASPNETCORE_ENVIRONMENT=Development DOTNET_ENVIRONMENT=Development
ENTRYPOINT ["dotnet","AppService.Api.dll","--contentRoot","/app/config","--urls","http://127.0.0.1:18121"]
COPY deploy/qa/migrations/ /qa-migrations/
