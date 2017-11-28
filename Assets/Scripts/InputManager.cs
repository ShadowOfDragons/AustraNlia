using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CharacterManager player;
    public PointTest point;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(Input.GetMouseButtonDown(0)) point.UpdatePosition();
        if(Input.GetKeyDown(KeyCode.LeftShift)) player.Change();
	}
}
