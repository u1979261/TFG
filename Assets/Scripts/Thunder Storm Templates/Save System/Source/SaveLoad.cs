using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    private SaveHandler saveHandler;

    private void Start()
    {
        saveHandler = GetComponent<SaveHandler>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            saveHandler.Save();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            saveHandler.Load();
        }
    }
}
