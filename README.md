# 🧬 CDC Data Sync Prototype

This is a full-stack .NET 8 prototype demonstrating how to sync SQL Server data changes using **Change Data Capture (CDC)** and modern .NET technologies. It's a clean, event-driven architecture designed to replace legacy “shove XML in a queue” nightmares with something robust, testable, and just a little bit beautiful.

---

## 🧱 Architecture

This repo contains:

| Project                     | Purpose                                                                 |
|-----------------------------|-------------------------------------------------------------------------|
| `BusinessApp`               | ASP.NET Core MVC app simulating an internal Line-of-Business system     |
| `CdcPublisher`              | .NET Worker Service that reads CDC changes and publishes to a queue     |
| `Database`                  | SQL scripts and PowerShell helper to enable CDC and prepare the schema  |

---

## 🚀 Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/Gary-Moore/cdc-data-sync-prototype.git
cd cdc-data-sync-prototype
```

### 2. Set up the database (SQL Server LocalDB)

> Ensure you have LocalDB or full SQL Server with CDC support.

```powershell
cd Database
.\setup-db.ps1
```

This enables CDC on the `Publications` table and creates a tracking checkpoint.

### 3. Run the apps

```bash
# Run the MVC business app
dotnet run --project CdcDataSyncPrototype.BusinessApp

# In another terminal, run the CDC publisher worker
dotnet run --project CdcDataSyncPrototype.CdcPublisher
```

---

## 🧠 Features

- ✅ SQL Server **Change Data Capture (CDC)** configured via scripts
- ✅ Entity Framework Core 8 with clean data layer setup
- ✅ MVC CRUD app simulating real business edits
- ✅ Background Worker with `IHostedService` and cancellation handling
- ✅ LSN (Log Sequence Number) tracking to ensure reliable sync resume
- 🛠️ Ready for integration with message queues (e.g., Azure Service Bus)

---

## 📦 Planned Enhancements

- 📬 Queue publisher implementation (Service Bus or RabbitMQ)
- 🧪 Test project with integration tests
- 📊 Dashboard to monitor sync health
- 📁 Use Backstage or GitHub templates to scaffold this setup

---

## 📄 License

MIT
