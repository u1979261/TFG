using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public ResourceDataSO [] resourceData;
    public int hits;
    public ItemSO [] prefferedTool;


    private void Update()
    {
        if (hits <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Recolect(ItemSO toolUsed, InventoryManager invetory)
    {
        //bool usingRightTool = false;
        float finalMultiplier = 0f;

        //CHECK IF THE TOOL USED IS THE RIGHT ONE
        if (prefferedTool.Length > 0)
        {
            for (int i = 0; i < prefferedTool.Length; i++)
            {
                if (toolUsed == prefferedTool[i])
                {
                    //usingRightTool = true;
                    finalMultiplier = toolUsed.toolMultiplier;
                    break;
                }
            }
        }

        // RECOLECT THE RESOURCE
        int selectedResource = Random.Range(0, resourceData.Length);
        float amountToAdd = resourceData[selectedResource].amount;

        
        amountToAdd *= finalMultiplier;
        
        int finalAmountToAdd = Mathf.FloorToInt(amountToAdd);
        invetory.AddItem(resourceData[selectedResource].items, finalAmountToAdd);
        hits--;
    }
}
