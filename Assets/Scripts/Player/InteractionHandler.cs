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
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.forward, out hit, interactionDistance))
        {
            Pickup pickup = hit.transform.GetComponent<Pickup>();
            Storage storage = hit.transform.GetComponent<Storage>();
            if (pickup != null)
            {
                GetComponentInParent<WindowHandler>().inventory.AddItem(pickup);
            }
            if (storage != null)
            {
                if(!storage.opened)
                {
                    GetComponentInParent<WindowHandler>().inventory.opened = true;
                    storage.Open(GetComponentInParent<WindowHandler>().storage);
                }
            }
        }
    }
}
