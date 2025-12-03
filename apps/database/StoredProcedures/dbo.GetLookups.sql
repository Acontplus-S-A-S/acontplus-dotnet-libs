-- =============================================
-- Author:      Acontplus Team
-- Create date: 2024-12-03
-- Description: Get lookup/reference data for dropdowns and select boxes
-- Usage:       EXEC dbo.GetLookups @Filters = '{"module":"test","context":"general"}'
-- Note:        Uses JSON @Filters parameter (default ADO repository behavior with UseJsonFilters=true)
-- =============================================
CREATE OR ALTER PROCEDURE [dbo].[GetLookups]
    @Filters NVARCHAR(MAX) = NULL,
    @SearchTerm NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Parse JSON filters
    DECLARE @module NVARCHAR(100);
    DECLARE @context NVARCHAR(100);
    DECLARE @category NVARCHAR(100);
    DECLARE @includeInactive BIT;

    IF @Filters IS NOT NULL
    BEGIN
        SELECT
            @module = JSON_VALUE(@Filters, '$.module'),
            @context = JSON_VALUE(@Filters, '$.context'),
            @category = JSON_VALUE(@Filters, '$.category'),
            @includeInactive = CAST(JSON_VALUE(@Filters, '$.includeInactive') AS BIT);
    END

    -- Set defaults if not provided
    SET @module = ISNULL(@module, 'test');
    SET @context = ISNULL(@context, 'general');
    SET @includeInactive = ISNULL(@includeInactive, 0);

    -- Example: User Status Lookups
    SELECT
        'UserStatuses' AS TableName,
        1 AS Id,
        'ACTIVE' AS Code,
        'Active' AS [Value],
        1 AS DisplayOrder,
        NULL AS ParentId,
        CAST(1 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'User is active and can access the system' AS Description,
        NULL AS Metadata

    UNION ALL

    SELECT
        'UserStatuses' AS TableName,
        2 AS Id,
        'INACTIVE' AS Code,
        'Inactive' AS [Value],
        2 AS DisplayOrder,
        NULL AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'User is inactive and cannot access the system' AS Description,
        NULL AS Metadata

    UNION ALL

    SELECT
        'UserStatuses' AS TableName,
        3 AS Id,
        'SUSPENDED' AS Code,
        'Suspended' AS [Value],
        3 AS DisplayOrder,
        NULL AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'User is temporarily suspended' AS Description,
        NULL AS Metadata

    UNION ALL

    -- Example: User Roles Lookups
    SELECT
        'UserRoles' AS TableName,
        1 AS Id,
        'ADMIN' AS Code,
        'Administrator' AS [Value],
        1 AS DisplayOrder,
        NULL AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'Full system access' AS Description,
        '{"icon":"👑","color":"#FF5722"}' AS Metadata

    UNION ALL

    SELECT
        'UserRoles' AS TableName,
        2 AS Id,
        'USER' AS Code,
        'Regular User' AS [Value],
        2 AS DisplayOrder,
        NULL AS ParentId,
        CAST(1 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'Standard user access' AS Description,
        '{"icon":"👤","color":"#2196F3"}' AS Metadata

    UNION ALL

    SELECT
        'UserRoles' AS TableName,
        3 AS Id,
        'GUEST' AS Code,
        'Guest' AS [Value],
        3 AS DisplayOrder,
        NULL AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'Limited guest access' AS Description,
        '{"icon":"🔓","color":"#9E9E9E"}' AS Metadata

    UNION ALL

    -- Example: Countries (for hierarchical demo)
    SELECT
        'Countries' AS TableName,
        1 AS Id,
        'US' AS Code,
        'United States' AS [Value],
        1 AS DisplayOrder,
        NULL AS ParentId,
        CAST(1 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'United States of America' AS Description,
        '{"flag":"🇺🇸","code":"USA"}' AS Metadata

    UNION ALL

    SELECT
        'Countries' AS TableName,
        2 AS Id,
        'EC' AS Code,
        'Ecuador' AS [Value],
        2 AS DisplayOrder,
        NULL AS ParentId,
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'Republic of Ecuador' AS Description,
        '{"flag":"🇪🇨","code":"ECU"}' AS Metadata

    UNION ALL

    -- Example: States/Provinces (hierarchical - children of countries)
    SELECT
        'States' AS TableName,
        1 AS Id,
        'CA' AS Code,
        'California' AS [Value],
        1 AS DisplayOrder,
        1 AS ParentId, -- Parent is United States
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'State of California' AS Description,
        NULL AS Metadata

    UNION ALL

    SELECT
        'States' AS TableName,
        2 AS Id,
        'NY' AS Code,
        'New York' AS [Value],
        2 AS DisplayOrder,
        1 AS ParentId, -- Parent is United States
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'State of New York' AS Description,
        NULL AS Metadata

    UNION ALL

    SELECT
        'States' AS TableName,
        3 AS Id,
        'PICH' AS Code,
        'Pichincha' AS [Value],
        1 AS DisplayOrder,
        2 AS ParentId, -- Parent is Ecuador
        CAST(0 AS BIT) AS IsDefault,
        CAST(1 AS BIT) AS IsActive,
        'Province of Pichincha' AS Description,
        NULL AS Metadata

    ORDER BY TableName, DisplayOrder;

    -- Note: In a real implementation, you would filter based on @module, @context, @SearchTerm, etc.
    -- Example:
    -- WHERE (@SearchTerm IS NULL OR [Value] LIKE @SearchTerm)
    --   AND (@includeInactive = 1 OR IsActive = 1)
    --   AND (@category IS NULL OR Category = @category)
    --   AND (@module IS NULL OR Module = @module)
    --   AND (@context IS NULL OR Context = @context)
END
GO
