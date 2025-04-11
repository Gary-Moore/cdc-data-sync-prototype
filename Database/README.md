# 🗄️ Database Setup Scripts

This folder contains SQL scripts for enabling **Change Data Capture (CDC)** and preparing the database for the `CdcDataSyncPrototype` solution.

These scripts are used during development and testing to simulate an internal Line-of-Business system that emits data changes via SQL Server CDC.

---

## 📜 Scripts Included

| File              | Purpose                                                                 |
|-------------------|--------------------------------------------------------------------------|
| `EnableCdc.sql`   | Enables CDC on the `CdcSyncPrototypeDb` database and the `Publications` table. |
| `setup-db.ps1`    | PowerShell helper script that runs the setup automatically via `sqlcmd`. |

---

## 🚀 Quick Start (Recommended)

Use the helper script to set up the database in one step:

```powershell
.\setup-db.ps1
```
You’ll see confirmation messages in the terminal once it completes.

---

## 🛠 Manual Usage (if preferred)

You can also run the SQL script manually using sqlcmd:

```bash
sqlcmd -S localhost -d CdcSyncPrototypeDb -i .\Database\EnableCdc.sql
```

> ⚠️ Ensure the database exists before running this.
⚠️ SQL Server Agent must be running for CDC to work.

## ⚠️ Requirements

* SQL Server 2016+ with CDC support

* SQL Server Agent running

* A Publications table with a primary key

* sqlcmd installed (comes with SSMS or can be added via command line tools)

### Notes

* This setup is intended for local development and testing only.

* If you need to enable CDC on additional tables, duplicate and modify EnableCdc.sql accordingly.

* In production scenarios, consider using Idempotent migrations or IaC tools to manage database state.