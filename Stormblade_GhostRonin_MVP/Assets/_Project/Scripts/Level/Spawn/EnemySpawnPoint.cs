using UnityEngine;

public enum EnemySpawnPositionMode
{
    WorldPosition,
    CameraEdge
}

public enum CameraSpawnSide
{
    Left,
    Right
}

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Optional Organization")]
    [SerializeField] private Transform spawnedEnemiesParent;

    [Header("Settings")]
    [SerializeField] private bool spawnOnlyOnce = true;

    [Header("Runtime Debug")]
    [SerializeField] private bool hasSpawned;
    [SerializeField] private GameObject lastSpawnedEnemy;

    [Header("Initial Configuration")]
    [SerializeField] private bool overrideStartingLane = true;
    [SerializeField] private GameplayLane startingLane = GameplayLane.Front;

    [SerializeField] private bool overrideInitialDirection = true;
    [SerializeField] private EnemyHorizontalDirection initialDirection = EnemyHorizontalDirection.Left;

    [Header("Position")]
    [SerializeField] private EnemySpawnPositionMode positionMode = EnemySpawnPositionMode.WorldPosition;

    [SerializeField] private CameraSpawnSide cameraSpawnSide;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private float cameraEdgeMargin = 0.05f;

    [Header("Encounter Participation")]
    [SerializeField] private EncounterResolutionRule encounterResolutionRule = EncounterResolutionRule.DefeatedOrEscaped;

    public EncounterResolutionRule EncounterResolutionRule => encounterResolutionRule;

    public bool HasSpawned => hasSpawned;
    public GameObject LastSpawnedEnemy => lastSpawnedEnemy;

    public GameObject Spawn()
    {
        if(spawnOnlyOnce && hasSpawned)
        {
            Debug.LogWarning($"{name}: este SpawnPoint já foi utilizado.", this);

            return null;
        }

        if(enemyPrefab == null)
        {
            Debug.LogWarning($"{name}: Enemy Prefab não configurado.", this);

            return null;
        }

        Vector3 spawnPosition = GetSpawnPosition();

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, spawnedEnemiesParent);

        ApplyInitialConfiguration(spawnedEnemy);

        hasSpawned = true;
        lastSpawnedEnemy = spawnedEnemy;

        Debug.Log($"[ENEMY SPAWN] {name} criou {spawnedEnemy.name} " + $"em {spawnPosition}.", this);

        return spawnedEnemy;
    }

    private void ApplyInitialConfiguration(GameObject spawnedEnemy)
    {
        if(spawnedEnemy == null)
            return;

        MonoBehaviour[] components = spawnedEnemy.GetComponentsInChildren<MonoBehaviour>(true);

        foreach(MonoBehaviour component in components)
        {
            if(overrideStartingLane && component is IEnemySpawnLaneConfigurable laneConfigurable)
                laneConfigurable.SetInitialLane(startingLane);

            if(overrideInitialDirection && component is IEnemySpawnDirectionConfigurable directionConfigurable)
                directionConfigurable.SetInitialDirection((float)initialDirection);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if(positionMode == EnemySpawnPositionMode.WorldPosition || mainCamera == null)
            return transform.position;

        float viewportX = cameraSpawnSide == CameraSpawnSide.Left ? -cameraEdgeMargin : 1f + cameraEdgeMargin;

        Vector3 viewportPosition = new Vector3(viewportX, 0.5f, Mathf.Abs(mainCamera.transform.position.z));

        Vector3 worldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);

        //Mantém o Y definido pelo Level Design
        worldPosition.y = transform.position.y;
        worldPosition.z = transform.position.z;

        return worldPosition;
    }

    [ContextMenu("TEST/Spawn Enemy")]
    private void TestSpawn()
    {
        Spawn();
    } 
}
