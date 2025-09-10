namespace Acontplus.Core.Enums;

/// <summary>
/// Company size classifications for business applications
/// </summary>
public enum CompanySize
{
    /// <summary>
    /// Company size not specified
    /// </summary>
    NotSpecified = 0,

    /// <summary>
    /// Startup or very small company (1-10 employees)
    /// </summary>
    Startup = 1,

    /// <summary>
    /// Small business (11-50 employees)
    /// </summary>
    Small = 2,

    /// <summary>
    /// Medium business (51-250 employees)
    /// </summary>
    Medium = 3,

    /// <summary>
    /// Large business (251-1000 employees)
    /// </summary>
    Large = 4,

    /// <summary>
    /// Enterprise (1001-5000 employees)
    /// </summary>
    Enterprise = 5,

    /// <summary>
    /// Corporation (5000+ employees)
    /// </summary>
    Corporation = 6,

    /// <summary>
    /// Multinational corporation (10000+ employees, multiple countries)
    /// </summary>
    Multinational = 7,

    /// <summary>
    /// Government entity
    /// </summary>
    Government = 8,

    /// <summary>
    /// Non-profit organization
    /// </summary>
    NonProfit = 9,

    /// <summary>
    /// Individual or sole proprietorship
    /// </summary>
    Individual = 10
}
