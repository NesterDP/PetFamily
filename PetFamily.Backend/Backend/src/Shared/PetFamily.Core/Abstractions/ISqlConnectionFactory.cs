using System.Data;

namespace PetFamily.Core.Abstractions;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}