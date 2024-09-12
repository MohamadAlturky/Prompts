using Dapper;
using System.Data;
using System.Threading.Tasks;

public class UserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        using (var connection = _connectionFactory.CreateConnection())
        {
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Id = @Id",
                new { Id = id }
            );
        }
    }
}