USE [CdcSyncPrototypeDb];
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.DATABASES WHERE name = 'CdcSyncPrototypeDb' AND is_cdc_enabled = 1
)
BEGIN
    EXEC sys.sp_cdc_enable_db;
    PRINT '✅ CDC enabled on the database';
END
ELSE
BEGIN
    PRINT '❌ CDC is already enabled on the database';
END
GO

-- Enable CDC on the table
IF NOT EXISTS (
    SELECT * FROM cdc.change_tables WHERE source_object_id = OBJECT_ID('dbo.Publications')
)
BEGIN
    EXEC sys.sp_cdc_enable_table
        @source_schema = N'dbo',
        @source_name = N'Publications',
        @role_name = NULL,
        @supports_net_changes = 0;

    PRINT '✅ CDC enabled on the table dbo.Publications';
END
ELSE
BEGIN
    PRINT '❌ CDC is already enabled on the table dbo.Publications';
END