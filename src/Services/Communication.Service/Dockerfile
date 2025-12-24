FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Shared/Dressed.Shared/Dressed.Shared.csproj", "Shared/Dressed.Shared/"]
COPY ["src/Services/Communication.Service/Communication.Service.csproj", "Services/Communication.Service/"]
RUN dotnet restore "Services/Communication.Service/Communication.Service.csproj"
COPY src/Shared/Dressed.Shared/. Shared/Dressed.Shared/
COPY src/Services/Communication.Service/. Services/Communication.Service/
WORKDIR /src/Services/Communication.Service
RUN dotnet build "Communication.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Communication.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Communication.Service.dll"]
