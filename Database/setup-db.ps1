# setup-db.ps1
Write-Host "⚙️  Setting up CDC on CdcSyncPrototypeDb..."
sqlcmd -S localhost -d CdcSyncPrototypeDb -i .\Database\enable-cdc.sql

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ CDC setup completed."
} else {
    Write-Error "❌ CDC setup failed. Check for errors above."
}
