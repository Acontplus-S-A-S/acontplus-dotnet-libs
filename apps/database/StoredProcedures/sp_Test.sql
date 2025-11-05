USE TestDb
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE sp_Test 
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; -- Automatically rolls back transactions on errors
    
    BEGIN TRY
        -- Validate input parameters first
        --IF @IdCard IS NULL OR @IdCard = ''
        --BEGIN
            -- Using THROW with custom error number (must be >= 13000)
-- Proper way to throw custom errors in modern SQL Server
THROW 50001, 'Id Card parameter cannot be empty', 1;
--END
        
        -- Begin transaction if making data modifications
        --BEGIN TRANSACTION;
        
        -- Your actual business logic here
        --SELECT 
        --    CustomerId,
        --    CustomerName,
        --    InternalDetails
        --FROM dbo.Customers
        --WHERE IdCard = @IdCard;
        
        -- Commit if successful
        --COMMIT TRANSACTION;

		SELECT '0' AS Code, 'Test' AS 'Message'
    END TRY
    BEGIN CATCH
        -- Rollback any open transaction
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        -- Enhanced error handling with error metadata
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @ErrorProcedure NVARCHAR(126) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        
        -- Log the error (could use sp_logerror or a custom logging table)
        --EXEC dbo.LogError 
        --    @ErrorNumber = @ErrorNumber,
        --    @ErrorSeverity = @ErrorSeverity,
        --    @ErrorState = @ErrorState,
        --    @ErrorProcedure = @ErrorProcedure,
        --    @ErrorLine = @ErrorLine,
        --    @ErrorMessage = @ErrorMessage;
        
        ---- Re-throw with additional context if needed
        DECLARE @FullErrorMessage NVARCHAR(4000) = 
            CONCAT('Procedure: ', @ErrorProcedure, 
                   ' | Line: ', @ErrorLine,
                   ' | Error: ', @ErrorMessage);
        
        --THROW @ErrorNumber, @FullErrorMessage, @ErrorState;
		THROW;
    END CATCH
END
GO