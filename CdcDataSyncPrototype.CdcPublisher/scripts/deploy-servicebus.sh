#!/bin/bash
set -e

# This script provisions an Azure Service Bus namespace and topic for CDC message publishing

# Load environment variables from .env file
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

if [[ -f "$SCRIPT_DIR/.env" ]]; then
  source "$SCRIPT_DIR/.env"
fi

# Defaults
: "${RESOURCE_GROUP_NAME:=cdc-sync-demo-rg}"
: "${SERVICE_BUS_NAMESPACE:=cdc-demo-sbns}"
: "${SERVICE_BUS_TOPIC_NAME:=cdc-updates}"
: "${LOCATION:=uksouth}"

# Access policy name (RootManageSharedAccessKey is default for connection string)
POLICY_NAME="RootManageSharedAccessKey"

echo "Creating resource group (if needed)..."
az group create --name "$RESOURCE_GROUP_NAME" --location "$LOCATION"

echo "Creating Service Bus Namespace: $SERVICE_BUS_NAMESPACE"
az servicebus namespace create \
  --name "$SERVICE_BUS_NAMESPACE" \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --location "$LOCATION" \
  --sku Standard

echo "Creating Topic: $SERVICE_BUS_TOPIC_NAME"
az servicebus topic create \
  --name "$SERVICE_BUS_TOPIC_NAME" \
  --namespace-name "$SERVICE_BUS_NAMESPACE" \
  --resource-group "$RESOURCE_GROUP_NAME"

echo "Creating subscription to topic..."
az servicebus topic subscription create \
  --name "${SERVICE_BUS_SUBSCRIPTION_NAME:=cdc-receiver}" \
  --topic-name "$SERVICE_BUS_TOPIC_NAME" \
  --namespace-name "$SERVICE_BUS_NAMESPACE" \
  --resource-group "$RESOURCE_GROUP_NAME"


echo "Fetching connection string for topic publisher..."
SERVICE_BUS_CONNECTION_STRING=$(az servicebus namespace authorization-rule keys list \
  --resource-group "$RESOURCE_GROUP_NAME" \
  --namespace-name "$SERVICE_BUS_NAMESPACE" \
  --name "$POLICY_NAME" \
  --query primaryConnectionString -o tsv)

echo "✅ Service Bus setup complete"
echo "Connection string (store this in your .env file):"
echo ""
echo "SERVICE_BUS_CONNECTION_STRING=\"$SERVICE_BUS_CONNECTION_STRING\""
echo "SERVICE_BUS_TOPIC_NAME=$SERVICE_BUS_TOPIC_NAME"

echo " Don't forget to update your .env file!"
read -p "Press Enter to close..." dummy
