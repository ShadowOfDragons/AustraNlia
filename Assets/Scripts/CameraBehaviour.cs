using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    //Camera follow + smooth
    public Transform target;
    public float smoothTime;
    public Vector2 offset;
    private Vector3 velocity = Vector3.zero;

    //Camera bounds with background
    public Sprite background;
    public Vector3 cameraBounds;
    public Camera cam;
    // Use this for initialization
    void Start()
    {
        cameraBounds = background.bounds.size;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -cameraBounds.x/2, cameraBounds.x/2),
                                         Mathf.Clamp(transform.position.y, -cameraBounds.y/2, cameraBounds.y/2),
                                         transform.position.z);
    }
    void FixedUpdate()
    {
        Vector3 newPosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }
}
