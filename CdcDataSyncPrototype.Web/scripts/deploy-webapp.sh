#!/bin/bash
set -e

# 🌱 Load environment variables from .env file
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"
PUBLISH_DIR="$REPO_ROOT/publish-flat"
ZIP_PATH="$REPO_ROOT/publish.zip"

# Check .env exists before sourcing
if [[ ! -f "$SCRIPT_DIR/.env" ]]; then
  echo "❌ .env file not found in $SCRIPT_DIR"
  exit 1
fi

source "$SCRIPT_DIR/.env"

: "${RESOURCE_GROUP_NAME:=cdc-sync-demo-rg}"
: "${APP_SERVICE_NAME:=cdc-webapp-$(date +%s)}"
: "${APP_SERVICE_PLAN_NAME:=cdc-sync-plan}"
: "${LOCATION:=uksouth}"
: "${SQL_SERVER_NAME:=cdc-web-sql-free-$(date +%s)}"
: "${SQL_DB_NAME:=CdcPrototype}"
: "${SQL_ADMIN_USER:=sqladmin}"
: "${SQL_ADMIN_PASSWORD:?❌ Missing SQL_ADMIN_PASSWORD in .env}"

echo "Creating resource group..."
az group create --name "$RESOURCE_GROUP_NAME" --location "$LOCATION"

echo "Creating App Service Plan..."
az appservice plan create \
    --name "$APP_SERVICE_PLAN_NAME" \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --location "$LOCATION" \
    --sku B1

echo "Creating Web App: $APP_SERVICE_NAME"
az webapp create \
    --name "$APP_SERVICE_NAME" \
    --plan "$APP_SERVICE_PLAN_NAME" \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --runtime "dotnet:8"

echo "Creating Azure SQL Server: $SQL_SERVER_NAME"
az sql server create \
  --name "$SQL_SERVER_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --location "$LOCATION" \
  --admin-user "$SQL_ADMIN_USER" \
  --admin-password "$SQL_ADMIN_PASSWORD"

echo "Creating Azure SQL DB: $SQL_DB_NAME (Serverless Tier)"
az sql db create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --server "$SQL_SERVER_NAME" \
  --name "$SQL_DB_NAME" \
  --compute-model Serverless \
  --edition GeneralPurpose \
  --family Gen5 \
  --capacity 2 \
  --auto-pause-delay 60 \
  --min-capacity 0.5 \
  --zone-redundant false

echo "Adding firewall rule for Azure services"
az sql server firewall-rule create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --server "$SQL_SERVER_NAME" \
  --name "AllowAllAzureIPs" \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0

echo "Detecting current public IP..."
MY_IP=$(curl -s https://api.ipify.org)
echo "Adding firewall rule for your IP: $MY_IP"
az sql server firewall-rule create \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --server "$SQL_SERVER_NAME" \
  --name "AllowMyIp" \
  --start-ip-address "$MY_IP" \
  --end-ip-address "$MY_IP"

echo "Applying EF Core migrations..."
SQL_CONNECTION_STRING="Server=tcp:${SQL_SERVER_NAME}.database.windows.net,1433;Initial Catalog=${SQL_DB_NAME};Persist Security Info=False;User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

dotnet ef database update --project "$REPO_ROOT" --connection "$SQL_CONNECTION_STRING"

echo "Injecting connection string into Web App"
az webapp config connection-string set \
  --name "$APP_SERVICE_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --settings ReceiverDb="$SQL_CONNECTION_STRING" \
  --connection-string-type SQLAzure


echo "Cleaning old build..."
rm -rf "$PUBLISH_DIR" "$ZIP_PATH"

echo "Publishing app..."
dotnet publish "$REPO_ROOT" -c Release -o "$PUBLISH_DIR"

WINDOWS_PUBLISH_DIR=$(cygpath -w "$PUBLISH_DIR")
WINDOWS_ZIP_PATH=$(cygpath -w "$ZIP_PATH")

echo "Zipping publish output using PowerShell..."
powershell.exe -Command "Compress-Archive -Path '$WINDOWS_PUBLISH_DIR\*' -DestinationPath '$WINDOWS_ZIP_PATH' -Force"

echo "☁️ Deploying to Azure..."
az webapp deploy --name "$APP_SERVICE_NAME" --resource-group "$RESOURCE_GROUP_NAME" --src-path "$ZIP_PATH" --type zip --restart true --verbose

echo "✅ Done! Visit: https://${APP_SERVICE_NAME}.azurewebsites.net"
read -p "Press Enter to close..." dummy
