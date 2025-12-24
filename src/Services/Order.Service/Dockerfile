FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Shared/Dressed.Shared/Dressed.Shared.csproj", "Shared/Dressed.Shared/"]
COPY ["src/Services/Order.Service/Order.Service.csproj", "Services/Order.Service/"]
RUN dotnet restore "Services/Order.Service/Order.Service.csproj"
COPY src/Shared/Dressed.Shared/. Shared/Dressed.Shared/
COPY src/Services/Order.Service/. Services/Order.Service/
WORKDIR "/src/Services/Order.Service"
RUN dotnet build "Order.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Order.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.Service.dll"]
