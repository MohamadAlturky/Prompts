using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

public class PaginatedRequest
{
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}

public class PaginatedResponse<T>
{
    public int TotalCount { get; set; }
    public List<T> Items { get; set; } = new List<T>();
}

public class DatabaseQuery
{
    private readonly string _connectionString;

    public DatabaseQuery(string connectionString)
    {
        _connectionString = connectionString;
    }

    public PaginatedResponse<dynamic> ExecuteQuery(
        string selectClause,
        string tableName,
        string whereClause,
        object parameters,
        string orderByClause,
        PaginatedRequest paginatedRequest)
    {
        string countQuery = $"SELECT COUNT(*) FROM [{tableName}] WHERE {whereClause}";
        string dataQuery = $@"
            SELECT {selectClause} 
            FROM [{tableName}] 
            WHERE {whereClause} 
            {orderByClause}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY";

        using (var connection = new SqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                Console.WriteLine($"Count query: {countQuery}");
                Console.WriteLine($"Data query: {dataQuery}");
                Console.WriteLine("");

                var queryParameters = new DynamicParameters(parameters);
                queryParameters.Add("@Offset", (paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize);
                queryParameters.Add("@PageSize", paginatedRequest.PageSize);

                var totalCount = connection.ExecuteScalar<int>(countQuery, parameters);
                var items = connection.Query(dataQuery, queryParameters).ToList();

                return new PaginatedResponse<dynamic>
                {
                    TotalCount = totalCount,
                    Items = items
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new PaginatedResponse<dynamic>();
            }
        }
    }
}

// Usage example:
public class Program
{
    public static void Main()
    {
        string connectionString = "Server=DESKTOP-OO326C9\\SQLEXPRESS01;Database=ABAC;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;";
        DatabaseQuery dbQuery = new DatabaseQuery(connectionString);

        string selectClause = "FirstName, LastName, Age";
        string tableName = "Users";
        string whereClause = "FirstName = @p1 AND Age > @p2 AND LastName LIKE '%' + @p3 + '%'";
        var parameters = new
        {
            p1 = "John",
            p2 = 30,
            p3 = "son"
        };
        string orderByClause = "ORDER BY LastName ASC, Age DESC, FirstName ASC";

        var paginatedRequest = new PaginatedRequest
        {
            PageSize = 10,
            PageNumber = 1
        };

        var result = dbQuery.ExecuteQuery(selectClause, tableName, whereClause, parameters, orderByClause, paginatedRequest);

        Console.WriteLine($"Total Count: {result.TotalCount}");
        foreach (var row in result.Items)
        {
            Console.WriteLine($"FirstName: {row.FirstName}, LastName: {row.LastName}, Age: {row.Age}");
        }
    }
}