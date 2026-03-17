using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using General.Common.Result;
using General.Data.Types;

namespace General.Common.Query;

public static class QueryService
{
    extension<T>(IQueryable<T> queryable) where T : Entity
    {
        public async Task<ListDataResult<T>> ToListDataResultAsync(int skip, int take, List<Filter> filters,
            List<Sort> sorts, CancellationToken cancellationToken, bool calcTotal = true)
        {
            var errors = new List<string>();
            queryable = queryable.Filter(filters, errors);

            if (errors.Count > 0)
            {
                return new ListDataResult<T>([], 0)
                {
                    Errors = errors
                };
            }

            var total = calcTotal ? await queryable.CountAsync(cancellationToken) : 0;
            queryable = sorts.Count > 0 ? queryable.Sort(sorts) : queryable.OrderBy(x => x.Id);

            if (take > 0) 
                queryable = queryable.Page(skip, take);
            
            return new ListDataResult<T>(await queryable.ToListAsync(cancellationToken), total);
        }
        
        public async Task<int> ToCountAsync(List<Filter> filters, CancellationToken cancellationToken)
        {
            return await queryable.Filter<T>(filters, []).CountAsync(cancellationToken);
        }
        
        private IQueryable<T> Filter(List<Filter> filters, List<string> errors)
        {
            if (filters.Count == 0)
                return queryable;

            try
            {
                filters = PreliminaryWork(typeof(T), filters);
                var values = filters.SelectMany(x => x.Constraints.Select(y => y.Keyword)).ToArray();

                var prevConstraintCount = 0;
                List<string> expressions = [];
                
                for (var i = 0; i < filters.Count; i++)
                {
                    expressions.Add(filters[i].ToExpression(i == 0 ? i : i + prevConstraintCount - 1));
                    prevConstraintCount = filters[i].Constraints.Count;
                }

                var predicate = string.Join(" AND ", expressions.Where(x => !string.IsNullOrWhiteSpace(x)));
                queryable = queryable.Where(predicate, values);
            }
            catch (Exception e)
            {
                errors.Add(e.Message);
            }

            return queryable;
        }
        
        private IQueryable<T> Sort(List<Sort> sorts)
        {
            if (sorts.Count == 0) 
                return queryable;
            
            var ordering = string.Join(",", sorts.Select(x => x.ToExpression()));
            return queryable.OrderBy(ordering);
        }

        private IQueryable<T> Page(int skip, int take)
        {
            return queryable.Skip(skip).Take(take);
        }
    }
    
    private static List<Filter> PreliminaryWork(Type type, List<Filter> filters)
    {
        foreach (var filter in filters)
        {
            foreach (var constraint in filter.Constraints)
            {
                constraint.Operator = constraint.Operator!
                    .Replace("equals", "eq")
                    .Replace("notEquals", "neq")
                    .Replace("startWith", "starts-with")
                    .Replace("endsWith", "ends-with")
                    .Replace("notContains", "does-not-contain");

                var currentPropertyType = General.Common.Query.Filter.GetLastPropertyType(type, filter.Field ?? "");
                
                if ((currentPropertyType == typeof(decimal) || currentPropertyType == typeof(decimal?)) &&
                    decimal.TryParse(constraint.Keyword?.ToString() ?? "", out var number))
                {
                    constraint.Keyword = number;
                    continue;
                }

                if ((currentPropertyType == typeof(DateTime) || currentPropertyType == typeof(DateTime?)) &&
                    DateTime.TryParse(constraint.Keyword?.ToString() ?? "", out var datetime))
                {
                    filter.Field = currentPropertyType == typeof(DateTime)
                        ? $"{filter.Field}.Date"
                        : $"{filter.Field}.Value.Date";

                    constraint.Keyword = datetime;
                    continue;
                }

                if (constraint.Keyword is not JArray v) 
                    continue;

                if (currentPropertyType == typeof(int))
                    constraint.Keyword = v.Select(x => Convert.ChangeType(x, currentPropertyType))
                        .Cast<int>().ToArray();

                else if (currentPropertyType == typeof(long))
                    constraint.Keyword = v.Select(x => Convert.ChangeType(x, currentPropertyType))
                        .Cast<long>().ToArray();

                else if (currentPropertyType == typeof(short))
                    constraint.Keyword = v.Select(x => Convert.ChangeType(x, currentPropertyType))
                        .Cast<short>().ToArray();

                else
                    constraint.Keyword = v.Select(x => Convert.ChangeType(x, currentPropertyType))
                        .Cast<int>().ToArray();
            }
            
        }

        return filters;
    }
}