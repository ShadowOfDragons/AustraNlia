using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public GameObject[] characters;
    private GameObject currentCharacter;
    private int charactersIndex;

	// Use this for initialization
	void Start ()
    {
        charactersIndex = 0;
        currentCharacter = characters[0];
	}
	
    public void Change()
    {
        charactersIndex++;
        if (charactersIndex == characters.Length) charactersIndex = 0;

        currentCharacter.GetComponent<PlayerBehaviour>().enabled = false;
        currentCharacter.tag = "Follower";
        currentCharacter.GetComponent<FollowerBehaviour>().enabled = true;
        currentCharacter.GetComponent<BoxCollider2D>().enabled = true;

        characters[charactersIndex].GetComponent<PlayerBehaviour>().enabled = true;
        characters[charactersIndex].tag = "Player";
        characters[charactersIndex].GetComponent<FollowerBehaviour>().enabled = false;
        characters[charactersIndex].GetComponent<BoxCollider2D>().enabled = false;

        currentCharacter = characters[charactersIndex];
    }
}
