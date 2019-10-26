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
    Transform mSprits;
    Animator mAnimator;

    [HideInInspector]public bool grounded = false;
    float jumpOffset = 0.1f;
    Vector3 mRunVelocity;
    List<Collider2D> mGravities = new List<Collider2D>();
    // Start is called before the first frame update
    void Start()
    {
        mBody = GetComponent<Rigidbody2D>();
        mCamera = Camera.main.GetComponent<CameraFollow>();
        mSprits=  transform.Find("Sprites");
        mAnimator = mSprits.gameObject.GetComponent<Animator>();
    }
    // Update is called once per frame
    void Update()
    {
        

    }
    private void FixedUpdate()
    {

        mAnimator.SetBool("Grounded", grounded);
        SetMyPlanet();
        if (grounded)
        {
            Run();
        }
        else {
            Fly();
        }
    }
    void Run() {
        float move = Input.GetAxis("Horizontal") * mSpeed;
        mRunVelocity = transform.right *  move;
        transform.position += mRunVelocity * Time.fixedDeltaTime;

        //set animator
        Vector3 newScale = mSprits.localScale;
        if (move < 0 && newScale.x > 0) {
            newScale.x *= -1;
            mSprits.localScale = newScale;
        }
        else if (move > 0 && newScale.x < 0) {
            newScale.x *= -1;
            mSprits.localScale = newScale;
        }
        mAnimator.SetFloat("Horizontal", Mathf.Abs(move));

        if (Input.GetKeyDown("space"))
        {
            //Debug.Log("Jump!");
            transform.position += transform.up * jumpOffset;
            mBody.velocity = (transform.up * jumpForce + mRunVelocity).normalized*jumpForce;
            grounded = false;
            mAnimator.SetTrigger("Jumping");
        }
    }
    void Fly() {
        float downSpeed = Vector3.Dot(mBody.velocity, -transform.up);
        downSpeed /= jumpForce;
        mAnimator.SetFloat("Vertical",downSpeed);
    }
    void SetMyPlanet() {
        float minDis = float.PositiveInfinity;
        GameObject prev = mPlanet;
        foreach(Collider2D c in mGravities) {
            float dis = (c.transform.position - transform.position).magnitude;
            GameObject p = c.gameObject.transform.parent.gameObject;
            dis -= p.transform.localScale.x/2;
            if ( dis < minDis) {
                minDis = dis;
                mPlanet = c.gameObject.transform.parent.gameObject;
            }
        }
        if (prev != mPlanet) {
            mAnimator.SetTrigger("Fliping");
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
