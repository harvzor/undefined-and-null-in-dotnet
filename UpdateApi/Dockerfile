FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["UpdateApi/UpdateApi.csproj", "UpdateApi/"]
RUN dotnet restore "UpdateApi/UpdateApi.csproj"
COPY . .
WORKDIR "/src/UpdateApi"
RUN dotnet build "UpdateApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UpdateApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UpdateApi.dll"]
