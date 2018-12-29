using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour {

    public GameObject[] Characters;
    public Transform PlayerSpawnPoint;
    void Start()
    {
        for(int i = 0; i < Characters.Length; i++)
            Instantiate(Characters[i], PlayerSpawnPoint.position, PlayerSpawnPoint.rotation);
    }
}

