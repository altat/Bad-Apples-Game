using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ARFC;
using System;

public class DialogueManager : MonoBehaviour
{

    public DialogueParser parser;

    public string dialogue, characterName;
    public int lineNum;
    int pose;
    string position;
    string[] options;
    public bool buttonTalking;
    List<Button> buttons = new List<Button>();
    string evidence;

    public Text dialogueBox;
    public Text nameBox;
    public GameObject choiceBox;

    public GameObject ip;
    public bool inventoryOpen;

    public GameObject character;
    public GameObject model;

    public bool progress;
    public EvidenceManager em;

    public int Testimony;
    public int[] testimonies;

    public int endTestimony;
    public int[] completeTestimonies;

    public bool hasRead;

    public Button objection;

    public string evidenceName;
    public Evidence[] evidenceList;

    public GameObject crosshair;

    public Button present;
    public bool presented;

    public GameObject namePanel;
    public GameObject dialoguePanel;

    public int numEvidence;
    public int maxEvidence;

    public FPController fp;

    public Button next;
    public Button previous;

    public int once;

    public int lives;

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;


    // Use this for initialization
    void Start()
    {
        dialogue = "";
        characterName = "";
        pose = 0;
        position = "L";
        evidence = "";
        buttonTalking = false;
        parser = GameObject.Find("DialogueParser").GetComponent<DialogueParser>();
        lineNum = -1;
        progress = false;

        inventoryOpen = false;

        Testimony = 0;
        testimonies = new int[20];

        endTestimony = 0;
        completeTestimonies = new int[20];

        objection.onClick.AddListener(Objection);
        present.onClick.AddListener(presentEvidence);

        numEvidence = 0;

        once = 0;

        lives = 3;

        if (parser.investigation)
        {
            objection.gameObject.SetActive(false);
            Cursor.visible = false;
            crosshair.SetActive(true);
        }
        else
        {
            Cursor.visible = true;
            crosshair.SetActive(false);
            objection.gameObject.SetActive(true);
            hasRead = false;
            next.onClick.AddListener(Next);
            previous.onClick.AddListener(Previous);
            
        }

        namePanel.SetActive(false);
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!parser.investigation)
        {
            if (once == 0)
            {
                parser.key = true;
                once++;
            }
            else
            {
                if (once == 1)
                {
                    parser.key = false;
                    once++;
                }
            }

            if (parser.GetEvidence(lineNum + 1) == "et") hasRead = true;
        }
        if (parser.key && buttonTalking == false && inventoryOpen == false && parser.speaking)
        {
            lineNum++;
            ShowDialogue();

            if (parser.investigation)
                fp.Constraints.Control = false;
        }

        if (!hasRead)
        {
            objection.interactable = false;
        }

        if (hasRead)
        {
            objection.interactable = true;
        }

        if (lineNum > 0)
        {
            if (parser.GetEvidence(lineNum - 1) == "et") hasRead = false;
        }

        UpdateUI();

        if (lineNum == parser.numLines && parser.investigation)
        {
            namePanel.SetActive(false);
            dialoguePanel.SetActive(false);

            parser.speaking = false;
            lineNum = -1;
            fp.Constraints.Control = true;
        }

        if (buttonTalking || inventoryOpen)
        {
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
        else
        {
            if (!parser.speaking)
            {
                Cursor.visible = false;
                crosshair.SetActive(true);
            }
        }

        if (parser.investigation && !parser.speaking)
        {
            if (numEvidence > maxEvidence - 1)
            {
                StartCoroutine(waiter());
            }
        }

        openInventory();
    }

    public void ShowDialogue()
    {
        ResetImages();
        ParseLine();
    }

    void UpdateUI()
    {
        if (!buttonTalking)
        {
            ClearButtons();
        }
        dialogueBox.text = dialogue;
        nameBox.text = characterName;
    }

    void ClearButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            print("Clearing buttons");
            Button b = buttons[i];
            buttons.Remove(b);
            Destroy(b.gameObject);
        }
    }

    void ParseLine()
    {
        if (parser.GetName(lineNum) == "Line")
        { 
            int newLine = parser.GetPose(lineNum);
            lineNum = newLine;
            ParseLine();
        }
        else
        {
            if (parser.GetName(lineNum) != "Button")
            {
                buttonTalking = false;
                namePanel.SetActive(true);
                dialoguePanel.SetActive(true);
                characterName = parser.GetName(lineNum);
                dialogue = parser.GetContent(lineNum);
                pose = parser.GetPose(lineNum);
                position = parser.GetPosition(lineNum);
                evidence = parser.GetEvidence(lineNum);
                if (parser.GetEvidence(lineNum + 1) == "t")
                {
                    testimonies[Testimony] = lineNum + 1;
                    Testimony++;
                    progress = false;
                    hasRead = false;
                   
                }
                else if (parser.GetEvidence(lineNum + 1) == "et")
                {
                    completeTestimonies[endTestimony] = lineNum + 1;
                    endTestimony++;
                    hasRead = true;
                } 
                else if (evidence == "et")
                {
                    resetTestimoney(1);
                }
                else if (parser.investigation)
                {
                    evidenceName = parser.GetEvidence(lineNum);
                    addToInventory(evidenceName);
                }

                DisplayImages();
            }
            else
            {
                buttonTalking = true;
                characterName = "";
                dialogue = "";
                pose = 0;
                position = "";
                evidence = "";
                options = parser.GetOptions(lineNum);

                namePanel.SetActive(false);
                dialoguePanel.SetActive(false);

                CreateButtons();
            }
        }
    }

    void CreateButtons()
    {
        for (int i = 0; i < options.Length; i++)
        {
            GameObject button = (GameObject)Instantiate(choiceBox);
            Button b = button.GetComponent<Button>();
            ChoiceButton cb = button.GetComponent<ChoiceButton>();
            cb.SetText(options[i].Split(':')[0]);
            cb.option = options[i].Split(':')[1];
            cb.box = this;
            b.transform.SetParent(this.transform);
            b.transform.localPosition = new Vector3(0, -25 + (i * 100));
            b.transform.localScale = new Vector3(1, 1, 1);
            buttons.Add(b);
        }
    }

    void ResetImages()
    {
        if (characterName != "" && !parser.investigation)
        {
            //character = GameObject.Find(characterName);
            //SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();
            //currSprite.sprite = null;
            Destroy(model);
        }
    }

    void DisplayImages()
    {
        if (characterName != "" && !characterName.Equals("Random Guy") && !parser.investigation)
        {
            character = GameObject.Find(characterName);

            //SetSpritePositions(character);

            //SpriteRenderer currSprite = character.GetComponent<SpriteRenderer>();

            model = character.GetComponent<Character>().characterPoses[pose];

            SetSpritePositions(model);

            model = Instantiate(model);

            //currSprite.sprite = character.GetComponent<Character>().characterPoses[pose];
        }
    }


    void SetSpritePositions(GameObject spriteObj)
    {
        if (position == "L")
        {
            spriteObj.transform.position = new Vector3(960, 420);
        }
        else if (position == "R")
        {
            spriteObj.transform.position = new Vector3(960, 420);
        }
        else if (position == "M")
        {
            spriteObj.transform.position = new Vector3(960, 420);
        }
        spriteObj.transform.position = new Vector3(spriteObj.transform.position.x, spriteObj.transform.position.y, 200);
        spriteObj.transform.localScale = new Vector3(1, 1, 1);
        if (characterName == "Rob")
            spriteObj.transform.localScale = new Vector3(75, 75, 75);
        spriteObj.transform.eulerAngles = new Vector3(0, 180);
    }

    public void openInventory()
    {
        if ((Input.GetKeyDown(KeyCode.I) && hasRead) || presented)
        {
            if (!ip.activeSelf)
            {
                inventoryOpen = true;
                ip.SetActive(true);

                if(parser.investigation)
                    fp.Constraints.Control = false;
            }
            else
            {
                ip.SetActive(false);
                inventoryOpen = false;
                presented = false;

                if (parser.investigation && !parser.speaking)
                    fp.Constraints.Control = true;
            }
        }
    }

    public void checkEvidence()
    {
        evidence = parser.GetEvidence(lineNum);
        string playerEvidence = em.currentEvidence.itemName;
        ResetImages();

        if (evidence.Equals(playerEvidence))
        {
            buttonTalking = false;
            progress = true;
            dialogue = "Huh?!";
            DisplayImages();
            UpdateUI();
            lives = 3;
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);
        }
        else
        {
            buttonTalking = false;
            characterName = "Judge Justice";
            dialogue = "What does that have to do with anything?";
            pose = 0;
            position = "M";
            evidence = "";
            UpdateUI();

            lives--;
            if (lives == 2) heart1.SetActive(false);
            else if (lives == 1) heart2.SetActive(false);
            else if (lives == 0)
            {
                heart3.SetActive(false);
                GameOver();
            }
        }

        resetTestimoney(0);
    }

    public void resetTestimoney(int offSet)
    {
        if (!progress)
        {
            lineNum = testimonies[Testimony - 1] - offSet;
        }
        else
        {
            lineNum = completeTestimonies[endTestimony - 1];
        }
    }

    public void Objection()
    {
        ResetImages();

        if (evidence == "c")
        {
            buttonTalking = false;
            progress = true;
            dialogue = "Huh?!";
            DisplayImages();
            UpdateUI();
            resetTestimoney(1);
        }
        else if (evidence != null)
        {
            inventoryOpen = true;
            ip.SetActive(true);
        }
        else
        {
            buttonTalking = false;
            characterName = "Judge Justice";
            dialogue = "What's wrong with what they said?";
            pose = 0;
            position = "M";
            evidence = "";
            UpdateUI();
        }
    }

    public void presentEvidence()
    {
        presented = true;
    }

    public void Next()
    {
        if (lineNum < parser.numLines - 1)
        {
            lineNum++;
            ShowDialogue();
            namePanel.SetActive(true);
            dialoguePanel.SetActive(true);
        }
    }

    public void Previous()
    {
        if (lineNum == 0 || evidence == "t" || parser.GetEvidence(lineNum -1) == "et")
        {
            ShowDialogue();
            namePanel.SetActive(true);
            dialoguePanel.SetActive(true);

        }
        else
        {
            lineNum--;
            ShowDialogue();
            namePanel.SetActive(true);
            dialoguePanel.SetActive(true);
        }
    }

    public void addEvidence(Evidence e)
    {
        if (!em.search(e.itemName))
        {
            em.add(e);
            numEvidence++;
        }
    }

    public void addToInventory(string name)
    {
        for (int i = 0; i < evidenceList.Length; i++)
        {
            if (string.Equals(evidenceList[i].name, name, StringComparison.OrdinalIgnoreCase))
            {
                addEvidence(evidenceList[i]);
            }
        }
    }

    IEnumerator waiter()
    {
        characterName = "Pat";
        dialogue = "Looks like I've found all the evidence in this house. Time to head over to the courtroom.";
        UpdateUI();
        Cursor.visible = true;

        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("Choose");
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Scene5");
    }
}


        
