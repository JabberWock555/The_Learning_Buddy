using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HorizontalImageScroller : MonoBehaviour
{
    public RectTransform imageContainer; // The container with the HorizontalLayoutGroup
    public Button leftButton; // Button to move left
    public Button rightButton; // Button to move right
    public float scrollDuration = 0.5f; // Duration of the scroll animation
    public int visibleCount = 5; // Number of visible objects

    public float scrollStep; // Distance to move (width of one image)
    private float targetPositionX; // Target position for the container
    private bool isScrolling = false;
    private int currentIndex = 0; // Current index of the first visible image

    private float maxPos_X;
    void Start()
    {
        // Add listeners to buttons
        leftButton.onClick.AddListener(MoveLeft);
        rightButton.onClick.AddListener(MoveRight);

        // Calculate the scroll step based on the width of the first child
        // if (imageContainer.childCount > 0)
        // {
        //     RectTransform firstChild = imageContainer.GetChild(0).GetComponent<RectTransform>();
        //     scrollStep = firstChild.rect.width;
        // }
        // else
        // {
        //     Debug.LogError("Image container has no children.");
        //     enabled = false;
        // }
        maxPos_X = imageContainer.anchoredPosition.x;
        // Ensure the container starts at the correct position
        imageContainer.anchoredPosition = new Vector2(imageContainer.anchoredPosition.x, imageContainer.anchoredPosition.y);
    }

    void MoveLeft()
    {
        if (!isScrolling && currentIndex > 0)
        {
            currentIndex--;
            float newPosition = imageContainer.anchoredPosition.x + scrollStep;
            StartCoroutine(ScrollToPosition(newPosition));
        }
    }

    void MoveRight()
    {
        if (!isScrolling && currentIndex < imageContainer.childCount - visibleCount)
        {
            currentIndex++;
            float newPosition = imageContainer.anchoredPosition.x - scrollStep;
            StartCoroutine(ScrollToPosition(newPosition));
        }
    }

    IEnumerator ScrollToPosition(float newPosition)
    {
        isScrolling = true;
        targetPositionX = Mathf.Clamp(newPosition, GetMinScrollPosition(), GetMaxScrollPosition());

        float startPosition = imageContainer.anchoredPosition.x;
        float elapsedTime = 0f;

        while (elapsedTime < scrollDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / scrollDuration);
            float newPositionX = Mathf.Lerp(startPosition, targetPositionX, t);
            imageContainer.anchoredPosition = new Vector2(newPositionX, imageContainer.anchoredPosition.y);
            yield return null;
        }

        // Snap to the final target position to avoid any precision issues
        imageContainer.anchoredPosition = new Vector2(targetPositionX, imageContainer.anchoredPosition.y);
        isScrolling = false;
    }

    float GetMinScrollPosition()
    {
        return -(scrollStep * (imageContainer.childCount - visibleCount));
    }

    float GetMaxScrollPosition()
    {
        return maxPos_X;
    }
}
