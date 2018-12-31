using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ARFC;
using System;
/*
 * This script is based on a tutorial about making visual novels which can be found here: http://www.indiana.edu/~gamedev/2015/09/27/creating-a-visual-novel-in-unity/
 * DialogueManager displays the dialogue and other UI elements in the game. It also adds evidence to the inventory during investigation scenes and it loops testimonies
 * during the court scenes.
 */

//TODO Remove unnecessary code, encapsulate, and reorganize code. Add better documentation.

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

        // hide the cursor and objection button during the investigation and show the crosshair
        if (parser.investigation)
        {
            objection.gameObject.SetActive(false);
            Cursor.visible = false;
            crosshair.SetActive(true);
        }

        // show everything except the crosshair during court scenes
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
                // if not an investigation, then emulate a 'space' key press once
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

            // if the next line's evidence is "et", that means the end of a testimony has been reached.
            // This means that the testimony has been read at least once.
            if (parser.GetEvidence(lineNum + 1) == "et") hasRead = true;
        }

        // if the space key has been pressed while speaking
        if (parser.key && buttonTalking == false && inventoryOpen == false && parser.speaking)
        {
            // display the dialogue on the next line
            lineNum++;
            ShowDialogue();

            // turn off the controls if its an investigation scene
            if (parser.investigation)
                fp.Constraints.Control = false;
        }

        // if a testimony hasn't been read at least once, turn off the objection button
        if (!hasRead)
        {
            objection.interactable = false;
        }
        else 
        {
            objection.interactable = true;
        }

        if (lineNum > 0)
        {
            // if the previous line's evidence is "et" then the testimony is over
            if (parser.GetEvidence(lineNum - 1) == "et") hasRead = false;
        }

        UpdateUI();

        // if the current line number is equal to the total number of lines in the dialogue file, then the end of the dialogue has been reached.
        // if the scene is an investigation
        if (lineNum == parser.numLines && parser.investigation)
        {
            // hide the dialogue UI and turn player controls on
            namePanel.SetActive(false);
            dialoguePanel.SetActive(false);

            parser.speaking = false;
            lineNum = -1;
            fp.Constraints.Control = true;
        }

        // if there are buttons on the screen or the inventory is open
        if (buttonTalking || inventoryOpen)
        {
            // show the cursor and hide the crosshair
            Cursor.visible = true;
            crosshair.SetActive(false);
        }
        else
        {
            if (!parser.speaking)
            {
                // hide the cursor while not speaking
                Cursor.visible = false;
                crosshair.SetActive(true);
            }
        }

        if (parser.investigation && !parser.speaking)
        {
            // if the number of evidences collected is equal to the number of evidences to collect, then load the next scene
            if (numEvidence == maxEvidence)
            {
                StartCoroutine(waiter());
            }
        }

        // check to see if the player tries to open the inventory
        openInventory();
    }

    /// <summary> 
    /// Clears the screen and parses the next line
    /// </summary> 
    public void ShowDialogue()
    {
        ResetImages();
        ParseLine();
    }

    /// <summary> 
    /// Sets the text in the game
    /// </summary> 
    void UpdateUI()
    {
        if (!buttonTalking)
        {
            ClearButtons();
        }
        dialogueBox.text = dialogue;
        nameBox.text = characterName;
    }

    /// <summary> 
    /// Destroys temporary UI buttons
    /// </summary> 
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

    /// <summary> 
    /// Parses the DialogueLines in DialogueParser
    /// </summary> 
    void ParseLine()
    {
        // if the DialogueLine's name is "Line" then set the DialogueLine's pose to lineNum and parse again
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

                // if the evidence on the next line is "t", then it indicates that the next line is the beginning of a testimony
                if (parser.GetEvidence(lineNum + 1) == "t")
                {
                    // record the line number
                    Testimony = lineNum + 1;
                    progress = false;
                    hasRead = false;
                   
                }

                // if the evidence on the next line is "et", then it indicates the end of a testimony
                else if (parser.GetEvidence(lineNum + 1) == "et")
                {
                    // record the line number and set hasRead to true to indicate that the testimony has been read at least once
                    endTestimony = lineNum + 1;
                    hasRead = true;
                } 

                // if the current line's evidence is "et", then reset the testimony (go back to the beginning of the testimony)
                else if (evidence == "et")
                {
                    resetTestimony(1);
                }

                // if an investigation, then add the evidence to the inventory
                else if (parser.investigation)
                {
                    evidenceName = parser.GetEvidence(lineNum);
                    addToInventory(evidenceName);
                }

                DisplayImages();
            }

            // if the name is "Button" then create buttons and don't display any dialogue
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

    /// <summary> 
    /// Creates temporary buttons 
    /// </summary> 
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

    /// <summary> 
    /// Clears characters models during courtroom scenes
    /// </summary> 
    void ResetImages()
    {
        if (characterName != "" && !parser.investigation)
        {
            Destroy(model);
        }
    }

    /// <summary> 
    /// Instantiates character models during courtroom scenes
    /// </summary> 
    void DisplayImages()
    {
        if (characterName != "" && !characterName.Equals("Random Guy") && !parser.investigation)
        {
            character = GameObject.Find(characterName);

            model = character.GetComponent<Character>().characterPoses[pose];

            SetSpritePositions(model);

            model = Instantiate(model);

        }
    }

    /// <summary> 
    /// Sets the positions of character models onscreen.
    /// </summary> 
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
        spriteObj.transform.eulerAngles = new Vector3(0, 180);
    }

    /// <summary> 
    /// Opens the inventory if it is closed and closes the inventory if it is open
    /// </summary> 
    public void openInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) || presented)
        {
            if (!ip.activeSelf)
            {
                inventoryOpen = true;
                ip.SetActive(true);

                // turns off player controls
                if(parser.investigation)
                    fp.Constraints.Control = false;
            }
            else
            {
                ip.SetActive(false);
                inventoryOpen = false;
                presented = false;

                // turns on player controls
                if (parser.investigation && !parser.speaking)
                    fp.Constraints.Control = true;
            }
        }
    }

    /// <summary> 
    /// Checks if the evidence presented matches with the evidence on the current line
    /// </summary> 
    public void checkEvidence()
    {
        evidence = parser.GetEvidence(lineNum);
        string playerEvidence = em.currentEvidence.itemName;
        ResetImages();

        if (evidence.Equals(playerEvidence))
        {
            buttonTalking = false;
            progress = true;
            em.remove(playerEvidence);
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
            DisplayImages();
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

        resetTestimony(0);
    }

    /// <summary> 
    /// resetTestimony either goes back to the beginning of the testimony or it goes to the end of the testimony
    /// </summary> 
    public void resetTestimony(int offSet)
    {
        // progress indicates whether the player has successfully objectioned (identified a contradiction in) a testimony
        // if progress is false, then go back to the beginning
        if (!progress)
        {
            lineNum = Testimony - offSet;
        }
        else
        {
            lineNum = endTestimony;
        }
    }

    /// <summary> 
    /// Objection either opens the inventory so the player can present evidence, or it takes you to the end of a testimony
    /// </summary> 
    public void Objection()
    {
        ResetImages();

        // if the evidence on this line is "c", then go to the end of the testimony
        if (evidence == "c")
        {
            buttonTalking = false;
            progress = true;
            dialogue = "Huh?!";
            DisplayImages();
            UpdateUI();
            resetTestimony(1);
        }

        // otherwise open the inventory
        else
        {
            inventoryOpen = true;
            ip.SetActive(true);
        }
    }

    public void presentEvidence()
    {
        presented = true;
    }

    /// <summary> 
    /// Next loads the next DialogueLine
    /// </summary> 
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

    /// <summary> 
    /// Previous loads the previous DialogueLine
    /// </summary> 
    public void Previous()
    {
        // if the first line, beginning of a testimony, or the line after the end of a testimony, then do NOT load the previous line
        if (lineNum == 0 || evidence == "t" || parser.GetEvidence(lineNum - 1) == "et")
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

    /// <summary> 
    /// Searches to see if the evidence already exists in the inventory. If it does not already exist, then it adds the evidence to EvidenceManager's inventory.
    /// </summary> 
    public void addEvidence(Evidence e)
    {
        if (!em.search(e.itemName))
        {
            em.add(e);
            numEvidence++;
        }
    }

    /// <summary> 
    /// Checks to see if the evidence exists in the investigation and adds it if it does exist. Does nothing if it does not exist.
    /// </summary> 
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

    /// <summary> 
    /// waiter displays some text on the screen, waits 3 seconds, and then loads a scene called "choose"
    /// </summary> 
    IEnumerator waiter()
    {
        characterName = "Pat";
        dialogue = "Looks like I've found all the evidence in this house. Time to head over to the courtroom.";
        UpdateUI();
        Cursor.visible = true;

        yield return new WaitForSeconds(3);

        SceneManager.LoadScene("Choose");
    }

    /// <summary> 
    /// GameOver loads the gameover scene
    /// </summary> 
    public void GameOver()
    {
        SceneManager.LoadScene("Scene5");
    }
}


        
