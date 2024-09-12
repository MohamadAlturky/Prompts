public class WhereCriterion<T>
{
    public string ColumnName { get; set; }
    public T Value { get; set; }
    public FilterAction Action { get; set; }
}

public enum FilterAction
{
    Equals,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    Like,
    Contains,
    NotEqual
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static string BuildWhereClause<T>(List<WhereCriterion<T>> criteria)
{
    if (criteria == null || !criteria.Any())
        return string.Empty;

    var whereClause = new StringBuilder();
    var parameterIndex = 1;

    foreach (var criterion in criteria)
    {
        if (whereClause.Length > 0)
            whereClause.Append(" AND ");

        switch (criterion.Action)
        {
            case FilterAction.Equals:
                whereClause.AppendFormat("[{0}] = @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.NotEqual:
                whereClause.AppendFormat("[{0}] <> @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.LessThan:
                whereClause.AppendFormat("[{0}] < @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.LessThanOrEqual:
                whereClause.AppendFormat("[{0}] <= @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.GreaterThan:
                whereClause.AppendFormat("[{0}] > @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.GreaterThanOrEqual:
                whereClause.AppendFormat("[{0}] >= @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.Like:
                whereClause.AppendFormat("[{0}] LIKE @p{1}", criterion.ColumnName, parameterIndex);
                break;
            case FilterAction.Contains:
                whereClause.AppendFormat("[{0}] LIKE '%' + @p{1} + '%'", criterion.ColumnName, parameterIndex);
                break;
            default:
                throw new ArgumentException($"Unsupported filter action: {criterion.Action}", nameof(criterion.Action));
        }

        parameterIndex++;
    }

    return whereClause.ToString();
}