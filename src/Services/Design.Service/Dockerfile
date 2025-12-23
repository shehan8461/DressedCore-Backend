FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["src/Shared/Dressed.Shared/Dressed.Shared.csproj", "Shared/Dressed.Shared/"]
COPY ["src/Services/Design.Service/Design.Service.csproj", "Services/Design.Service/"]

RUN dotnet restore "Services/Design.Service/Design.Service.csproj"

COPY src/Shared/Dressed.Shared/. Shared/Dressed.Shared/
COPY src/Services/Design.Service/. Services/Design.Service/

WORKDIR "/src/Services/Design.Service"
RUN dotnet build "Design.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Design.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Design.Service.dll"]
