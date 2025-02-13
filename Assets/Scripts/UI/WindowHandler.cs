using UnityEngine;

public class WindowHandler : MonoBehaviour
{
    [HideInInspector] public InventoryManager inventory;
    [HideInInspector] public CraftingManager crafting;
    public bool isOpen;

    private CameraLook _camera;
    
    private void Start()
    {
        _camera = GetComponentInChildren<CameraLook>();
        inventory = GetComponentInChildren<InventoryManager>();
        crafting = GetComponentInChildren<CraftingManager>();
    }

    private void Update()
    {
        if (isOpen)
        {
            _camera.lockCursor = false;
            _camera.canMove = false;
        }
        else
        {
            _camera.lockCursor = true;
            _camera.canMove = true;
        }
        if (inventory.opened)
        {
            isOpen = true;
        }
        else
            isOpen = false;
    }
}
