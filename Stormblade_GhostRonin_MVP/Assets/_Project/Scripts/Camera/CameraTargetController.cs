using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    [Header("Horizontal Progression")]
    [SerializeField] float forwardActivationViewportX = 0.8f;
    [SerializeField] float backwardLimitViewportX = 0.10f;

    [Header("Vertical Progression")]
    [SerializeField, Range(0f, 1f)] private float lowerActivationViewportY = 0.15f;

    [SerializeField, Range(0f, 1f)] private float upperActivationViewportY = 0.85f;

    private float targetX;
    private float targetY;
    private float fixedZ;

    private float previousPlayerX;
    private float previousPlayerY;

    public bool IsBlockingBackwardMovement { get; private set; }

    private void Awake()
    {
        targetX = transform.position.x;
        targetY = transform.position.y;
        fixedZ = transform.position.z;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player != null)
        {
            previousPlayerX = player.position.x;
            previousPlayerY = player.position.y;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null || mainCamera == null)
            return;

        Vector3 playerViewportPosition = mainCamera.WorldToViewportPoint(player.position);

        // --------------------------------
        // HORIZONTAL
        // -------------------------------- 

        IsBlockingBackwardMovement = playerViewportPosition.x <= backwardLimitViewportX;

        float playerDeltaX = player.position.x - previousPlayerX;

        bool reachedForwardLimit = playerViewportPosition.x >= forwardActivationViewportX;

        bool playerMovedForward = playerDeltaX > 0f;

        if(reachedForwardLimit && playerMovedForward)
            targetX += playerDeltaX;

        // --------------------------------
        // VERTICAL
        // -------------------------------- 

        float playerDeltaY = player.position.y - previousPlayerY;

        bool reachedLowerLimit = playerViewportPosition.y <= lowerActivationViewportY;

        bool reachedUpperLimit = playerViewportPosition.y >= upperActivationViewportY;

        bool playerMovedDown = playerDeltaY < 0f;

        bool playerMovedUp = playerDeltaY > 0f;

        if(reachedLowerLimit && playerMovedDown)
            targetY += playerDeltaY;

        if(reachedUpperLimit && playerMovedUp)
            targetY += playerDeltaY;

        // --------------------------------
        // APPLY POSITION
        // -------------------------------- 

        transform.position = new Vector3(targetX, targetY, fixedZ);

        previousPlayerX = player.position.x;
        previousPlayerY = player.position.y;

    }

    public void ResetAfterRespawn(Vector3 playerPosition)
    {
        targetX = playerPosition.x;
        targetY = playerPosition.y;

        transform.position = new Vector3(targetX, targetY, fixedZ);

        previousPlayerX = playerPosition.x;
        previousPlayerY = playerPosition.y;

        IsBlockingBackwardMovement = false;
    }
}
