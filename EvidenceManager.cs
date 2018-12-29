using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EvidenceManager : MonoBehaviour {
    public Evidence[] evidence = new Evidence[20];
    public int numElements;
    public int i;
    public Button next;
    public Button back;
    public Button present;
    public DialogueManager dm;
    public Evidence currentEvidence;

    public static bool isTrial;
    public bool investigation;

    private int q;

    public GameObject indestructable;



    private void OnEnable()
    {
        if (!investigation)
        {
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

    public void UpdateInfo()
    {
        Text displayText = transform.Find("Evidence Description").GetComponent<Text>();
        Text name = transform.Find("Name of Evidence").GetComponent<Text>();
        Image displayImage = transform.Find("Image").GetComponent<Image>();

        displayText.text = evidence[i].itemDescription;
        name.text = evidence[i].itemName;
        displayImage.sprite = evidence[i].itemImage;
    }

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

    public void Present()
    {
        dm.openInventory();
        dm.checkEvidence();
    }

    public void add(Evidence e)
    {
        evidence[numElements++] = e;
    }

    public void remove(string name)
    {
        for (int i = 0; i < numElements; i++)
        {
            if (string.Equals(evidence[i].itemName, name, StringComparison.OrdinalIgnoreCase))
            {
                for (int j = i; j < numElements; j++)
                {
                    evidence[j] = evidence[j + 1];
                }
                numElements--;
                return;
            }
        }
    }
    public bool search(string name)
    {
        for (int i = 0; i < numElements; i++)
        {
            if (string.Equals(evidence[i].itemName, name, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public void saveInventory()
    {
        GlobalInventory.Instance.evidences = evidence;
    }

    void OnDestroy()
    {
        saveInventory();
    }
}
