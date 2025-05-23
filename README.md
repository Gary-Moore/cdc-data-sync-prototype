#  CDC Data Sync Prototype

This is a full-stack .NET 8 prototype demonstrating how to sync SQL Server data changes using **Change Data Capture (CDC)** and modern .NET technologies. It’s a clean, event-driven architecture designed to replace legacy “shove XML in a queue” nightmares with something robust, testable, and just a little bit beautiful.

---

## Architecture

This repo contains:


| Project                         | Purpose                                                                 |
|--------------------------------|-------------------------------------------------------------------------|
| `BusinessApp`                  | ASP.NET Core MVC app simulating an internal Line-of-Business system     |
| `CdcPublisher`                 | .NET Worker Service that reads CDC changes and publishes to a queue     |
| `CdcReceiver`                  | .NET Worker Service that processes messages from the queue and updates a staging table |
| `CdcMockReceiver`             | Console app that logs queue messages — for local testing                |
| `Database`                     | SQL scripts for CDC setup (optional — EF migrations now handle schema)  |
| `webapp` | ASP.NET Core MVC app simulating an public website      |

---

## 🔁 Sync Eligibility Logic

The `BusinessApp` simulates a real-world system that controls which records are eligible for syncing.

A record will be **included in sync** if:

- `InternalOnly = false`
- `PublishStartDate <= DateTime.UtcNow`
- `PublishEndDate = null` or `> DateTime.UtcNow`

Otherwise, it is **suppressed**, and a badge is shown in the UI explaining why.

🌀 The `Republish` action updates `LastModified` to explicitly re-send a record through the pipeline.

---

## 🚀 Getting Started (Local Dev)

### 1. Clone the repo

```bash
git clone https://github.com/Gary-Moore/cdc-data-sync-prototype.git
cd cdc-data-sync-prototype
```

### 2. Configure secrets

Use [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for local dev:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your-sql-connection>"
dotnet user-secrets set "ServiceBus:ConnectionString" "<your-service-bus-connection>"
dotnet user-secrets set "ServiceBus:TopicName" "cdc-updates"
```

### 3. Apply migrations (optional)

If running locally against SQL Server:

```bash
dotnet ef database update --project CdcDataSyncPrototype.BusinessApp
```

### 4. Run the apps

```bash
dotnet run --project CdcDataSyncPrototype.BusinessApp     # MVC App
dotnet run --project CdcDataSyncPrototype.CdcPublisher    # CDC Worker
dotnet run --project CdcMockReceiver                      # Optional queue subscriber
```

---

## ☁️ Azure One-Click Deployment (Recommended)

### 📦 Run this command:

```bash
./scripts/deploy-businessapp.sh
```

This script:

- ✅ Creates Resource Group, App Service Plan, and Web App (Windows)
- ✅ Creates an Azure SQL DB (Serverless Tier) and applies EF Core migrations
- ✅ Adds required firewall rules
- ✅ Sets secrets and connection strings into App Settings
- ✅ Publishes the Business App
- ✅ Uses `.env` file for all sensitive values

> Requires: [Azure CLI](https://learn.microsoft.com/en-us/cli/azure/install-azure-cli) and [PowerShell Core](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell) (for zipping on Windows)

---

## Features

- ✅ SQL Server **Change Data Capture (CDC)** integration
- ✅ Entity Framework Core 8 migrations
- ✅ ASP.NET Core MVC UI with sync badges and republish support
- ✅ Suppression rules engine + audit trail per record
- ✅ CDC Publisher service reads and publishes deltas to Service Bus
- ✅ Receiver service listens to the queue, updates staging, and applies deltas
- ✅ Optional mock receiver logs messages locally

---

## Sync Visibility

The BusinessApp offers:

- 🟢 Sync preview logic
- ❌ Suppression feedback
- 🕓 Audit logs with rule outcomes
- 🔁 One-click republish option

## 📄 .env File Template (for deployment)

```ini
# .env

# Azure Resource Info
RESOURCE_GROUP_NAME=cdc-sync-demo-rg
LOCATION=uksouth

# App Service + SQL
SQL_ADMIN_PASSWORD=YourStrongP@ssword123
SERVICE_BUS_CONNECTION_STRING=Endpoint=sb://<your-bus>.servicebus.windows.net/... 
```

---

## 📄 License

MIT

