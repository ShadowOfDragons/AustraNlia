using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private BoxCollider2D cameraBox; //Camera collider
    private Transform player; //Player position
    public BoxCollider2D boundaryBox; //Box Limit

    private float cameraWidth;
    private float cameraHeight;

    // Use this for initialization
    void Start()
    {
        cameraBox = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        AspectRatioBoxchange();
        FollowPlayer();
    }

    void AspectRatioBoxchange() //Projection rectangle for the size of the camera
    {
        cameraHeight = 2 * Camera.main.orthographicSize;
        cameraWidth = cameraHeight * Camera.main.aspect;
        cameraBox.size = new Vector2(cameraWidth, cameraHeight);
    }

    void FollowPlayer()
    {
        if (GameObject.Find("Boundary"))
        {
            transform.position = new Vector3(Mathf.Clamp(player.position.x, (boundaryBox.bounds.min.x + (cameraBox.size.x / 2)), (boundaryBox.bounds.max.x - (cameraBox.size.x / 2))),
                                             Mathf.Clamp(player.position.y, (boundaryBox.bounds.min.y + (cameraBox.size.y / 2)), (boundaryBox.bounds.max.y - (cameraBox.size.y / 2))),
                                             transform.position.z);
        }
    }
}
