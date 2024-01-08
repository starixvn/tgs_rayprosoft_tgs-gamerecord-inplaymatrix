using Tgs.GameRecord.InplayMatrix.Core;
namespace Tgs.GameRecord.InplayMatrix;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
