using System.Linq.Expressions;

namespace AirBnb.Interface
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; }
        Expression<Func<T, bool>> AdditionalCriteria { get; }
        List<Expression<Func<T, object>>> Includes { get; }
        List<string> IncludeStrings { get; }
        List<Expression<Func<T, object>>> OrderBy { get; }
        List<Expression<Func<T, object>>> OrderByDescending { get; }
        public Dictionary<Expression<Func<T, object>>, Boolean> MultiSort { get; }
        string OrderByString { get; }
        string OrderByDescendingString { get; }

        Expression<Func<T, object>> GroupBy { get; }

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}
