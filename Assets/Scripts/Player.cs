using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float jumpForce = 5;
    [SerializeField] float jumpOffset = 0.1f;

    Rigidbody2D body;
    GameObject currentPlanet;
    Transform sprite;
    Animator animator;
    bool grounded = false;
    Vector3 velocity;
    List<Collider2D> gravities = new List<Collider2D>();

    public GameObject CurrentPlanet { get => currentPlanet; set => currentPlanet = value; }
    public bool Grounded { get => grounded; set => grounded = value; }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprites");
        animator = sprite.gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate() {

        animator.SetBool("Grounded", grounded);
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
        float move = Input.GetAxis("Horizontal") * speed;
        velocity = transform.right *  move;
        transform.position += velocity * Time.fixedDeltaTime;

        //set animator
        Vector3 newScale = sprite.localScale;
        if (move < 0 && newScale.x > 0) {
            newScale.x *= -1;
            sprite.localScale = newScale;
        }
        else if (move > 0 && newScale.x < 0) {
            newScale.x *= -1;
            sprite.localScale = newScale;
        }
        animator.SetFloat("Horizontal", Mathf.Abs(move));

        if (Input.GetKeyDown("space"))
        {
            transform.position += transform.up * jumpOffset;
            body.velocity = (transform.up * jumpForce + velocity).normalized * jumpForce;
            grounded = false;
            animator.SetTrigger("Jumping");
        }
    }

    void Fly() {
        float downSpeed = Vector3.Dot(body.velocity, -transform.up);
        downSpeed /= jumpForce;
        animator.SetFloat("Vertical", downSpeed);
    }

    void SetMyPlanet() {
        float minDis = float.PositiveInfinity;
        GameObject prev = currentPlanet;
        foreach(Collider2D c in gravities) {
            float dis = (c.transform.position - transform.position).magnitude;
            GameObject p = c.gameObject.transform.parent.gameObject;
            dis -= p.transform.localScale.x/2;
            if ( dis < minDis) {
                minDis = dis;
                currentPlanet = c.gameObject.transform.parent.gameObject;
            }
        }
        if (prev != currentPlanet) {
            animator.SetTrigger("Fliping");
        }
        Vector3 frocurrentPlanet = transform.position- currentPlanet.transform.position ;
        float angle = Mathf.Atan2(frocurrentPlanet.y, frocurrentPlanet.x) * Mathf.Rad2Deg-90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void SnapToGround() {
        if (currentPlanet == null) return;
        Vector3 dif = transform.position - currentPlanet.transform.position;
        dif.Normalize();
        float groundDis = (currentPlanet.transform.localScale.x + transform.localScale.y) / 2;
        transform.position = currentPlanet.transform.position + dif * groundDis;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (currentPlanet == collision.gameObject) {
            //Debug.Log(collision.collider.name);
            body.velocity = Vector3.zero;
            SnapToGround();
            grounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "GravityFeild") {
            gravities.Add(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "GravityFeild")
        {
            gravities.Remove(other);
        }
    }
}
