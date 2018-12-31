using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * EvidenceManager displays the evidence in the inventory
 */
public class EvidenceManager : MonoBehaviour {
    public Evidence[] evidence = new Evidence[20];
    public int numElements;
    public int i;
    public Button next;
    public Button back;
    public Button present;
    public DialogueManager dm;
    public Evidence currentEvidence;

    public bool investigation;


    private void OnEnable()
    {
        if (!investigation)
        {
            // if not an investigation, load the global inventory
            evidence = GlobalInventory.Instance.evidences;
        } 
    }

    private void Start()
    {
        numElements = 0;
        i = 0;
        UpdateInfo();
        next.onClick.AddListener(Next);
        back.onClick.AddListener(Back);
        present.onClick.AddListener(delegate { Present(); });
        dm = GameObject.Find("Panel").GetComponent<DialogueManager>();
        currentEvidence = null;

        if (investigation)
        {
            present.gameObject.SetActive(false);
        }

        // count the number of items in the inventory
        for (int j = 0; j < evidence.Length; j++)
        {
            if (evidence[j] != null)
            {
                numElements++;
            }
        }

    }

    private void Update()
    {
        UpdateInfo();
        currentEvidence = evidence[i];
    }

    /// <summary> 
    /// Updates the information in the inventory
    /// </summary> 
    public void UpdateInfo()
    {
        Text displayText = transform.Find("Evidence Description").GetComponent<Text>();
        Text name = transform.Find("Name of Evidence").GetComponent<Text>();
        Image displayImage = transform.Find("Image").GetComponent<Image>();

        displayText.text = evidence[i].itemDescription;
        name.text = evidence[i].itemName;
        displayImage.sprite = evidence[i].itemImage;
    }

    /// <summary> 
    /// Causes the next item in the inventory to be loaded
    /// </summary> 
    public void Next()
    {
        if (i < numElements - 1)
        {
            i++;
        }
        else
        {
            i = 0;
        }
    }

    /// <summary> 
    /// Causes the previous item in the inventory to be loaded
    /// </summary> 
    public void Back()
    {
        if (i > 0)
        {
            i--;
        }
        else
        {
            i = numElements - 1;
        }
    }

    /// <summary> 
    /// Presents the current evidence to the court and checks if the evidence is relevant to the case. Also closes the inventory if open.
    /// </summary> 
    public void Present()
    {
        dm.openInventory();
        dm.checkEvidence();
    }

    /// <summary> 
    /// adds an evidence to the inventory
    /// </summary> 
    public void add(Evidence e)
    {
        evidence[numElements++] = e;
    }

    /// <summary> 
    /// removes an evidence from the inventory
    /// </summary> 
    public void remove(string name)
    {
        for (int i = 0; i < numElements; i++)
        {
            if (string.Equals(evidence[i].itemName, name, StringComparison.OrdinalIgnoreCase))
            {
                // if the evidence is found, then shift all subsequent items to the left
                for (int j = i; j < numElements; j++)
                {
                    evidence[j] = evidence[j + 1];
                }
                numElements--;
                return;
            }
        }
    }

    /// <summary> 
    /// searches for an evidence in the inventory by name and returns true if found
    /// </summary> 
    public bool search(string name)
    {
        for (int i = 0; i < numElements; i++)
        {
            if (string.Equals(evidence[i].itemName, name, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    /// <summary> 
    /// writes the evidence in the local inventory to the global inventory
    /// </summary> 
    public void saveInventory()
    {
        if (investigation) GlobalInventory.Instance.evidences = evidence;
    }

    // triggered right before a new scene is loaded
    void OnDestroy()
    {
        saveInventory();
    }
}
