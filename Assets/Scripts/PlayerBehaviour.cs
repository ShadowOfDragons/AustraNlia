using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{

    [Header("Movement")]
    public float speed;
    public GameObject point;
    private SpriteRenderer sprite;
    public bool isFacingLeft = true;
    private Vector3 target;

    // Use this for initialization
    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        target = point.transform.position;

        if ((target.x < transform.position.x) && (isFacingLeft == false)) Flip();
        else if ((target.x > transform.position.x) && (isFacingLeft == true)) Flip();

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (this.gameObject.tag == "Player") sprite.sortingOrder = 1;
    }

    void Flip()
    {
        sprite.flipX = !sprite.flipX;
        isFacingLeft = !isFacingLeft;
    }
}
