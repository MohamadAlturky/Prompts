using System.Text;

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
    public static string BuildOrderByClause(List<OrderByCriterion> criteria)
    {
        if (criteria == null || !criteria.Any())
            return string.Empty;

        var orderByClause = new StringBuilder("ORDER BY ");

        for (int i = 0; i < criteria.Count; i++)
        {
            var criterion = criteria[i];

            if (i > 0)
                orderByClause.Append(", ");

            orderByClause.AppendFormat("[{0}] {1}",
                criterion.ColumnName,
                criterion.Direction == OrderDirection.Ascending ? "ASC" : "DESC");
        }

        return orderByClause.ToString();
    }

    static void Main(string[] args)
    {
        // Create a list of OrderByCriterion objects
        var criteria = new List<OrderByCriterion>
        {
            new OrderByCriterion
            {
                ColumnName = "LastName",
                Direction = OrderDirection.Ascending
            },
            new OrderByCriterion
            {
                ColumnName = "Age",
                Direction = OrderDirection.Descending
            },
            new OrderByCriterion
            {
                ColumnName = "FirstName",
                Direction = OrderDirection.Ascending
            }
        };

        // Build the ORDER BY clause
        var orderByClause = BuildOrderByClause(criteria);

        // Print the result
        Console.WriteLine("Generated ORDER BY clause:");
        Console.WriteLine(orderByClause);
    }
}