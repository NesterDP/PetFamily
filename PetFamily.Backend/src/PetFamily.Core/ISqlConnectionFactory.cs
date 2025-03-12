using System.Data;

namespace PetFamily.Core;

public interface ISqlConnectionFactory
{
    IDbConnection Create();
}