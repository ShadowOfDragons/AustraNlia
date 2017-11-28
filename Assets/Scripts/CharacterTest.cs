using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTest : MonoBehaviour
{

    public float speed;
    public GameObject point;
    private CharacterTest player;
    private Vector3 target;


	// Use this for initialization
	void Start ()
    {
        player = GetComponent<CharacterTest>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        target = point.transform.position;
        if(this.gameObject.tag == "Player")
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else if(this.gameObject.tag == "Follower")
        {
            target = GameObject.FindGameObjectWithTag("Player").transform.position;
            target.x = target.x + 2;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

	}

    public void CharacterChange()
    {
        if(this.gameObject.tag == "Player")
        {
            this.gameObject.tag = "Follower";
            player.enabled = !player.enabled;
        }
        else if (this.gameObject.tag == "Follower")
        {
            this.gameObject.tag = "Player";
            player.enabled = !player.enabled;
        }
    }
}
