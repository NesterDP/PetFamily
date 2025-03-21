using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;
using PetFamily.Core.Abstractions;
using PetFamily.SharedKernel.Constants;

namespace PetFamily.Accounts.Infrastructure.TransactionServices;

public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;
    
    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public IDbConnection Create () => 
        new NpgsqlConnection(_configuration.GetConnectionString(InfrastructureConstants.DATABASE));
}