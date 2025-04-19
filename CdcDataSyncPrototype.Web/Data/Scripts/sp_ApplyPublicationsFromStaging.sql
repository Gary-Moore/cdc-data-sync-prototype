CREATE OR ALTER PROCEDURE [dbo].[sp_ApplyPublicationsFromStaging]
AS 
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        MERGE dbo.Publications AS target
        USING(
            SELECT Id, Title, Type, PublishedDate, LastModified
            FROM dbo.Publications_Staging
            WHERE Processed = 0 AND Operation in (2, 4)
        ) AS SOURCE
        ON target.Id = source.Id
        WHEN MATCHED THEN
            UPDATE SET 
                Title = source.Title,
                Type = source.Type,
                PublishedDate = source.PublishedDate,
                LastModified = source.LastModified
        WHEN NOT MATCHED THEN
            INSERT (Id, Title, Type, PublishedDate, LastModified)
            VALUES (source.Id, source.Title, source.Type, source.PublishedDate, source.LastModified);

        -- Update the processed flag in the staging table
        UPDATE dbo.Publications_Staging
        SET Processed = 1
        WHERE Processed = 0 AND Operation in (2, 4);
    END TRY
    BEGIN CATCH
        -- Log error to each affected row (optional for granular retry tracking)
        DECLARE @ErrorMessage NVARCHAR(1000) = ERROR_MESSAGE();
        
        UPDATE dbo.Publications_Staging
        SET ErrorMessage = @ErrorMessage
        WHERE Processed = 0 AND Operation in (2, 4);

        THROW; -- Re-throw the error to be handled by the calling process
    END CATCH
END
    