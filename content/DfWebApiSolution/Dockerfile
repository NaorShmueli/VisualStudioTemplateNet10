FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY ["Directory.Build.props", "."]
COPY ["WebApiTemplate/WebApiTemplate.csproj", "WebApiTemplate/"]
COPY ["Abstraction/Abstraction.csproj", "Abstraction/"]
COPY ["BusinessLogic/BusinessLogic.csproj", "BusinessLogic/"]
COPY ["DAL/DAL.csproj", "DAL/"]
COPY ["Models/Models.csproj", "Models/"]
COPY ["HttpManager/HttpManager.csproj", "HttpManager/"]
COPY ["Validator/Validator.csproj", "Validator/"]
RUN dotnet restore "WebApiTemplate/WebApiTemplate.csproj"

COPY . .
RUN dotnet publish "WebApiTemplate/WebApiTemplate.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "WebApiTemplate.dll"]
