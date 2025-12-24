FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy shared project
COPY ["src/Shared/Dressed.Shared/Dressed.Shared.csproj", "Shared/Dressed.Shared/"]
COPY ["src/Services/Payment.Service/Payment.Service.csproj", "Services/Payment.Service/"]

RUN dotnet restore "Services/Payment.Service/Payment.Service.csproj"

COPY src/Shared/Dressed.Shared/. Shared/Dressed.Shared/
COPY src/Services/Payment.Service/. Services/Payment.Service/

WORKDIR "/src/Services/Payment.Service"
RUN dotnet build "Payment.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Payment.Service.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Service.dll"]
