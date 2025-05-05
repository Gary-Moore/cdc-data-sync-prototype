#!/bin/bash
set -e

# Deploy CdcPublisher as an Azure Container App

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
REPO_ROOT="$(dirname "$PROJECT_DIR")" 
DOCKERFILE_PATH="$REPO_ROOT/publisher.dockerfile"
IMAGE_NAME="cdc-publisher"

# Load .env config
if [[ ! -f "$SCRIPT_DIR/.env" ]]; then
  echo "❌ .env file not found"
  exit 1
fi
source "$SCRIPT_DIR/.env"

# Fallbacks
: "${RESOURCE_GROUP_NAME:=cdc-sync-demo-rg}"
: "${CONTAINER_APP_NAME:=cdc-publisher-app}"
: "${CONTAINER_REGISTRY_NAME:=cdcpublisheracr}"
: "${LOCATION:=uksouth}"

# Required
: "${SERVICE_BUS_CONNECTION_STRING:?❌ Missing SERVICE_BUS_CONNECTION_STRING}"
: "${SQL_CONNECTION_STRING:?❌ Missing SQL_CONNECTION_STRING}"

# Login to Azure if not already
az account show > /dev/null 2>&1 || az login

# Create resource group
az group create --name "$RESOURCE_GROUP_NAME" --location "$LOCATION"

# Create ACR if it doesn't exist
if ! az acr show --name "$CONTAINER_REGISTRY_NAME" --resource-group "$RESOURCE_GROUP_NAME" &>/dev/null; then
  echo "Creating Azure Container Registry: $CONTAINER_REGISTRY_NAME"
  az acr create --name "$CONTAINER_REGISTRY_NAME" \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --sku Basic --admin-enabled true
fi

# Get ACR login server
ACR_LOGIN_SERVER=$(az acr show --name "$CONTAINER_REGISTRY_NAME" --query "loginServer" -o tsv)

# Tag and push image
echo "Building Docker image..."
docker build -f "$DOCKERFILE_PATH" -t "$IMAGE_NAME" "$REPO_ROOT"

echo "Logging into ACR..."
az acr login --name "$CONTAINER_REGISTRY_NAME"

echo "Tagging image as $ACR_LOGIN_SERVER/$IMAGE_NAME"
docker tag "$IMAGE_NAME" "$ACR_LOGIN_SERVER/$IMAGE_NAME"

echo "Pushing image to ACR..."
docker push "$ACR_LOGIN_SERVER/$IMAGE_NAME"

# Create Container App env if needed
if ! az containerapp env show --name cdc-env --resource-group "$RESOURCE_GROUP_NAME" &>/dev/null; then
  echo "Creating Container App Environment..."
  az containerapp env create --name cdc-env \
    --resource-group "$RESOURCE_GROUP_NAME" \
    --location "$LOCATION"
fi

# Deploy Container App
echo "Deploying container app..."
az containerapp create \
  --name "$CONTAINER_APP_NAME" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --environment cdc-env \
  --image "$ACR_LOGIN_SERVER/$IMAGE_NAME" \
  --registry-server "$ACR_LOGIN_SERVER" \
  --cpu 0.5 --memory 1.0Gi \
  --revisions-mode Single \
  --env-vars \
    ConnectionStrings__SyncDb="$SQL_CONNECTION_STRING" \
    ServiceBus__ConnectionString="$SERVICE_BUS_CONNECTION_STRING" \
    ServiceBus__TopicName="$SERVICE_BUS_TOPIC_NAME"

echo "✅ Done! Container App deployed: $CONTAINER_APP_NAME"
az containerapp show --name "$CONTAINER_APP_NAME" --resource-group "$RESOURCE_GROUP_NAME" --query "properties.configuration.ingress.fqdn" -o tsv

read -p "Press Enter to close..." dummy