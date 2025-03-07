using UnityEngine;

public class InteractionHandler :MonoBehaviour
{
    public float interactionDistance = 4.5f;
    private InputSystem_Actions _playerInput;


    private void Start()
    {
        _playerInput = new InputSystem_Actions();
        _playerInput.Player.Enable();
    }
    private void Update()
    {
        if (_playerInput.Player.Interact.WasPressedThisFrame())
        {
            Interact();
        }
    }

    private void Interact()
    {
        WindowHandler windowHandler = GetComponentInParent<WindowHandler>();

        // Raycast para detectar todas las colisiones en la dirección de la vista
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, interactionDistance);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.transform.name);

            // Buscar componente Door en el objeto o en sus padres
            Door door = hit.transform.GetComponent<Door>() ?? hit.transform.GetComponentInParent<Door>();
            if (door != null)
            {
                door.ToggleDoor();
                return;
            }
        }

        // Si no se encontró una puerta, hacemos un Raycast normal para otros objetos
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitObject, interactionDistance))
        {
            Pickup pickup = hitObject.transform.GetComponent<Pickup>();
            Storage storage = hitObject.transform.GetComponent<Storage>();

            if (pickup != null)
            {
                windowHandler.inventory.AddItem(pickup);
            }

            if (storage != null && !storage.opened)
            {
                windowHandler.inventory.opened = true;
                storage.Open(windowHandler.storage);
            }
        }
    }

}
