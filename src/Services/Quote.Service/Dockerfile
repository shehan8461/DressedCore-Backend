FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Shared/Dressed.Shared/Dressed.Shared.csproj", "Shared/Dressed.Shared/"]
COPY ["src/Services/Quote.Service/Quote.Service.csproj", "Services/Quote.Service/"]

RUN dotnet restore "Services/Quote.Service/Quote.Service.csproj"

COPY src/Shared/Dressed.Shared/. Shared/Dressed.Shared/
COPY src/Services/Quote.Service/. Services/Quote.Service/

WORKDIR "/src/Services/Quote.Service"
RUN dotnet build "Quote.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Quote.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Quote.Service.dll"]
