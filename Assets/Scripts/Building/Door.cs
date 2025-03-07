using UnityEngine;

public class Door : MonoBehaviour
{
    private Animation doorAnimation;
    private BoxCollider doorCollider; // BoxCollider de colisión
    public bool isOpen = false; // Estado de la puerta

    void Start()
    {
        doorAnimation = GetComponentInChildren<Animation>();
        doorCollider = GetComponent<BoxCollider>(); // Obtiene el BoxCollider de la puerta
    }

    public void ToggleDoor()
    {
        if (doorAnimation == null) return;

        if (isOpen)
        {
            doorAnimation.Play("CloseDoor");    
            isOpen = false;
            if (doorCollider != null) doorCollider.enabled = true;
        }
        else
        {
            doorAnimation.Play("OpenDoor");
            isOpen = true;
            if (doorCollider != null) doorCollider.enabled = false;
        }
    }
}
