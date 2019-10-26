using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * Time.deltaTime);
        if(Input.GetKey(KeyCode.S))
            transform.Translate(-Vector3.up * Time.deltaTime);
        if (Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * Time.deltaTime);
        if (Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * Time.deltaTime);
        if (Input.GetKey(KeyCode.Q))
            transform.Rotate(0, 0, 1.0f, Space.Self);
        if (Input.GetKey(KeyCode.E))
            transform.Rotate(0, 0, -1.0f, Space.Self);

    }
}
