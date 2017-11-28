using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTest : MonoBehaviour
{

    public float speed;
    public GameObject point;
    private Vector3 target;


	// Use this for initialization
	void Start ()
    {

	}
	
	// Update is called once per frame
	void Update ()
    {
        target = point.transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
	}
}
