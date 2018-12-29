using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public GameObject panel;
    void Update() {
	    if ((Input.GetKeyDown(KeyCode.I)))
        {
            if (panel.active)
            {
                panel.SetActive(false);
            }
            else
            {
                panel.SetActive(true);
            }
        }
    }
}
