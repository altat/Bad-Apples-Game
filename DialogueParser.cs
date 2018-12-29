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
        if (!investigation)
        {
            file = "Dialogue";
            string sceneNum = SceneManager.GetActiveScene().name;
            sceneNum = Regex.Replace(sceneNum, "[^0-9]", "");
            file += sceneNum;
            //file += ".txt";

         

            lines = new List<DialogueLine>();

            TextAsset txt = (TextAsset)Resources.Load(file, typeof(TextAsset));
            string newFile = txt.text;

            LoadDialogue(newFile);
        }

    }

    // Update is called once per frame
    void Update()
    {
        getKey();
   
        if (Input.GetMouseButtonDown(0) && investigation && !speaking && !dm.inventoryOpen)
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height/2, 0));
            RaycastHit hit;

            numLines = 0;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.gameObject.tag == "evidence")
                {
                    speaking = true;
                    character = hit.transform.gameObject.name;
                    file = character;

                    lines = new List<DialogueLine>();

                    TextAsset txt = (TextAsset)Resources.Load(file, typeof(TextAsset));
                    string newFile = txt.text;

                    LoadDialogue(newFile);
                    key = true;
                }
            }
        }
    }

    void LoadDialogue(string filename)
    {
        string line;
        int index = 0;

        string[] result = filename.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            do
            {
            line = result[index];
                if (line != null)
                {
                    string[] lineData = line.Split(';');
                    numLines++;
                    if (lineData[0] == "Button")
                    {
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

    public void getKey()
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
