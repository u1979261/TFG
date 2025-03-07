using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    public bool foundation; 
    public List<Collider> col = new List<Collider>();
    public Material green;
    public Material red;
    public bool IsBuildable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 9 && foundation)
        {
            col.Add(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9 && foundation)
        {
            col.Remove(other);
        }
    }

    private void Update()
    {
        ChangeColor();
    }
    public void ChangeColor()
    {
        if(col.Count == 0)
        {
            IsBuildable = true;
        }
        else
        {
            IsBuildable = false;
        }
        if (IsBuildable)
        {
            foreach (Transform child in this.transform)
            {
                child.GetComponent<Renderer>().material = green;
            }
        }
        else
        {
            foreach (Transform child in this.transform)
            {
                child.GetComponent<Renderer>().material = red;
            }
        }
    }
}
