using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DragDropHandler : MonoBehaviour
{
    [HideInInspector] public bool isDragging;
    [HideInInspector] public Slot slotFrom;
    [HideInInspector] public Slot slotTo;
    [HideInInspector] public bool isRightClick = false;
    [HideInInspector] public bool isMiddleClick = false;
    [Space]
    public Image dragDropIcon;

    private void Update()
    {
        if (isDragging && slotFrom != null)
        {
            dragDropIcon.sprite = slotFrom.icon.sprite;
            dragDropIcon.transform.position = Input.mousePosition;
        }
        else
        {
            dragDropIcon.transform.position = new Vector3(-100000,0,0);
        }
    }
}
