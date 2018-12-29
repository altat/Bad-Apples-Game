using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour {

    public static string level;
    public void CharacterSelectFunction(string choice)
    {
        level = choice;
        Application.LoadLevel(level);
    }
}
