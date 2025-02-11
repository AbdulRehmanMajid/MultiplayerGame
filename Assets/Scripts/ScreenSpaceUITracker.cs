using UnityEngine;
using UnityEngine.UI;

public class ScreenSpaceUITracker : MonoBehaviour
{
    [Tooltip("The world-space target to track.")]
    public Transform target;

    [Tooltip("The camera rendering the scene.")]
    public Camera mainCamera;

    [Tooltip("The UI element (as a RectTransform) to display on screen.")]
    public RectTransform uiElement;

    [Tooltip("The canvas containing the UI element (should be screen-space).")]
    public Canvas canvas;

    void LateUpdate()
    {
        if (target == null || mainCamera == null || uiElement == null || canvas == null)
            return;

        // Convert the target's world position to viewport point (values between 0 and 1)
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(target.position);

        // Check if target is in front of the camera and within the viewport bounds
        bool onScreen = viewportPoint.z > 0 &&
                        viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                        viewportPoint.y >= 0 && viewportPoint.y <= 1;

        uiElement.gameObject.SetActive(onScreen);

        if (onScreen)
        {
            // Convert target's screen space point to local position within the canvas
            Vector2 canvasPos;
            RectTransform canvasRect = canvas.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mainCamera.WorldToScreenPoint(target.position), canvas.worldCamera, out canvasPos);
            uiElement.anchoredPosition = canvasPos;
        }
    }
}