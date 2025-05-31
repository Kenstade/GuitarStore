using System.Data.Common;

namespace BuildingBlocks.Core.Dapper;

public interface IDbConnectionFactory
{
    ValueTask<DbConnection> OpenConnectionAsync();
}