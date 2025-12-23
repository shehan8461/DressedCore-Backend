FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Shared/Dressed.Shared/Dressed.Shared.csproj", "Shared/Dressed.Shared/"]
COPY ["src/Services/Auth.Service/Auth.Service.csproj", "Services/Auth.Service/"]

RUN dotnet restore "Services/Auth.Service/Auth.Service.csproj"

COPY src/Shared/Dressed.Shared/. Shared/Dressed.Shared/
COPY src/Services/Auth.Service/. Services/Auth.Service/

WORKDIR "/src/Services/Auth.Service"
RUN dotnet build "Auth.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auth.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.Service.dll"]
