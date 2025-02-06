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

    // Use this for initialization
    

    // Update is called once per frame
    void Update()
    {
       if (Pressed)
        {
            
            if (PointerId >= 0 && PointerId < Input.touches.Length)
            {
                TouchDist = Input.touches[PointerId].position - PointerOld;
                PointerOld = Input.touches[PointerId].position;
            }
           
        }
        else
        {
            TouchDist = new Vector2();
        }
        Vector2 inverted = new Vector2(TouchDist.x * senstivity_x,-TouchDist.y * senstivity_y);
        mobile_input.VirtualLookInput(inverted);
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