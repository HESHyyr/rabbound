using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditMovement : MonoBehaviour
{
    [SerializeField]
    GameObject camera;

    private Rigidbody2D rb;
    private bool catchCamera;

    // Start is called before the first frame update
    void Start()
    {
        catchCamera = false;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y - camera.transform.position.y <= 0.0f && !catchCamera)
        {
            catchCamera = true;
            transform.parent = camera.transform;
        }
    }
}
