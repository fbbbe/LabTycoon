using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("이동 설정")]
    public float dragMoveSpeed = 1.0f;

    [Header("줌 설정")]
    public float zoomSpeed = 3.0f;
    public float minZoom = 3.0f;
    public float maxZoom = 12.0f;

    [Header("카메라 이동 제한")]
    public bool useCameraLimit = true;
    public Vector2 minPosition = new Vector2(-10f, -10f);
    public Vector2 maxPosition = new Vector2(10f, 10f);

    private Camera targetCamera;
    private Vector3 lastMousePosition;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        HandleZoom();
        HandleDragMove();
        ClampCameraPosition();
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0f)
        {
            return;
        }

        float nextSize = targetCamera.orthographicSize - scroll * zoomSpeed;
        nextSize = Mathf.Clamp(nextSize, minZoom, maxZoom);

        targetCamera.orthographicSize = nextSize;
    }

    private void HandleDragMove()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 currentMousePosition = Input.mousePosition;
            Vector3 difference = targetCamera.ScreenToWorldPoint(lastMousePosition) - targetCamera.ScreenToWorldPoint(currentMousePosition);

            transform.position += difference * dragMoveSpeed;

            lastMousePosition = currentMousePosition;
        }
    }

    private void ClampCameraPosition()
    {
        if (useCameraLimit == false)
        {
            return;
        }

        float clampedX = Mathf.Clamp(transform.position.x, minPosition.x, maxPosition.x);
        float clampedY = Mathf.Clamp(transform.position.y, minPosition.y, maxPosition.y);

        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}