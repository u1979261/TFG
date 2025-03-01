using UnityEngine;

[CreateAssetMenu (fileName = "New Resource Data", menuName = "The Island/Resources/Resource Data")]
public class ResourceDataSO : ScriptableObject
{
    public ItemSO items;
    public int amount;
}
