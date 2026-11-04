public interface IEnemySpawnLaneConfigurable
{
    void SetInitialLane(GameplayLane lane);
}

public interface IEnemySpawnDirectionConfigurable
{
    void SetInitialDirection(float direction);
}

public enum EnemyHorizontalDirection
{
    Left = -1,
    Right = 1
}