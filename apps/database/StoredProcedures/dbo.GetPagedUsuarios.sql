USE TestDb
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:      Acontplus Team
-- Create date: 2025-11-05
-- Description: High-performance paginated query for Usuario table
--              Supports filtering via JSON filters, sorting, and search
--              Returns both data and total count for pagination metadata
-- Usage:
--   EXEC dbo.GetPagedUsuarios
--     @PageIndex = 1,
--     @PageSize = 10,
--     @Filters = '{"UserId":"123","EmailDomain":"example.com"}',
--     @SortBy = 'CreatedAt',
--     @SortDirection = 'DESC',
--     @SearchTerm = 'john'
-- =============================================
CREATE OR ALTER PROCEDURE dbo.GetPagedUsuarios
    @PageIndex INT = 1,
    @PageSize INT = 10,
    @SortBy NVARCHAR(50) = 'CreatedAt',
    @SortDirection NVARCHAR(4) = 'DESC',
    @SearchTerm NVARCHAR(100) = NULL,
    @Filters NVARCHAR(MAX) = NULL,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        -- Validate input parameters
        IF @PageIndex < 1
        BEGIN
            THROW 50001, 'PageIndex must be greater than or equal to 1', 1;
        END

        IF @PageSize < 1 OR @PageSize > 1000
        BEGIN
            THROW 50002, 'PageSize must be between 1 and 1000', 1;
        END

        -- Validate and sanitize SortBy to prevent SQL injection
        IF @SortBy NOT IN ('Id', 'Username', 'Email', 'CreatedAt', 'UpdatedAt')
        BEGIN
            SET @SortBy = 'CreatedAt'; -- Default to safe column
        END

        -- Validate SortDirection
        IF @SortDirection NOT IN ('ASC', 'DESC')
        BEGIN
            SET @SortDirection = 'DESC'; -- Default to descending
        END

        -- Calculate offset
        DECLARE @Offset INT = (@PageIndex - 1) * @PageSize;

        -- Parse JSON filters
        DECLARE @UserId NVARCHAR(100) = NULL;
        DECLARE @EmailDomain NVARCHAR(100) = NULL;
        DECLARE @IncludeDeleted BIT = 0; -- Default to exclude deleted

        IF @Filters IS NOT NULL AND ISJSON(@Filters) = 1
        BEGIN
            SELECT
                @UserId = JSON_VALUE(@Filters, '$.UserId'),
                @EmailDomain = JSON_VALUE(@Filters, '$.EmailDomain'),
                @IncludeDeleted = ISNULL(TRY_CAST(JSON_VALUE(@Filters, '$.IncludeDeleted') AS BIT), 0);
        END

        -- Build WHERE clause dynamically
        DECLARE @WhereClause NVARCHAR(MAX) = N'WHERE 1=1';

        -- Filter by IsDeleted
        IF @IncludeDeleted = 0
        BEGIN
            SET @WhereClause = @WhereClause + N' AND IsDeleted = 0';
        END

        -- Filter by UserId
        IF @UserId IS NOT NULL AND LEN(@UserId) > 0
        BEGIN
            SET @WhereClause = @WhereClause + N' AND Id = @UserIdParam';
        END

        -- Filter by EmailDomain
        IF @EmailDomain IS NOT NULL AND LEN(@EmailDomain) > 0
        BEGIN
            SET @WhereClause = @WhereClause + N' AND Email LIKE N''%'' + @EmailDomainParam + N''%''';
        END

        -- Filter by SearchTerm (searches in Username and Email)
        IF @SearchTerm IS NOT NULL AND LEN(@SearchTerm) > 0
        BEGIN
            SET @WhereClause = @WhereClause + N' AND (Username LIKE N''%'' + @SearchTermParam + N''%'' OR Email LIKE N''%'' + @SearchTermParam + N''%'')';
        END

        -- Build ORDER BY clause
        DECLARE @OrderByClause NVARCHAR(100) =
            CASE @SortBy
                WHEN 'Id' THEN N'ORDER BY Id ' + @SortDirection
                WHEN 'Username' THEN N'ORDER BY Username ' + @SortDirection
                WHEN 'Email' THEN N'ORDER BY Email ' + @SortDirection
                WHEN 'CreatedAt' THEN N'ORDER BY CreatedAt ' + @SortDirection
                WHEN 'UpdatedAt' THEN N'ORDER BY UpdatedAt ' + @SortDirection
                ELSE N'ORDER BY CreatedAt DESC'
            END;

        -- Build count query
        DECLARE @CountSQL NVARCHAR(MAX) = N'
            SELECT @TotalCountOut = COUNT(*)
            FROM dbo.Usuarios ' + @WhereClause;

        -- Build data query with pagination
        DECLARE @DataSQL NVARCHAR(MAX) = N'
            SELECT
                Id,
                Username,
                Email,
                CreatedAt,
                UpdatedAt,
                IsDeleted
            FROM dbo.Usuarios ' + @WhereClause + N' ' + @OrderByClause + N'
            OFFSET @OffsetParam ROWS
            FETCH NEXT @PageSizeParam ROWS ONLY';

        -- Declare local variable for total count
        DECLARE @LocalTotalCount INT;

        -- Execute count query
        EXEC sp_executesql
            @CountSQL,
            N'@UserIdParam NVARCHAR(100), @EmailDomainParam NVARCHAR(100), @SearchTermParam NVARCHAR(100), @TotalCountOut INT OUTPUT',
            @UserIdParam = @UserId,
            @EmailDomainParam = @EmailDomain,
            @SearchTermParam = @SearchTerm,
            @TotalCountOut = @LocalTotalCount OUTPUT;

        -- Set output parameter
        SET @TotalCount = @LocalTotalCount;

        -- Execute data query (returns the result set)
        EXEC sp_executesql
            @DataSQL,
            N'@UserIdParam NVARCHAR(100), @EmailDomainParam NVARCHAR(100), @SearchTermParam NVARCHAR(100), @OffsetParam INT, @PageSizeParam INT',
            @UserIdParam = @UserId,
            @EmailDomainParam = @EmailDomain,
            @SearchTermParam = @SearchTerm,
            @OffsetParam = @Offset,
            @PageSizeParam = @PageSize;

    END TRY
    BEGIN CATCH
        -- Enhanced error handling
        DECLARE @ErrorNumber INT = ERROR_NUMBER();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        DECLARE @ErrorProcedure NVARCHAR(126) = ERROR_PROCEDURE();
        DECLARE @ErrorLine INT = ERROR_LINE();
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();

        -- Build comprehensive error message
        DECLARE @FullErrorMessage NVARCHAR(4000) =
            CONCAT('Stored Procedure: ', @ErrorProcedure,
                   ' | Line: ', @ErrorLine,
                   ' | Error: ', @ErrorMessage,
                   ' | Parameters: PageIndex=', @PageIndex,
                   ', PageSize=', @PageSize,
                   ', Filters=', ISNULL(@Filters, 'NULL'),
                   ', SortBy=', @SortBy,
                   ', SortDirection=', @SortDirection);

        -- Log error (in production, log to error table)
        RAISERROR(@FullErrorMessage, @ErrorSeverity, @ErrorState);

        -- Re-throw original error
        THROW;
    END CATCH
END
GO

-- =============================================
-- Test Examples
-- =============================================

-- Example 1: Basic pagination (first page, 10 records)
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios @PageIndex = 1, @PageSize = 10, @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 2: Filter by UserId using JSON
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios
--     @PageIndex = 1,
--     @PageSize = 10,
--     @Filters = '{"UserId":"123"}',
--     @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 3: Filter by EmailDomain using JSON
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios
--     @PageIndex = 1,
--     @PageSize = 10,
--     @Filters = '{"EmailDomain":"example.com"}',
--     @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 4: Multiple filters using JSON
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios
--     @PageIndex = 1,
--     @PageSize = 10,
--     @Filters = '{"UserId":"123","EmailDomain":"gmail.com"}',
--     @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 5: Search by term
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios @PageIndex = 1, @PageSize = 10, @SearchTerm = 'john', @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 6: Custom sorting
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios @PageIndex = 1, @PageSize = 10, @SortBy = 'Username', @SortDirection = 'ASC', @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 7: Include deleted users (using Filters JSON)
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios
--     @PageIndex = 1,
--     @PageSize = 10,
--     @Filters = '{"IncludeDeleted":true}',
--     @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 8: Combined filters with JSON, search and sorting
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios
--     @PageIndex = 2,
--     @PageSize = 20,
--     @Filters = '{"UserId":"123","EmailDomain":"gmail.com"}',
--     @SearchTerm = 'test',
--     @SortBy = 'Email',
--     @SortDirection = 'ASC',
--     @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;

-- Example 9: Complex filters with various data types (string, int, bool, date)
-- DECLARE @TotalCount INT;
-- EXEC dbo.GetPagedUsuarios
--     @PageIndex = 1,
--     @PageSize = 10,
--     @Filters = '{"UserId":"123","IncludeDeleted":true,"EmailDomain":"example.com"}',
--     @TotalCount = @TotalCount OUTPUT;
-- SELECT @TotalCount AS TotalCount;
