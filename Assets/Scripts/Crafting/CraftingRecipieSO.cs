using UnityEngine;
using UnityEngine.UI;
using static ItemSO;

[CreateAssetMenu(fileName = "New Recipie", menuName = "The Island/Crafting/Recipie")]
public class CraftingRecipieSO : ScriptableObject
{

    [Header("General")]

    public Sprite recipieIcon;
    public string recipieName;

    public CraftingRequirements[] requirements;

    [Space]
    public float craftingTime;
    public ItemSO output;
    public int outputAmount = 1;

}
