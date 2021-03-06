﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* This script is based on a tutorial about making visual novels which can be found here: http://www.indiana.edu/~gamedev/2015/09/27/creating-a-visual-novel-in-unity/
*/

public class ChoiceButton : MonoBehaviour
{

    public string option;
    public DialogueManager box;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string newText)
    {
        this.GetComponentInChildren<Text>().text = newText;
    }

    public void SetOption(string newOption)
    {
        this.option = newOption;
    }

    public void ParseOption()
    {
        string command = option.Split(',')[0];
        string commandModifier = option.Split(',')[1];
        box.buttonTalking = false;
        if (command == "line")
        {
            box.lineNum = int.Parse(commandModifier);
            box.ShowDialogue();
        }
        else if (command == "scene" || command == "Scene")
        {
            SceneManager.LoadScene(commandModifier);
        }
    }
}
