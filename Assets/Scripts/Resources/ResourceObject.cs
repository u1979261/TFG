using UnityEngine;

public class ResourceObject : MonoBehaviour
{
    public enum DeathType { Destroy, EnablePhysics}
    public DeathType deathType;
    public ResourceDataSO [] resourceData;
    public int hits;
    private AudioSource audioS;
    public ItemSO [] prefferedTool;
    public float forceDestruction = 100f;

    bool hasDied;
    private void Update()
    {
        if (hits <= 0 && !hasDied)
        {
            
            if(deathType == DeathType.Destroy)
            {
                Destroy(gameObject);
            }
            else if(deathType == DeathType.EnablePhysics)
            {
               if(GetComponent<Rigidbody>() != null)
               {
                    GetComponent<Rigidbody>().isKinematic = false;
                    GetComponent<Rigidbody>().useGravity = true;

                    if(GetComponent<Tree>() != null)
                    {
                        audioS = GetComponent<AudioSource>();
                        audioS.PlayOneShot(GetComponent<Tree>().treeSound);
                    }
                       
                    GetComponent<Rigidbody>().AddTorque(Vector3.right * 100);
                    Destroy(gameObject, 5f);
                }
               else
                    Destroy(gameObject);
            }
            hasDied = true;
        }
    }

    public void Recolect(ItemSO toolUsed, InventoryManager invetory)
    {
        
        float finalMultiplier = 0f;
        if (hits <= 0)
        {
            return;
        }

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
