using Acontplus.Core.Dtos.Requests;
using Acontplus.TestApplication.Helpers;
using Acontplus.TestApplication.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Acontplus.TestApi.Endpoints.Business.Analytics;

/// <summary>
/// Sales Analytics endpoints demonstrating Acontplus.Analytics package usage
/// Clean Architecture: API → Application → Analytics Library
/// </summary>
public static class SalesAnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapSalesAnalyticsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/analytics/sales")
            .WithTags("Sales Analytics");

        // Dashboard Statistics
        group.MapGet("/dashboard", async (
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? language,
            [FromServices] ISalesAnalyticsService analyticsService,
            CancellationToken cancellationToken) =>
        {
            var filter = new FilterRequest
            {
                Filters = new Dictionary<string, object>
                {
                    { "StartDate", startDate ?? DateTime.UtcNow.AddDays(-30) },
                    { "EndDate", endDate ?? DateTime.UtcNow }
                }
            };

            var result = await analyticsService.GetDashboardStatsAsync(filter, cancellationToken);

            return result.Match(
                success: data =>
                {
                    // Apply localization at presentation layer
                    data.Labels = SalesAnalyticsLocalization.GetLabels(language ?? "en");
                    return Results.Ok(data);
                },
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        })
        .WithName("GetSalesDashboard")
        .WithSummary("Get comprehensive sales dashboard statistics")
        .WithDescription("Returns KPIs for sales performance including revenue, orders, customers, and payment methods")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Real-Time Statistics
        group.MapGet("/realtime", async (
            [FromQuery] string? language,
            [FromServices] ISalesAnalyticsService analyticsService,
            CancellationToken cancellationToken) =>
        {
            var result = await analyticsService.GetRealTimeStatsAsync(null, cancellationToken);

            return result.Match(
                success: data =>
                {
                    data.Labels = SalesAnalyticsLocalization.GetLabels(language ?? "en");
                    return Results.Ok(data);
                },
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        })
        .WithName("GetSalesRealTime")
        .WithSummary("Get real-time sales statistics")
        .WithDescription("Returns live sales metrics including active sales, pending payments, and current hour revenue")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Aggregated Statistics (Time-series)
        group.MapGet("/aggregated", async (
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? groupBy,
            [FromQuery] string? language,
            [FromServices] ISalesAnalyticsService analyticsService,
            CancellationToken cancellationToken) =>
        {
            var filter = new FilterRequest
            {
                Filters = new Dictionary<string, object>
                {
                    { "StartDate", startDate },
                    { "EndDate", endDate },
                    { "GroupBy", groupBy ?? "day" } // hour, day, week, month, year
                }
            };

            var result = await analyticsService.GetAggregatedStatsAsync(filter, cancellationToken);

            return result.Match(
                success: data =>
                {
                    var labels = SalesAnalyticsLocalization.GetLabels(language ?? "en");
                    foreach (var item in data)
                    {
                        item.Labels = labels;
                    }
                    return Results.Ok(data);
                },
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        })
        .WithName("GetSalesAggregated")
        .WithSummary("Get aggregated sales statistics by time period")
        .WithDescription("Returns time-series sales data grouped by hour/day/week/month/year with statistical analysis")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        // Trend Analysis
        group.MapGet("/trends", async (
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string? metric,
            [FromQuery] string? language,
            [FromServices] ISalesAnalyticsService analyticsService,
            CancellationToken cancellationToken) =>
        {
            var filter = new FilterRequest
            {
                Filters = new Dictionary<string, object>
                {
                    { "StartDate", startDate },
                    { "EndDate", endDate },
                    { "Metric", metric ?? "revenue" } // revenue, orders, customers, aov
                }
            };

            var result = await analyticsService.GetTrendsAsync(filter, cancellationToken);

            return result.Match(
                success: data =>
                {
                    var labels = SalesAnalyticsLocalization.GetLabels(language ?? "en");
                    foreach (var item in data)
                    {
                        item.Labels = labels;
                    }
                    return Results.Ok(data);
                },
                failure: error => Results.BadRequest(new { error = error.Message, code = error.Code }));
        })
        .WithName("GetSalesTrends")
        .WithSummary("Get sales trend analysis with forecasting")
        .WithDescription("Returns trend data with moving averages, year-over-year comparisons, and anomaly detection")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
