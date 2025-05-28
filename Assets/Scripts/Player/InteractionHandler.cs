using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHandler :MonoBehaviour
{
    public float interactionDistance = 4.5f;
    private InputSystem_Actions _playerInput;
    private AudioSource audioS;
    public AudioClip pickupSound;
    public TextMeshProUGUI interactText;

    private void Start()
    {
        audioS = GetComponent<AudioSource>();
        _playerInput = new InputSystem_Actions();
        _playerInput.Player.Enable();
    }
    private void Update()
    {   
        Interact();   
    }
    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

    private void OnDestroy()
    {
        if (_playerInput != null)
        {
            _playerInput.Dispose();
            _playerInput = null;
        }
    }
    private void Interact()
    {
        WindowHandler windowHandler = GetComponentInParent<WindowHandler>();

        // Raycast para detectar todas las colisiones en la dirección de la vista
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, interactionDistance);

        foreach (RaycastHit hit in hits)
        {
            // Buscar componente Door en el objeto o en sus padres
            Door door = hit.transform.GetComponent<Door>() ?? hit.transform.GetComponentInParent<Door>();
            if (door != null)
            {
                if(Input.GetKeyDown(KeyCode.E))
                    door.ToggleDoor();
                return;
            }
        }

        // Si no se encontró una puerta, hacemos un Raycast normal para otros objetos
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitObject, interactionDistance))
        {
            Pickup pickup = hitObject.transform.GetComponent<Pickup>();
            Storage storage = hitObject.transform.GetComponent<Storage>();
            Bed bed = hitObject.transform.GetComponent<Bed>();


            if (_playerInput.Player.Interact.WasPressedThisFrame())
            {
                if (pickup != null)
                {

                    windowHandler.inventory.AddItem(pickup);
                    if (pickupSound != null && audioS != null)
                    {
                        audioS.PlayOneShot(pickupSound);
                    }
                }

                if (bed != null)
                {
                    bed.SaveBed();
                }

                if (storage != null && !storage.opened)
                {
                    windowHandler.inventory.opened = true;
                    storage.Open(windowHandler.storage);
                }
            }
            if (pickup != null || storage != null || bed != null)
            {
                interactText.gameObject.SetActive(true);
                if (pickup != null)
                {
                    interactText.text = $"x{pickup.stackSize} {pickup.data.itemName}";
                }
                if (storage != null)
                {
                    interactText.text = $"Open";
                }
                if (bed != null)
                {
                    interactText.text = $"Sleep";
                }
            }
            else
            {
                interactText.gameObject.SetActive(false);
            }
        }
        else
        {
            interactText.gameObject.SetActive(false);
        }
    }
}
