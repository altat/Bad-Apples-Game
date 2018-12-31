using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class DialogueParser : MonoBehaviour
{
    public bool investigation;
    public string file;
    public string character;
    public bool speaking;
    public int numLines;
    public bool key;
    public DialogueLine lineEntry;
    public DialogueManager dm;
    public EvidenceManager em;

    /* This script is based on a tutorial about making visual novels which can be found here: http://www.indiana.edu/~gamedev/2015/09/27/creating-a-visual-novel-in-unity/
    Dialogue Parser takes a text file and places each line into a List.
    */

    /*  
     * DialogueLine represents a line of text in a dialogue text file and the information each line should contain. 
     * The text file's lines should be formatted as name;content;pose;evidence
     */
    public struct DialogueLine
    {
        public string name;
        public string content;
        public int pose;
        public string position;
        public string[] options;
        public string evidence;

        public DialogueLine(string Name, string Content, int Pose, string Position, string Evidence)
        {
            name = Name;
            content = Content;
            pose = Pose;
            position = Position;
            options = new string[0];
            evidence = Evidence;
        }
    }

    public List<DialogueLine> lines;

    // Use this for initialization
    void Start()
    {
        numLines = 0;

        // if not an investigation scene, then search for the corresponding text file for this scene.
        // The scene needs to be named "Scene#" and the text file also needs to be named "Dialogue#"
        if (!investigation)
        {
            file = "Dialogue";
            string sceneNum = SceneManager.GetActiveScene().name;
            sceneNum = Regex.Replace(sceneNum, "[^0-9]", "");
            file += sceneNum;
            //file += ".txt";

         

            lines = new List<DialogueLine>();

            // creates a TextAsset with the contents of the text file
            TextAsset txt = (TextAsset)Resources.Load(file, typeof(TextAsset));
            string newFile = txt.text;

            // places each line into the "lines" List
            LoadDialogue(newFile);
        }

    }

    // Update is called once per frame
    void Update()
    {
        setKey();
   
        // if an investigation scene and the mouse was clicked while not speaking or not looking at the inventory
        if (Input.GetMouseButtonDown(0) && investigation && !speaking && !dm.inventoryOpen)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
            RaycastHit hit;

            numLines = 0;

            if (Physics.Raycast(ray, out hit, 100))
            {
                // if the object in the middle of the screen is tagged with evidence
                if (hit.transform.gameObject.tag == "evidence")
                {
                    // find a text file with a name that matches the object
                    speaking = true;
                    character = hit.transform.gameObject.name;
                    file = character;

                    lines = new List<DialogueLine>();

                    TextAsset txt = (TextAsset)Resources.Load(file, typeof(TextAsset));
                    string newFile = txt.text;

                    LoadDialogue(newFile);
                    // setting key to true emulates pressing 'space' or 'return'
                    key = true;
                }
            }
        }
    }

    /// <summary> 
    /// Separates a wall of text by line, creates new DialogueLines for each line, and adds each DialogueLine to the 'lines' List 
    /// </summary> 
    /// <param name="text">The text in the dialogue text file.</param> 
    void LoadDialogue(string text)
    {
        string line;
        int index = 0;

        // places each line of text into an array
        string[] result = text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            do
            {
            line = result[index];
                if (line != null)
                {
                    // split the line at ';' and place the tokens into an array
                    string[] lineData = line.Split(';');
                    numLines++;

                    // if the first word in the line is 'Button'
                    if (lineData[0] == "Button")
                    {
                        // fill in the options
                        lineEntry = new DialogueLine(lineData[0], "", 0, "", "");
                        lineEntry.options = new string[lineData.Length - 1];
                        for (int i = 1; i < lineData.Length; i++)
                        {
                            lineEntry.options[i - 1] = lineData[i];
                        }
                        lines.Add(lineEntry);
                    }
                    else
                    {
                        lineEntry = new DialogueLine(lineData[0], lineData[1], int.Parse(lineData[2]), lineData[3], lineData[4]);
                        lines.Add(lineEntry);
                    }
                }
            index++;
            }
            while (index < result.Length);
        
    }

    public string GetPosition(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].position;
        }
        return "";
    }

    public string GetName(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].name;
        }
        return "";
    }

    public string GetContent(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].content;
        }
        return "";
    }

    public int GetPose(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].pose;
        }
        return 0;
    }

    public string[] GetOptions(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].options;
        }
        return new string[0];
    }

    public string GetEvidence(int lineNumber)
    {
        if (lineNumber < lines.Count)
        {
            return lines[lineNumber].evidence;
        }
        return "";
    }

    /*
     * setKey sets key to true if the space or return key is pressed and sets it to false otherwise.
     */
    public void setKey()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            key = true;
        }
        else
        {
            key = false;
        }
    }
}
