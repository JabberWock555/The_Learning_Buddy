using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The target object to orbit around
    public float distance = 10.0f; // The initial distance from the target
    public float zoomSpeed = 2.0f; // The speed of zooming
    public float minDistance = 2.0f; // The minimum distance to the target
    public float maxDistance = 20.0f; // The maximum distance to the target
    public float rotationSpeed = 200.0f; // The speed of rotation

    private float currentX = 0.0f;
    private float currentY = 0.0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set. Please set the target object.");
            enabled = false;
            return;
        }

        // Initialize the camera position
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void Update()
    {
        // Zooming in and out
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // Rotating the camera
        if (Input.GetMouseButton(0))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, -90, 90); // Clamp the vertical rotation
        }

        // Update the camera position and rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target.position);
    }
}
