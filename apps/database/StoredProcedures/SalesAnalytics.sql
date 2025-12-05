-- =============================================
-- Sales Analytics Stored Procedures
-- Demonstrates Acontplus.Analytics package usage
-- =============================================

-- =============================================
-- 1. Dashboard Statistics
-- Returns comprehensive KPIs for sales dashboard
-- =============================================
CREATE OR ALTER PROCEDURE [Sales].[GetDashboardStats]
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Default to last 30 days if not provided
    SET @StartDate = ISNULL(@StartDate, DATEADD(DAY, -30, GETUTCDATE()));
    SET @EndDate = ISNULL(@EndDate, GETUTCDATE());

    -- Calculate previous period for comparison
    DECLARE @PeriodDays INT = DATEDIFF(DAY, @StartDate, @EndDate);
    DECLARE @PrevStartDate DATETIME2 = DATEADD(DAY, -@PeriodDays - 1, @StartDate);
    DECLARE @PrevEndDate DATETIME2 = DATEADD(DAY, -1, @StartDate);

    -- Current period metrics
    DECLARE @CurrentRevenue DECIMAL(18,2), @PreviousRevenue DECIMAL(18,2);

    SELECT
        -- Transaction Metrics (Base DTO properties)
        @CurrentRevenue = SUM(TotalAmount),
        @TotalTransactions = COUNT(*),
        @CompletedTransactions = SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END),
        @TotalRevenue = SUM(TotalAmount),
        @NetRevenue = SUM(NetAmount),
        @TotalTax = SUM(TaxAmount),
        @AverageTransactionValue = AVG(TotalAmount),
        @UniqueCustomers = COUNT(DISTINCT CustomerId),

        -- Sales-specific metrics
        @AverageOrderValue = AVG(TotalAmount),
        @TotalDiscounts = SUM(DiscountAmount),
        @CancelledOrders = SUM(CASE WHEN Status = 'Cancelled' THEN 1 ELSE 0 END),

        -- Payment method breakdown
        @CashSales = SUM(CASE WHEN PaymentMethod = 'Cash' THEN TotalAmount ELSE 0 END),
        @CreditCardSales = SUM(CASE WHEN PaymentMethod = 'CreditCard' THEN TotalAmount ELSE 0 END),
        @TransferSales = SUM(CASE WHEN PaymentMethod = 'Transfer' THEN TotalAmount ELSE 0 END)
    FROM Sales
    WHERE SaleDate BETWEEN @StartDate AND @EndDate;

    -- Previous period revenue for growth calculation
    SELECT @PreviousRevenue = SUM(TotalAmount)
    FROM Sales
    WHERE SaleDate BETWEEN @PrevStartDate AND @PrevEndDate;

    -- Calculate growth rate
    DECLARE @GrowthRate DECIMAL(18,2) =
        CASE
            WHEN @PreviousRevenue > 0 THEN ((@CurrentRevenue - @PreviousRevenue) / @PreviousRevenue) * 100
            ELSE 0
        END;

    -- Calculate derived metrics
    DECLARE @CompletionRate DECIMAL(18,2) =
        CASE
            WHEN @TotalTransactions > 0 THEN (@CompletedTransactions * 100.0 / @TotalTransactions)
            ELSE 0
        END;

    DECLARE @DiscountPercentage DECIMAL(18,2) =
        CASE
            WHEN @TotalRevenue > 0 THEN (@TotalDiscounts * 100.0 / @TotalRevenue)
            ELSE 0
        END;

    DECLARE @CancellationRate DECIMAL(18,2) =
        CASE
            WHEN @TotalTransactions > 0 THEN (@CancelledOrders * 100.0 / @TotalTransactions)
            ELSE 0
        END;

    -- Customer metrics
    DECLARE @NewCustomers INT, @ReturningCustomers INT;

    SELECT
        @NewCustomers = COUNT(DISTINCT CustomerId)
    FROM Sales
    WHERE SaleDate BETWEEN @StartDate AND @EndDate
    AND CustomerId NOT IN (
        SELECT DISTINCT CustomerId
        FROM Sales
        WHERE SaleDate < @StartDate
    );

    SET @ReturningCustomers = @UniqueCustomers - @NewCustomers;

    DECLARE @CustomerRetentionRate DECIMAL(18,2) =
        CASE
            WHEN @UniqueCustomers > 0 THEN (@ReturningCustomers * 100.0 / @UniqueCustomers)
            ELSE 0
        END;

    -- Return results
    SELECT
        -- Base DTO properties
        @TotalTransactions AS TotalTransactions,
        @CompletedTransactions AS CompletedTransactions,
        @TotalRevenue AS TotalRevenue,
        @NetRevenue AS NetRevenue,
        @TotalTax AS TotalTax,
        @GrowthRate AS GrowthRate,
        @CompletionRate AS CompletionRate,
        @UniqueCustomers AS UniqueEntities,
        @AverageTransactionValue AS AverageTransactionValue,

        -- Sales-specific properties
        @AverageOrderValue AS AverageOrderValue,
        @TotalDiscounts AS TotalDiscounts,
        @DiscountPercentage AS DiscountPercentage,
        @CancelledOrders AS CancelledOrders,
        @CancellationRate AS CancellationRate,
        @CashSales AS CashSales,
        @CreditCardSales AS CreditCardSales,
        @TransferSales AS TransferSales,
        @NewCustomers AS NewCustomers,
        @ReturningCustomers AS ReturningCustomers,
        @CustomerRetentionRate AS CustomerRetentionRate;
END
GO

-- =============================================
-- 2. Real-Time Statistics
-- Returns current operational metrics
-- =============================================
CREATE OR ALTER PROCEDURE [Sales].[GetRealTimeStats]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentHour DATETIME2 = DATEADD(HOUR, DATEDIFF(HOUR, 0, GETUTCDATE()), 0);
    DECLARE @CurrentDay DATETIME2 = CAST(GETUTCDATE() AS DATE);

    SELECT
        -- Base real-time properties
        ActiveSales = COUNT(CASE WHEN Status = 'Pending' THEN 1 END),

        -- Sales-specific real-time
        PendingPayments = COUNT(CASE WHEN Status = 'Pending' THEN 1 END),
        CurrentHourRevenue = SUM(CASE WHEN SaleDate >= @CurrentHour THEN TotalAmount ELSE 0 END),
        CurrentDayRevenue = SUM(CASE WHEN SaleDate >= @CurrentDay THEN TotalAmount ELSE 0 END),
        AverageSaleTime = AVG(DATEDIFF(MINUTE, CreatedAt, UpdatedAt)),
        SalesPerHour = COUNT(CASE WHEN SaleDate >= @CurrentHour THEN 1 END),
        PeakHourRevenue = (
            SELECT TOP 1 SUM(TotalAmount)
            FROM Sales
            WHERE SaleDate >= @CurrentDay
            GROUP BY DATEPART(HOUR, SaleDate)
            ORDER BY SUM(TotalAmount) DESC
        )
    FROM Sales
    WHERE SaleDate >= @CurrentDay;
END
GO

-- =============================================
-- 3. Aggregated Statistics
-- Returns time-series data grouped by period
-- =============================================
CREATE OR ALTER PROCEDURE [Sales].[GetAggregatedStats]
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @GroupBy NVARCHAR(10) = 'day' -- hour, day, week, month, year
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Period = CASE @GroupBy
            WHEN 'hour' THEN DATEADD(HOUR, DATEDIFF(HOUR, 0, SaleDate), 0)
            WHEN 'day' THEN CAST(SaleDate AS DATE)
            WHEN 'week' THEN DATEADD(DAY, -(DATEPART(WEEKDAY, SaleDate) - 1), CAST(SaleDate AS DATE))
            WHEN 'month' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, SaleDate), 0)
            WHEN 'year' THEN DATEADD(YEAR, DATEDIFF(YEAR, 0, SaleDate), 0)
        END,
        [Value] = SUM(TotalAmount),
        MinValue = MIN(TotalAmount),
        MaxValue = MAX(TotalAmount),
        AvgValue = AVG(TotalAmount),
        [Count] = COUNT(*),
        TotalQuantity = SUM(ItemCount),
        Percentile25 = PERCENTILE_CONT(0.25) WITHIN GROUP (ORDER BY TotalAmount) OVER(),
        Percentile50 = PERCENTILE_CONT(0.50) WITHIN GROUP (ORDER BY TotalAmount) OVER(),
        Percentile75 = PERCENTILE_CONT(0.75) WITHIN GROUP (ORDER BY TotalAmount) OVER()
    FROM Sales
    WHERE SaleDate BETWEEN @StartDate AND @EndDate
    GROUP BY CASE @GroupBy
        WHEN 'hour' THEN DATEADD(HOUR, DATEDIFF(HOUR, 0, SaleDate), 0)
        WHEN 'day' THEN CAST(SaleDate AS DATE)
        WHEN 'week' THEN DATEADD(DAY, -(DATEPART(WEEKDAY, SaleDate) - 1), CAST(SaleDate AS DATE))
        WHEN 'month' THEN DATEADD(MONTH, DATEDIFF(MONTH, 0, SaleDate), 0)
        WHEN 'year' THEN DATEADD(YEAR, DATEDIFF(YEAR, 0, SaleDate), 0)
    END
    ORDER BY Period;
END
GO

-- =============================================
-- 4. Trend Analysis
-- Returns trend data with moving averages and comparisons
-- =============================================
CREATE OR ALTER PROCEDURE [Sales].[GetTrendStats]
    @StartDate DATETIME2,
    @EndDate DATETIME2,
    @Metric NVARCHAR(50) = 'revenue' -- revenue, orders, customers, aov
AS
BEGIN
    SET NOCOUNT ON;

    WITH DailySales AS (
        SELECT
            [Date] = CAST(SaleDate AS DATE),
            [Value] = CASE @Metric
                WHEN 'revenue' THEN SUM(TotalAmount)
                WHEN 'orders' THEN COUNT(*)
                WHEN 'customers' THEN COUNT(DISTINCT CustomerId)
                WHEN 'aov' THEN AVG(TotalAmount)
                ELSE SUM(TotalAmount)
            END
        FROM Sales
        WHERE SaleDate BETWEEN @StartDate AND @EndDate
        GROUP BY CAST(SaleDate AS DATE)
    )
    SELECT
        [Date],
        [Value],
        MovingAverage7 = AVG([Value]) OVER (ORDER BY [Date] ROWS BETWEEN 6 PRECEDING AND CURRENT ROW),
        MovingAverage30 = AVG([Value]) OVER (ORDER BY [Date] ROWS BETWEEN 29 PRECEDING AND CURRENT ROW),
        SamePeriodLastYear = LAG([Value], 365) OVER (ORDER BY [Date]),
        TrendDirection = CASE
            WHEN [Value] > LAG([Value], 1) OVER (ORDER BY [Date]) THEN 'up'
            WHEN [Value] < LAG([Value], 1) OVER (ORDER BY [Date]) THEN 'down'
            ELSE 'stable'
        END,
        ChangePercent = CASE
            WHEN LAG([Value], 1) OVER (ORDER BY [Date]) > 0
            THEN (([Value] - LAG([Value], 1) OVER (ORDER BY [Date])) / LAG([Value], 1) OVER (ORDER BY [Date])) * 100
            ELSE 0
        END
    FROM DailySales
    ORDER BY [Date];
END
GO
