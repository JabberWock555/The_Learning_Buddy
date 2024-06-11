using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class CameraControllerTouch : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 10.0f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float minDistance = 2.0f;
    [SerializeField] private float maxDistance = 20.0f;
    [SerializeField] private float rotationSpeed = 0.2f;
    [SerializeField] private float autoPanSpeed = 10.0f;
    [SerializeField] private float inactivityDelay = 5.0f; 
    [SerializeField] private float smoothTime = 0.3f;

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

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        targetDistance = distance;

        //StartAutoPan();
    }

    void Update()
    {
        TouchControls();

        distance = Mathf.SmoothDamp(distance, targetDistance, ref velocity, smoothTime);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 direction = new Vector3(0, 0, -distance);
        transform.position = target.position + rotation * direction;
        transform.LookAt(target.position);
    }

    private void TouchControls()
    {
        if (Input.touchCount > 0)
        {

            if (!isTouching)
            {
                isTouching = true;
                StopAutoPan();
            }

            if (Input.touchCount == 1)
            {
                OnClick?.Invoke();
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Moved)
                {
                    currentX += touch.deltaPosition.x * rotationSpeed;
                    currentY -= touch.deltaPosition.y * rotationSpeed;
                    currentY = Mathf.Clamp(currentY, -90, 90);
                }
            }
            else if (Input.touchCount == 2)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);


                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;


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

}
