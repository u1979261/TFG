using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    private InventoryManager inventory;
    public RecipieTemplate recipieTemplate;
    public CraftingRecipieSO[] recipies;

    public Transform contentHolder;


    public RecipieTemplate recipieInCraft;
    private bool _isCrafting;
    private float _currentTimer;

    public bool opened;
    private void Start()
    {
        RecepiesGenerator();
        inventory = GetComponentInParent<InventoryManager>();
    }

    private void Update()
    {
        if (_isCrafting) 
        { 
            if(_currentTimer > 0)
            {
                recipieInCraft.timeText.text = _currentTimer.ToString("f0");
            }
            else
            {
                recipieInCraft.timeText.text = "";
                inventory.AddItem(recipieInCraft.recipie.output, recipieInCraft.recipie.outputAmount);
                _isCrafting = false;
            }
            _currentTimer -= Time.deltaTime;
        }
        if (opened && inventory.opened)
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        else
            transform.position = new Vector3(-10000, 0, 0);
    }
    public void RecepiesGenerator()
    {
        for (int i = 0; i < recipies.Length; i++)
        {
            RecipieTemplate recipie = Instantiate(recipieTemplate.gameObject, contentHolder).GetComponent<RecipieTemplate>();

            recipie.recipie = recipies[i];
            recipie.icon.sprite = recipies[i].recipieIcon;
            recipie.nameText.text = recipies[i].recipieName;
            recipie.timeText.text = "";

            for (int j = 0; j < recipies[i].requirements.Length; j++)
            {
                if(j == 0)
                {
                    recipie.requirementsText.text = $"{recipies[i].requirements[j].data.itemName} {recipies[i].requirements[j].amount}";
                }
                else
                {
                    recipie.requirementsText.text = $"{recipie.requirementsText.text}, {recipies[i].requirements[j].data.itemName} {recipies[i].requirements[j].amount}";
                }
            }
        }
    }

    public void Craft(RecipieTemplate recipie)
    {
        if (!HasResources(recipie.recipie) || _isCrafting)
        {
            return;
        }
        TakeResources(recipie.recipie);
        recipieInCraft = recipie;
        _isCrafting = true;
        _currentTimer = recipie.recipie.craftingTime;

    }

    public void Cancel(RecipieTemplate recipie)
    {
        if(!_isCrafting) return;
        for (int i = 0; i < recipie.recipie.requirements.Length; i++)
        {
            inventory.AddItem(recipie.recipie.requirements[i].data, recipie.recipie.requirements[i].amount);
        }

        _isCrafting=false;

        recipieInCraft.timeText.text = "";
    }

    public bool HasResources(CraftingRecipieSO recipie)
    {
        bool canCraft = true;  
        int[] stacksNeeded = null;
        int[] stacksAvailable = null;
        List<int> stacksNeededList = new List<int>();

        //GET STACKS NEEDED
        for (int i = 0; i < recipie.requirements.Length; i++)
        {
            stacksNeededList.Add(recipie.requirements[i].amount);
        }
        stacksNeeded = stacksNeededList.ToArray();
        stacksAvailable = new int[stacksNeeded.Length];

        //CHECK FOR ITEMS

        for (int j = 0; j < recipie.requirements.Length ; j++)
        {
            for (int i = 0; i < inventory.inventorySlots.Length; i++)
            {
                if (inventory.inventorySlots[i].data == recipie.requirements[j].data)
                {
                    stacksAvailable[j] += inventory.inventorySlots[i].stackSize;
                }
            }
        }

        //CHECK IF CAN CRAFT
        for (int i = 0; i< stacksAvailable.Length; i++)
        {
            if(stacksAvailable[i] < stacksNeeded[i]){
                canCraft = false;
                break;
            }
        }
        return canCraft;
    }
    public void TakeResources(CraftingRecipieSO recipie)
    {
        int[] stacksNeeded = null;
        List<int> stacksNeededList = new List<int>();

        //GET STACKS NEEDED
        for (int i = 0; i < recipie.requirements.Length; i++)
        {
            stacksNeededList.Add(0);
        }
        stacksNeeded = stacksNeededList.ToArray();

       //TAKE ITEMS

        for (int i = 0;i < recipie.requirements.Length; i++)
        {
            for (int j = 0; j < inventory.inventorySlots.Length; j++)
            {
                if (!inventory.inventorySlots[j].isEmpty)
                {

                    if (inventory.inventorySlots[j].data == recipie.requirements[i].data)
                    {
                        if (stacksNeeded[i] < recipie.requirements[i].amount)
                        {
                            if (stacksNeeded[i] + inventory.inventorySlots[j].stackSize > recipie.requirements[i].amount)
                            {
                                int amountLeftOnSlot = (inventory.inventorySlots[j].stackSize + stacksNeeded[i]) - recipie.requirements[i].amount;
                                inventory.inventorySlots[j].stackSize = amountLeftOnSlot;
                                stacksNeeded[i] = recipie.requirements[i].amount;
                            }
                            else
                            {
                                stacksNeeded[i] += inventory.inventorySlots[j].stackSize;
                                inventory.inventorySlots[j].Clean();
                            }
                        }

                        inventory.inventorySlots[j].UpdateSlot();
                    }
                }
            }
        }
    }
}
