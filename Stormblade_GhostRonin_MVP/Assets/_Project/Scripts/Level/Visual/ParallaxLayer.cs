using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private Camera targetCamera;

    [Header("Parallax")]
    [SerializeField, Range(-0.25f, 1f)] private float followCameraX = 0.5f;

    [SerializeField] private bool followY = false;

    [SerializeField, Range(0f, 1f)] private float followCameraY = 0f;

    private Vector3 initialLayerPosition;
    private Vector3 initialCameraPosition;

    private void Awake()
    {
        if(targetCamera == null)
            targetCamera = Camera.main;
    }

    private void Start()
    {
        if(targetCamera == null)
        {
            Debug.LogError($"{name}: nenhuma câmera configurada para o ParallaxLayer.");

            enabled = false;
            return;
        }
            
        initialLayerPosition = transform.position;
        initialCameraPosition = targetCamera.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 cameraDelta = targetCamera.transform.position - initialCameraPosition;

        float targetX = initialLayerPosition.x + cameraDelta.x * followCameraX;

        float targetY = initialLayerPosition.y;

        if (followY)
        {
            targetY = initialLayerPosition.y + cameraDelta.y * followCameraY;
        }

        transform.position = new Vector3(targetX, targetY, initialLayerPosition.z);
    }
}
