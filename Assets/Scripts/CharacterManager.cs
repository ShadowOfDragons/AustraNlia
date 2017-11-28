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

        currentCharacter.GetComponent<CharacterTest>().enabled = false;
        currentCharacter.tag = "Follower";

        characters[charactersIndex].GetComponent<CharacterTest>().enabled = true;
        characters[charactersIndex].tag = "Player";

        currentCharacter = characters[charactersIndex];
    }
}
