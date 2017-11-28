using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTest : MonoBehaviour
{
    private Transform point;
    private Vector3 target;

    // Use this for initialization
    void Start ()
    {
        point = this.gameObject.transform;
	}

    public void UpdatePosition()
    {
        target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;

        point.position = target;
    }
}
