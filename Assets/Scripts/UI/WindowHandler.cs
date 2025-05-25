using UnityEngine;

public class WindowHandler : MonoBehaviour
{
    [HideInInspector] public InventoryManager inventory;
    [HideInInspector] public CraftingManager crafting;
    [HideInInspector] public StorageUI storage;
    [HideInInspector] public BuildingHandler building;
    [HideInInspector] public GameMenu gameMenu;
    public GameObject bloodImage;
    public bool isOpen;

    private CameraLook _camera;
    
    private void Start()
    {
        _camera = GetComponentInChildren<CameraLook>();
        storage = GetComponentInChildren<StorageUI>();
        inventory = GetComponentInChildren<InventoryManager>();
        crafting = GetComponentInChildren<CraftingManager>();
        building = GetComponentInChildren<BuildingHandler>();
        gameMenu = FindAnyObjectByType<GameMenu>();
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

        if (gameMenu != null && gameMenu.opened)
        {
            // Desactivar todo cuando el menú está abierto
            if (inventory != null) inventory.inventoryPanel.SetActive(false);
            if (inventory != null && inventory.hotBarContentHolder != null) inventory.hotBarContentHolder.gameObject.SetActive(false);
            if (crafting != null) crafting.gameObject.SetActive(false);
            isOpen = true;
            bloodImage.SetActive(false);
            return;
        }

        // Lógica normal para inventario/crafting
        if (inventory != null && inventory.opened)
        {
            inventory.inventoryPanel.SetActive(true);
            if (inventory.hotBarContentHolder != null) inventory.hotBarContentHolder.gameObject.SetActive(true);
            if (crafting != null) crafting.gameObject.SetActive(true);
            isOpen = true;
            bloodImage.SetActive(false);
        }
        else
        {
            if (inventory != null) inventory.inventoryPanel.SetActive(false);
            if (inventory.hotBarContentHolder != null) inventory.hotBarContentHolder.gameObject.SetActive(true);
            if (crafting != null) crafting.gameObject.SetActive(false);
            isOpen = false;
            bloodImage.SetActive(true);
        }
    }
}
