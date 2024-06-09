using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class CameraControllerTouch : MonoBehaviour
{
    public Transform target; // The target object to orbit around
    public float distance = 10.0f; // The initial distance from the target
    public float zoomSpeed = 0.5f; // The speed of zooming
    public float minDistance = 2.0f; // The minimum distance to the target
    public float maxDistance = 20.0f; // The maximum distance to the target
    public float rotationSpeed = 0.2f; // The speed of rotation
    public float autoPanSpeed = 10.0f; // Speed of automatic panning
    public float inactivityDelay = 5.0f; // Time to wait before auto-panning resumes
    public float smoothTime = 0.3f; // Time for smoothing the zoom
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float targetDistance;
    private float velocity = 0.0f;
    private bool isTouching = false;
    private Coroutine autoPanCoroutine;

    public Action OnClick;

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

        targetDistance = distance;

        //StartAutoPan();
    }

    void Update()
    {
        // Handle touch input for rotation and zoom
        if (Input.touchCount > 0)
        {

            if (!isTouching)
            {
                isTouching = true;
                StopAutoPan();
            }

            if (Input.touchCount == 1) // One finger touch for rotation
            {
                OnClick?.Invoke();
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (IsPointerOverUIObject(touch))
                    {
                        isTouching = false;
                        return;
                    }
                }
                if (touch.phase == TouchPhase.Moved)
                {
                    currentX += touch.deltaPosition.x * rotationSpeed;
                    currentY -= touch.deltaPosition.y * rotationSpeed;
                    currentY = Mathf.Clamp(currentY, -90, 90); // Clamp the vertical rotation
                }
            }
            else if (Input.touchCount == 2) // Two finger touch for zoom
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);

                // Calculate the position difference between the touches in the current frame and the previous frame.
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // Find the difference in the distances between each frame.
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                targetDistance += deltaMagnitudeDiff * zoomSpeed;
                targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
            }
        }
        else
        {
            if (isTouching)
            {
                isTouching = false;
                Invoke(nameof(StartAutoPan), inactivityDelay);
            }
        }

        // Smoothly interpolate the distance
        distance = Mathf.SmoothDamp(distance, targetDistance, ref velocity, smoothTime);

        // Update the camera position and rotation
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target.position);
    }

    public void StartAutoPan()
    {
        if (autoPanCoroutine == null)
        {
            autoPanCoroutine = StartCoroutine(AutoPan());
        }
    }

    void StopAutoPan()
    {
        if (autoPanCoroutine != null)
        {
            StopCoroutine(autoPanCoroutine);
            autoPanCoroutine = null;
        }
    }

    IEnumerator AutoPan()
    {
        while (true)
        {
            currentX += autoPanSpeed * Time.deltaTime;
            yield return null;
        }
    }

    bool IsPointerOverUIObject(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.position.x, touch.position.y);
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
}
