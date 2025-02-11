using StarterAssets;
using UnityEngine;
using UnityEngine.EventSystems;

public class FixedTouchField : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [HideInInspector]
    public Vector2 TouchDist;
    [HideInInspector]
    public Vector2 PointerOld;
    [HideInInspector]
    protected int PointerId;
    [HideInInspector]
    public bool Pressed;

    public UICanvasControllerInput mobile_input;
    public float senstivity_x = 1f;
    public float senstivity_y = 1f;
    public float gyroSensitivity = 1f;

    void Start()
    {
        
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    void Update()
    {
        Vector2 touchLook = Vector2.zero;
        if (Pressed)
        {
        
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
                touchLook = new Vector2(TouchDist.x * senstivity_x, -TouchDist.y * senstivity_y);
            }
        }
        else
        {
           
            TouchDist = Vector2.zero;
        }

       
        Vector2 gyroLook = Vector2.zero;
        if (SystemInfo.supportsGyroscope)
        {
            
            gyroLook = new Vector2(-Input.gyro.rotationRateUnbiased.y, -Input.gyro.rotationRateUnbiased.x) * gyroSensitivity;
        }

        
        Vector2 combinedInput = touchLook + gyroLook;
        mobile_input.VirtualLookInput(combinedInput);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Pressed = true;
        PointerId = eventData.pointerId;
        PointerOld = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Pressed = false;
    }
}