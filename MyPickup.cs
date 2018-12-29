using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PowerGridInventory;

[RequireComponent(typeof(PGISlotItem))]

public class MyPickup : MonoBehaviour
{
    public PGIModel DefaultInventory;
    private PGISlotItem Item;

    void Awake()
    {
        Item = GetComponent<PGISlotItem>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject == gameObject)
                {
                    if (DefaultInventory != null && Item != null)
                        DefaultInventory.Pickup(Item);
                }
            }
        }
        //PowerGridInventory.SaveLoad.Save("");
    }
}
