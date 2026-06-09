using UnityEngine;

public class CameraTargetController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera mainCamera;

    [Header("Horizontal Progression")]
    [SerializeField] float forwardActivationViewportX = 0.8f;
    [SerializeField] float horizontalFollowSmooth = 6f;
    [SerializeField] float backwardLimitViewportX = 0.10f;

    private float maxReachedX;
    private float targetX;
    private float fixedY;
    private float fixedZ;

    public bool IsBlockingBackwardMovement { get; private set; }

    private void Awake()
    {
        maxReachedX = transform.position.x;
        targetX = transform.position.x;
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {

        if (player == null || mainCamera == null)
            return;

        Vector3 playerViewportPosition = mainCamera.WorldToViewportPoint(player.position);

        IsBlockingBackwardMovement = playerViewportPosition.x <= backwardLimitViewportX;

        if (playerViewportPosition.x >= forwardActivationViewportX && player.position.x > maxReachedX)
        {
            maxReachedX = player.position.x;
            targetX = maxReachedX;
        }

        float smoothedX = Mathf.Lerp(transform.position.x, targetX, horizontalFollowSmooth * Time.deltaTime);

        transform.position = new Vector3(smoothedX, fixedY, fixedZ);

        
    }
}
