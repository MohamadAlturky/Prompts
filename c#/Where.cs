using Microsoft.Data.SqlClient;
using System.Text;

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
public class OrderByCriterion
{
    public string ColumnName { get; set; }
    public OrderDirection Direction { get; set; }
}

public enum OrderDirection
{
    Ascending,
    Descending
}
class Program
{
    public static (string WhereClause, List<SqlParameter> Parameters) BuildWhereClause<T>(List<WhereCriterion<T>> criteria)
    {
        if (criteria == null || !criteria.Any())
            return (string.Empty, new List<SqlParameter>());

        var whereClause = new StringBuilder();
        var parameters = new List<SqlParameter>();
        var parameterIndex = 1;

        foreach (var criterion in criteria)
        {
            if (whereClause.Length > 0)
                whereClause.Append(" AND ");

            string parameterName = $"@p{parameterIndex}";
            UpdateWhereClause(whereClause, criterion, parameterName);

            parameters.Add(new SqlParameter(parameterName, criterion.Value));
            parameterIndex++;
        }

        return (whereClause.ToString(), parameters);
    }

    private static void UpdateWhereClause<T>(StringBuilder whereClause, WhereCriterion<T> criterion, string parameterName)
    {
        switch (criterion.Action)
        {
            case FilterAction.Equals:
                whereClause.AppendFormat("[{0}] = {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.NotEqual:
                whereClause.AppendFormat("[{0}] <> {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.LessThan:
                whereClause.AppendFormat("[{0}] < {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.LessThanOrEqual:
                whereClause.AppendFormat("[{0}] <= {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.GreaterThan:
                whereClause.AppendFormat("[{0}] > {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.GreaterThanOrEqual:
                whereClause.AppendFormat("[{0}] >= {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.Like:
                whereClause.AppendFormat("[{0}] LIKE {1}", criterion.ColumnName, parameterName);
                break;
            case FilterAction.Contains:
                whereClause.AppendFormat("[{0}] LIKE '%' + {1} + '%'", criterion.ColumnName, parameterName);
                break;
            default:
                throw new ArgumentException($"Unsupported filter action: {criterion.Action}", nameof(criterion.Action));
        }
    }

    static void Main(string[] args)
    {
        // Create a list of WhereCriterion objects
        var criteria = new List<WhereCriterion<object>>
        {
            new WhereCriterion<object>
            {
                ColumnName = "FirstName",
                Value = "John",
                Action = FilterAction.Equals
            },
            new WhereCriterion<object>
            {
                ColumnName = "Age",
                Value = 30,
                Action = FilterAction.GreaterThan
            },
            new WhereCriterion<object>
            {
                ColumnName = "LastName",
                Value = "son",
                Action = FilterAction.Contains
            }
        };

        // Build the WHERE clause and get parameters
        var (whereClause, parameters) = BuildWhereClause(criteria);

        // Print the result
        Console.WriteLine("Generated WHERE clause:");
        Console.WriteLine(whereClause);

        Console.WriteLine("\nParameters:");
        foreach (var param in parameters)
        {
            Console.WriteLine($"{param.ParameterName}: {param.Value}");
        }
    }
}