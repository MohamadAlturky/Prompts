using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

public class DatabaseQuery
{
    private readonly string _connectionString;

    public DatabaseQuery(string connectionString)
    {
        _connectionString = connectionString;
    }

    public DataTable ExecuteQuery(
        string selectClause,
        string tableName,
        string whereClause,
        Dictionary<string, object> parameters,
        string orderByClause)
    {
        StringBuilder queryBuilder = new StringBuilder();
        queryBuilder.Append($"SELECT {selectClause} FROM [{tableName}] WHERE ");
        queryBuilder.Append(whereClause);
        queryBuilder.Append(" ");
        queryBuilder.Append(orderByClause);

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection))
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }

                try
                {
                    connection.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    return dataTable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return null;
                }
            }
        }
    }
}

// Usage example:
public class Program
{
    public static void Main()
    {
        string connectionString = "Your_Connection_String_Here";
        DatabaseQuery dbQuery = new DatabaseQuery(connectionString);

        string selectClause = "[FirstName], [LastName], [Age]";
        string tableName = "Users";
        string whereClause = "[FirstName] = @p1 AND [Age] > @p2 AND [LastName] LIKE '%' + @p3 + '%'";
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "@p1", "John" },
            { "@p2", 30 },
            { "@p3", "son" }
        };
        string orderByClause = "ORDER BY [LastName] ASC, [Age] DESC, [FirstName] ASC";

        DataTable result = dbQuery.ExecuteQuery(selectClause, tableName, whereClause, parameters, orderByClause);

        if (result != null)
        {
            foreach (DataRow row in result.Rows)
            {
                Console.WriteLine($"FirstName: {row["FirstName"]}, LastName: {row["LastName"]}, Age: {row["Age"]}");
            }
        }
    }
}