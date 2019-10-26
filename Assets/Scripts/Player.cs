using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float mSpeed = 1;
    public float jumpForce = 5;

    CameraFollow mCamera;
    Rigidbody2D mBody;
    [HideInInspector]public GameObject mPlanet;

    [HideInInspector]public bool grounded = false;
    float jumpOffset = 0.1f;
    Vector3 mRunVelocity;
    List<Collider2D> mGravities = new List<Collider2D>();
    // Start is called before the first frame update
    void Start()
    {
        mBody = GetComponent<Rigidbody2D>();
        mCamera = Camera.main.GetComponent<CameraFollow>();
    }
    // Update is called once per frame
    void Update()
    {
        

    }
    private void FixedUpdate()
    {
        SetMyPlanet();
        if (grounded)
        {
            Run();
        }
    }
    void Run() {
        mRunVelocity = transform.right * Input.GetAxis("Horizontal") * mSpeed ;
        transform.position += mRunVelocity * Time.fixedDeltaTime;

        if (Input.GetKeyDown("space"))
        {
            //Debug.Log("Jump!");
            transform.position += transform.up * jumpOffset;
            mBody.velocity = (transform.up * jumpForce + mRunVelocity).normalized*jumpForce;
            grounded = false;
        }
    }
    void SetMyPlanet() {
        float minDis = float.PositiveInfinity;
        foreach(Collider2D c in mGravities) {
            float dis = (c.transform.position - transform.position).magnitude;
            GameObject p = c.gameObject.transform.parent.gameObject;
            dis -= p.transform.localScale.x/2;
            if ( dis < minDis) {
                minDis = dis;
                mPlanet = c.gameObject.transform.parent.gameObject;
            }
        }

        Vector3 fromPlanet = transform.position- mPlanet.transform.position ;
        float angle = Mathf.Atan2(fromPlanet.y, fromPlanet.x) * Mathf.Rad2Deg-90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    void SnapToGround() {
        if (mPlanet == null) return;
        Vector3 dif = transform.position - mPlanet.transform.position;
        dif.Normalize();
        float groundDis = (mPlanet.transform.localScale.x + transform.localScale.y) / 2;
        transform.position = mPlanet.transform.position + dif * groundDis;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (mPlanet == collision.gameObject) {
            //Debug.Log(collision.collider.name);
            mBody.velocity = Vector3.zero;
            SnapToGround();
            grounded = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "GravityFeild") {
            mGravities.Add(other);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "GravityFeild")
        {
            mGravities.Remove(other);
        }
    }
}
