using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HighlightObject : MonoBehaviour
{
    public GameObject selectedObject;
    public GameObject thisObject;
    private Shader[] holder;
    void Update()
    {
        selectedObject = GameObject.Find(HighlightSelector.selectedObject);
        if (selectedObject == thisObject)
        {
            foreach (Material t in thisObject.GetComponentInChildren<Renderer>().materials)
            {
                t.shader = Shader.Find("Outline/Silhouette Only");
            }
        }
        else
        {
                foreach (Material t in thisObject.GetComponentInChildren<Renderer>().materials)
                {
                    t.shader = Shader.Find("Standard");
                }
        }

    }
    private void SaveShader()
    {
        if (thisObject.GetComponent<Renderer>())
        {
            int i = 0;

            foreach (Material t in thisObject.GetComponentInChildren<Renderer>().materials)
            {
                holder[i] = t.shader;
                i++;
            }
        }
    }
}