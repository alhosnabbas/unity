using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public static class UIUtils
{
    public static bool IsTouchOverUI(Vector2 touchPosition)
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
}
