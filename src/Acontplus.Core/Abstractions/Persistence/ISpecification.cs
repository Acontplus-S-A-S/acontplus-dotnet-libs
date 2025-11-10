namespace Acontplus.Core.Abstractions.Persistence;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    IReadOnlyList<Expression<Func<T, object>>> Includes { get; }
    IReadOnlyList<string> IncludeStrings { get; }
    IReadOnlyList<OrderByExpression<T>> OrderByExpressions { get; }
    PaginationRequest Pagination { get; }
    bool IsPagingEnabled { get; }
    bool IsTrackingEnabled { get; }
}

public abstract class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; protected set; } = null!;

    public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes.AsReadOnly();
    public IReadOnlyList<string> IncludeStrings => _includeStrings.AsReadOnly();
    public IReadOnlyList<OrderByExpression<T>> OrderByExpressions => _orderByExpressions.AsReadOnly();
    public PaginationRequest Pagination { get; private set; } = new();
    public bool IsPagingEnabled { get; private set; } = false;
    public bool IsTrackingEnabled { get; private set; } = false;

    private readonly List<Expression<Func<T, object>>> _includes = [];
    private readonly List<string> _includeStrings = [];
    private readonly List<OrderByExpression<T>> _orderByExpressions = [];

    protected BaseSpecification(Expression<Func<T, bool>>? criteria = null)
    {
        Criteria = criteria ?? (x => true);
    }

    protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        _includes.Add(includeExpression);
    }

    protected virtual void AddInclude(string includeString)
    {
        _includeStrings.Add(includeString);
    }

    protected virtual void AddOrderBy(Expression<Func<T, object>> orderByExpression, bool isDescending = false)
    {
        _orderByExpressions.Add(new OrderByExpression<T>(orderByExpression, isDescending));
    }

    protected virtual void ApplyPaging(PaginationRequest pagination)
    {
        Pagination = pagination;
        IsPagingEnabled = true;
    }

    protected virtual void ApplyTracking(bool isTracking = true)
    {
        IsTrackingEnabled = isTracking;
    }
}

public class OrderByExpression<T>
{
    public Expression<Func<T, object>> Expression { get; }
    public bool IsDescending { get; }

    public OrderByExpression(Expression<Func<T, object>> expression, bool isDescending = false)
    {
        ArgumentNullException.ThrowIfNull(expression);
        Expression = expression;
        IsDescending = isDescending;
    }
}
