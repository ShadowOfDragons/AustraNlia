using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerBehaviour : MonoBehaviour
{
    [Header("Movement")]
    public float speed;
    public GameObject player;
    private SpriteRenderer sprite;
    public bool isFacingLeft = true;
    private Vector3 target;

    [Header("Follow")]
    public Vector3 followPos;
    public Vector3 followSpace;
    public ContactFilter2D filter;
    public int maxColliders = 1;

    // Use this for initialization
    void Start ()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update ()
    {
        
        target = player.transform.position;

        if ((target.x < transform.position.x) && (isFacingLeft == false)) Flip();
        else if ((target.x > transform.position.x) && (isFacingLeft == true)) Flip();

        FollowMargin();

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (this.gameObject.tag == "Follower") sprite.sortingOrder = 0;
    }

    void Flip()
    {
        sprite.flipX = !sprite.flipX;
        isFacingLeft = !isFacingLeft;
    }

    void FollowMargin()
    {
        followPos = new Vector3(player.transform.position.x, player.transform.position.y + 2, player.transform.position.z);
        Collider2D[] results = new Collider2D[maxColliders];

        int numColliders = Physics2D.OverlapBox(followPos, followSpace, 0, filter, results);

        if (numColliders > 0)
        {
            speed = 0;
        }
        else speed = 5;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(followPos, followSpace);
    } 
}
