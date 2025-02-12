using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class world_gun_ui : MonoBehaviour
{
    public Camera player_cam;
    public bool is_active = true;
    public GameObject world_gun_ui_canvas; // This canvas is now assumed to be in Screen Space (UI)
    public world_gun_data_holder gun_data;
    public TextMeshProUGUI gun_stuff;
    public string Gun_Name;

    // Maximum distance within which the UI becomes visible.
    public float uiVisibleDistance = 20f;

    // Cached components for screenspace conversion.
    public RectTransform uiRect;
    public Canvas parentCanvas;

    void OnEnable()
    {
        // Ensure the UI logic is active upon (re)enable.
        is_active = true;
    }

    void Start()
    {
        if (player_cam == null)
        {
            player_cam = GameObject.FindGameObjectWithTag("Self_cam").GetComponent<Camera>();
        }
        // Update the gun text.
        gun_stuff.text = Gun_Name + " | " + "Level: " + gun_data.Gun_level.Value.ToString() + " | " + gun_data.Rareity_level.Value.ToString();

       // uiRect = world_gun_ui_canvas.GetComponent<RectTransform>();
       // parentCanvas = world_gun_ui_canvas.GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (player_cam == null)
        {
           if(GameObject.FindGameObjectWithTag("Self_cam").GetComponent<Camera>() != null)
            {
                 Camera cam = GameObject.FindGameObjectWithTag("Self_cam").GetComponent<Camera>();
            
           
            if (cam != null)
            {
                player_cam = cam;
            }
            else
            {
                return;
            }
            }
            else
            {
                return;
            }
        }

        // Check if the gun is in view.
        Vector3 viewportPos = player_cam.WorldToViewportPoint(transform.position);
        bool onScreen = viewportPos.z > 0 && viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1;

        bool shouldShow = false;
        if (onScreen)
        {
            // Check distance between the player and the gun.
            float distance = Vector3.Distance(player_cam.transform.position, transform.position);
            Debug.LogError(distance);
            if (distance <= uiVisibleDistance)
            {
                // Check if the player is actually looking at the gun using a dot product.
                Vector3 dirToGun = (transform.position - player_cam.transform.position).normalized;
                float dot = Vector3.Dot(player_cam.transform.forward, dirToGun);
                shouldShow = dot >= 0.85f;
            }
        }

        world_gun_ui_canvas.SetActive(shouldShow);

        // If the UI is active, update the anchored position so that it stays aligned with the gun.
        if (shouldShow)
        {
            if(parentCanvas)
            {
            Vector2 anchoredPos;
            RectTransform canvasRect = parentCanvas.transform as RectTransform;
            Vector2 screenPoint = player_cam.WorldToScreenPoint(transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, parentCanvas.worldCamera, out anchoredPos);
            uiRect.anchoredPosition = anchoredPos;
            }
        }
    }
}
