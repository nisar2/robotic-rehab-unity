using UnityEngine;
using UnityEngine.Events;

public class ClickAndDrag : MonoBehaviour
{
    Vector3 mousePositoinOffset;

    public UnityEvent OnDrag = new UnityEvent();

    Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0.1f;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    private void OnMouseDown()
    {
        mousePositoinOffset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + mousePositoinOffset;
        OnDrag.Invoke();
    }

    private void OnDisable()
    {
        OnDrag.RemoveAllListeners();    
    }
}
