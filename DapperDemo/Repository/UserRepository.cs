using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;

public class UserRepository
{
    private readonly IDbConnection _db;

    public UserRepository(IConfiguration config)
    {
        _db = new SqlConnection(config.GetConnectionString("DefaultConnection"));
    }

    public async Task<bool> UserExists(string username)
    {
        var sql = "SELECT 1 FROM Users WHERE Username = @Username";
        return await _db.ExecuteScalarAsync<bool>(sql, new { Username = username });
    }

    public async Task Register(User user, string password)
    {
        using var hmac = new HMACSHA512();
        user.PasswordSalt = hmac.Key;
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        var sql = "INSERT INTO Users (Username, Email, PasswordHash, PasswordSalt) VALUES (@Username, @Email, @PasswordHash, @PasswordSalt)";
        await _db.ExecuteAsync(sql, user);
    }

    public async Task<User> Login(string username, string password)
    {
        var sql = "SELECT * FROM Users WHERE Username = @Username";
        var user = await _db.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });

        if (user == null) return null;

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        if (!computedHash.SequenceEqual(user.PasswordHash))
            return null;

        return user;
    }
}
