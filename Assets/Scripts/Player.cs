using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] float startSpeed = 4f;
    [SerializeField] float jumpForce = 5f;
    //MaxJumpForce for jetpack
    [SerializeField] float maxJumpForce = 12;
    [SerializeField] float jumpOffset = 0.1f;
    [SerializeField] private Text GameWinText;
    [SerializeField] Text GameOverText;

    FuelSystem fuel;
    float speed;

    private bool gameOver = false;


    Rigidbody2D body;
    Planet currentPlanet;
    Transform sprite;
    Animator animator;
    bool grounded = false;
    Vector3 velocity;
    List<Collider2D> gravityFields = new List<Collider2D>();

    private float currentJumpForce;
    private bool firstland = true;

    public Planet CurrentPlanet { get => currentPlanet; set => currentPlanet = value; }
    public bool Grounded { get => grounded; set => grounded = value; }

    // Start is called before the first frame update
    void Start()
    {
        speed = startSpeed;
        fuel = GetComponent<FuelSystem>();
        body = GetComponent<Rigidbody2D>();
        sprite = transform.Find("Sprites");
        animator = sprite.gameObject.GetComponent<Animator>();
        SetMyPlanet();
        currentJumpForce = jumpForce;
        GameOverText.enabled = false;
        GameWinText.enabled = false;
    }

    void Update()
    {
        animator.SetBool("Grounded", grounded);
        SetMyPlanet();
        if (grounded)
        {
            Run();
            firstland = false;
        }
        else
        {
            Fly();
            firstland = true;
        }
        if (!gameOver)
        {
            if (GetFuelTank().isEmpty())
            {
                gameOver = true;
                GameOver();
            }
            if (CurrentPlanet.name == "target")
            {
                gameOver = true;
                GameWin();
            }
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

        if (Input.GetKey("space"))
            if (currentJumpForce <= maxJumpForce)
                currentJumpForce += 0.2f;

        if (Input.GetKeyUp("space"))
        {
            transform.position += transform.up * jumpOffset;
            body.velocity = (transform.up * currentJumpForce + velocity).normalized * currentJumpForce;
            grounded = false;
            animator.SetTrigger("Jumping");
            // testing fuel
            GetFuelTank().Drain(15);
        }
    }

    void Fly() {
        float downSpeed = Vector3.Dot(body.velocity, -transform.up);
        downSpeed /= currentJumpForce;
        animator.SetFloat("Vertical", downSpeed);
    }

    void SetMyPlanet() {
        float min = float.PositiveInfinity;
        Planet lastPlanet = currentPlanet;
        foreach (Collider2D field in gravityFields) {
            float distance = (field.transform.position - transform.position).magnitude;
            GameObject p = field.gameObject.transform.parent.gameObject;
            distance -= p.transform.localScale.x / 2;
            if (distance < min) {
                min = distance;
                GameObject planetObject = field.gameObject.transform.parent.gameObject;
                currentPlanet = planetObject.GetComponent<Planet>();
            }
        }
        if (lastPlanet != currentPlanet) {
            animator.SetTrigger("Fliping");
        }
        if (currentPlanet == null) return;
        Vector3 fromPlanet = transform.position - currentPlanet.transform.position;
        float angle = Mathf.Atan2(fromPlanet.y, fromPlanet.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void SnapToGround() {
        if (currentPlanet == null) return;
        Vector3 dif = transform.position - currentPlanet.transform.position;
        dif.Normalize();
        float groundDis = (currentPlanet.transform.localScale.x + transform.localScale.y) / 2;
        transform.position = currentPlanet.transform.position + dif * groundDis;
        currentPlanet.ApplyBuff(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentPlanet == null) return;
        if (currentPlanet.gameObject == collision.gameObject) {
            body.velocity = Vector3.zero;
            SnapToGround();
            grounded = true;
            if (firstland)
                currentJumpForce = jumpForce;
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

    public float GetSpeed() {
        return speed;
    }

    public void SetSpeed(float speed) {
        this.speed = speed;
    }

    public void SetOriginalSpeed() {
        SetSpeed(startSpeed);
    }

    public FuelTank GetFuelTank() {
        return fuel.GetFuelTank();
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        GameOverText.enabled = true;
    }

    private void GameWin()
    {
        GameWinText.enabled = true;
        GameObject wave = GameObject.Find("Wave").gameObject;
        wave.SetActive(false);
    }

}
