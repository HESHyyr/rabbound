using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mSpeed = 1;
    Rigidbody2D mBody;
    GameObject mPlanet;
    // Start is called before the first frame update
    void Start()
    {
        mBody = GetComponent<Rigidbody2D>();   
    }
    // Update is called once per frame
    void Update()
    {
        Move();
    }
    void Move() {
        float move = Input.GetAxis("Horizontal") * Time.deltaTime * mSpeed;
        Vector3 movement = transform.right * move;
        if (movement.magnitude > 0)
        {
            Debug.Log(transform.right);
        }
        transform.Translate(movement);
    }
    //void 
}
