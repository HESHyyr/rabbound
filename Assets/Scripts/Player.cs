using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float speed = 4;
    [SerializeField] float jumpForce = 5;
    [SerializeField] float jumpOffset = 0.1f;

    Rigidbody2D body;
    GameObject currentPlanet;
    Transform sprite;
    Animator animator;
    bool grounded = false;
    Vector3 velocity;
    [SerializeField] List<Collider2D> gravityFields = new List<Collider2D>();

    public GameObject CurrentPlanet { get => currentPlanet; set => currentPlanet = value; }
    public bool Grounded { get => grounded; set => grounded = value; }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprites");
        animator = sprite.gameObject.GetComponent<Animator>();
        SetMyPlanet();
    }

    void Update()
    {
        //Debug.Log(currentPlanet);
    }

    void FixedUpdate() {
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
        float min = float.PositiveInfinity;
        GameObject prev = currentPlanet;
        foreach (Collider2D field in gravityFields) {
            float distance = (field.transform.position - transform.position).magnitude;
            GameObject p = field.gameObject.transform.parent.gameObject;
            distance -= p.transform.localScale.x / 2;
            if (distance < min) {
                min = distance;
                currentPlanet = field.gameObject.transform.parent.gameObject;
            }
        }
        if (prev != currentPlanet) {
            animator.SetTrigger("Fliping");
        }
        if (currentPlanet == null) return;
        Vector3 fromPlanet = transform.position - currentPlanet.transform.position ;
        float angle = Mathf.Atan2(fromPlanet.y, fromPlanet.x) * Mathf.Rad2Deg - 90;
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
        Debug.Log("Collision...");
        if (currentPlanet == collision.gameObject) {
            body.velocity = Vector3.zero;
            SnapToGround();
            grounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D field)
    {
        if (field.tag == "GravityField") {
            gravityFields.Add(field);
        }
    }

    void OnTriggerExit2D(Collider2D field)
    {
        if (field.tag == "GravityField")
        {
            gravityFields.Remove(field);
        }
    }
}
