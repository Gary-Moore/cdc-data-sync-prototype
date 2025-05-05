# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all source needed
COPY ./CdcDataSyncPrototype.CdcReceiver  ./CdcDataSyncPrototype.CdcReceiver 
COPY ./CdcDataSyncPrototype.Core ./CdcDataSyncPrototype.Core
COPY ./CdcDataSyncPrototype.sln ./

WORKDIR /src/CdcDataSyncPrototype.CdcReceiver 
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CdcDataSyncPrototype.CdcReceiver.dll"]    