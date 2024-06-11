using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private float zoomSpeed = 2.0f; 
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 20.0f;
    [SerializeField] private float rotationSpeed = 200.0f; 

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

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;
    }

    void Update()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        if (Input.GetMouseButton(0))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, -90, 90);
        }

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target.position);
    }
}
