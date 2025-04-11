param (
    [string]$Server = "localhost",
    [string]$Database = "CdcSyncPrototypeDb",
    [string]$User = "",
    [string]$Password = ""
)

Write-Host "⚙️  Setting up CDC on $Database..."

# Construct log file path
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$logDir = ".\Database\logs"
$logPath = "$logDir\cdc-setup-$timestamp.log"
New-Item -ItemType Directory -Path $logDir -Force | Out-Null

# Construct sqlcmd arguments safely
if ($User -and $Password) {
    $authArgs = "-U `"$User`" -P `"$Password`""
} else {
    $authArgs = "-E"
}

$sqlcmdArgs = "-S `"$Server`" -d `"$Database`" $authArgs -i .\Database\enable-cdc.sql -C"
Write-Host "📤 Running sqlcmd with args: $sqlcmdArgs"

# Run command and tee output to log
Invoke-Expression "sqlcmd $sqlcmdArgs 2>&1 | Tee-Object -FilePath $logPath"

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ CDC setup completed."
} else {
    Write-Error "❌ CDC setup failed. Check log file for details: $logPath"
}
