using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static VirtualJoystick Instance { get; private set; }

    [Header("Joystick Components")]
    public RectTransform joystickBackground;
    public RectTransform joystickHandle;
    
    [Header("Settings")]
    public float handleRange = 50f;
    public bool fixedPosition = true; // Keep joystick at fixed position
    
    private Vector2 inputVector;
    private Canvas canvas;
    private bool isDragging = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        canvas = GetComponentInParent<Canvas>();
        
        // Keep joystick visible if fixed position
        if (joystickBackground != null)
        {
            joystickBackground.gameObject.SetActive(fixedPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        
        if (!fixedPosition)
        {
            // Show joystick at touch position (dynamic mode)
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );
            
            joystickBackground.anchoredPosition = localPoint;
            joystickBackground.gameObject.SetActive(true);
        }
        
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out position
        );

        position = Vector2.ClampMagnitude(position, handleRange);
        joystickHandle.anchoredPosition = position;

        inputVector = position / handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        inputVector = Vector2.zero;
        joystickHandle.anchoredPosition = Vector2.zero;
        
        if (!fixedPosition)
        {
            // Hide joystick only in dynamic mode
            joystickBackground.gameObject.SetActive(false);
        }
    }

    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    public bool IsDragging()
    {
        return isDragging;
    }
}
