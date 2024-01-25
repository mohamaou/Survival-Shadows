using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;


public class TouchControle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static TouchControle Instance {get; protected set; }
    [HideInInspector] public Vector2 TouchDist;
    private Vector2 pointerOld;
    private int pointerId;
    private bool pressed;
    
    private Vector3 starMousePoint;
    private float touchTime, touchDest;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        StartCoroutine(GetTouchDst());
    }



    private IEnumerator GetTouchDst()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (pressed)
            {
                if (pointerId >= 0 && pointerId < Input.touches.Length)
                {
                    TouchDist = Input.touches[pointerId].position - pointerOld;
                    pointerOld = Input.touches[pointerId].position;
                }
                else
                {
                    TouchDist = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - pointerOld;
                    pointerOld = Input.mousePosition;
                }
            }
            else
            {
                TouchDist = new Vector2();
            }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        pointerId = eventData.pointerId;
        pointerOld = eventData.position;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
    }


    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            starMousePoint = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            touchTime += Time.deltaTime;
            touchDest = Vector3.Distance(Input.mousePosition, starMousePoint);
        }
        if (Input.GetMouseButtonUp(0))
        {
            touchTime = 0;
        }
    }
}
