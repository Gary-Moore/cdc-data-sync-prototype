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
sqlcmd -S localhost -d CdcSyncPrototypeDb -i .\Database\enable-cdc.sql
```

> * Ensure the database exists before running this.
> * SQL Server Agent must be running for CDC to work.

---

## 🔐 Authenticated Setup Usage (Updated)

You can now pass credentials to the setup script to avoid using your personal login or `sa`:

```powershell
# Using SQL Authentication
.\setup-db.ps1 -User "cdc_admin" -Password "Str0ngP@ssword!"

# Using Windows Authentication (default)
.\setup-db.ps1
```

### Parameters

| Name      | Description                                | Default            |
|-----------|--------------------------------------------|--------------------|
| `-User`   | SQL login username                         | (Windows auth)     |
| `-Password` | SQL login password                      | (Windows auth)     |


---

## 💡 Tip

Make sure the `cdc_admin` SQL login:
- Exists in the SQL instance
- Has access to the `CdcSyncPrototypeDb` database
- Is a member of `db_owner` (minimum required to enable CDC)

SQL to create this login:

```sql
USE master;
GO
CREATE LOGIN cdc_admin WITH PASSWORD = 'Str0ngP@ssword!';
GO
USE CdcSyncPrototypeDb;
GO
CREATE USER cdc_admin FOR LOGIN cdc_admin;
EXEC sp_addrolemember N'db_owner', N'cdc_admin';
```

---

## ⚠️ Requirements

* SQL Server 2016+ with CDC support
* SQL Server Agent running
* A `Publications` table with a primary key
* `sqlcmd` installed (included with SSMS or via CLI tools)

### Notes

* This setup is intended for local development and testing only.
* If you need to enable CDC on additional tables, duplicate and modify `enable-cdc.sql` accordingly.
* Learn more: [CDC in SQL Server](https://learn.microsoft.com/en-us/sql/relational-databases/track-changes/about-change-data-capture-sql-server)

