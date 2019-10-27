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
    [SerializeField] Text GameWinText;
    [SerializeField] Text GameOverText;
    [SerializeField] Transform sprite;

    [SerializeField] AudioClip chargeAudio;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip jumpAudio;
    [SerializeField] AudioClip landAudio;
    public float chargeUpTime = 0.7f;
    private float chargeRate;

    FuelSystem fuel;
    float speed;
    bool gameOver = false;
    Rigidbody2D body;
    Planet currentPlanet;
    Animator animator;
    bool grounded = false;

    bool prevDidTrace = false;

    Vector3 velocity;
    List<Collider2D> gravityFields = new List<Collider2D>();
    bool doubleJump;
    float currentJumpForce;
    AudioSource audioSource;

    public Planet CurrentPlanet { get => currentPlanet; set => currentPlanet = value; }
    public bool Grounded { get => grounded; set => grounded = value; }
    public float Speed { get => speed; set => speed = value; }

    // Start is called before the first frame update
    void Start()
    {
        speed = startSpeed;
        fuel = GetComponent<FuelSystem>();
        body = GetComponent<Rigidbody2D>();
        animator = sprite.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentJumpForce = jumpForce;
        GameOverText.enabled = false;
        GameWinText.enabled = false;
    }

    void Update()
    {
        animator.SetBool("Grounded", grounded);
        RotatePlayerOnPlanet();
        if (grounded)
        {
            Run();
            Jump();
        }
        else
        {
            Fly();
        }
        if (!gameOver)
        {
            if (GetFuelTank().isEmpty() && grounded)
            {
                GameOver();
            }
            if (CurrentPlanet.name == "target")
            {
                GameWin();
            }
        }
    }

    void Run() {
        float move = Input.GetAxis("Horizontal") * speed;
        velocity = transform.right * move;
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
    }

    void Jump() {
        if (Input.GetKey("space")) {
            if (!audioSource.isPlaying) {
                audioSource.clip = chargeAudio;
                audioSource.Play();
                //audioSource.Play(chargeAudio);
            }
            if (currentJumpForce <= maxJumpForce)
                currentJumpForce += 0.2f;
        }
            

        if (Input.GetKeyUp("space"))
        {
            transform.position += transform.up * jumpOffset;
            body.velocity = (transform.up * currentJumpForce + velocity).normalized * currentJumpForce;
            grounded = false;
            animator.SetTrigger("Jumping");
            // testing fuel
            GetFuelTank().Drain(15);
            audioSource.Stop();
            audioSource.PlayOneShot(jumpAudio);
        }
    }

    void Fly() {
        if (!TraceBack()) {
            float downSpeed = Vector3.Dot(body.velocity, -transform.up);
            downSpeed /= currentJumpForce;
            animator.SetFloat("Vertical", downSpeed);
        }
    }

    bool TraceBack()
    {
        bool pressedSpace = Input.GetKeyDown("space");
        bool inField = gravityFields.Count > 0;

        bool hasTracedBack = false;

        if (pressedSpace && !prevDidTrace && inField) {

            body.velocity = Vector3.zero;

            Vector3 toClosest = FindClosestPlanet().transform.position - transform.position;
            toClosest.Normalize();

            body.velocity = toClosest * jumpForce;

            hasTracedBack = true;
        }

        prevDidTrace = pressedSpace;

        return hasTracedBack;
    }

    void RotatePlayerOnPlanet() {
        Planet lastPlanet = currentPlanet;
        currentPlanet = FindClosestPlanet();

        if (lastPlanet != currentPlanet) {
            animator.SetTrigger("Fliping");
        }

        AdjustPlayerOrientation();
    }

    void AdjustPlayerOrientation() {
        Vector3 fromPlanet = transform.position - currentPlanet.transform.position;
        float angle = Mathf.Atan2(fromPlanet.y, fromPlanet.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    Planet FindClosestPlanet() {
        float min = float.PositiveInfinity;
        Planet planet = currentPlanet;
        foreach (Collider2D field in gravityFields)
        {
            float distance = (field.transform.position - transform.position).magnitude;
            GameObject planetObject = field.gameObject.transform.parent.gameObject;
            distance -= planetObject.transform.localScale.x / 2;
            if (distance < min)
            {
                min = distance;
                planet = planetObject.GetComponent<Planet>();
            }
        }

        return planet;
    }

    void SnapToGround() {
        Vector3 dif = transform.position - currentPlanet.transform.position;
        dif.Normalize();
        float groundDis = (currentPlanet.transform.localScale.x + transform.localScale.y) / 2;
        transform.position = currentPlanet.transform.position + dif * groundDis;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentPlanet == null) return;
        if (currentPlanet.gameObject == collision.gameObject) {
            body.velocity = Vector3.zero;
            SnapToGround();
            OnFirstLand();
            grounded = true;
        }
    }

    void OnFirstLand() {
        if (grounded) return;
        audioSource.PlayOneShot(landAudio);
        currentPlanet.ApplyBuff(this);
        currentJumpForce = jumpForce;
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

    public void SetOriginalSpeed() {
        speed = startSpeed;
    }

    public FuelTank GetFuelTank() {
        return fuel.GetFuelTank();
    }

    public void GameOver()
    {
        if (!gameOver) {
            audioSource.PlayOneShot(deathAudio);
        }
        
        Time.timeScale = 0;
        GameOverText.enabled = true;
        gameOver = true;
    }

    private void GameWin()
    {
        GameWinText.enabled = true;
        GameObject wave = GameObject.Find("Wave").gameObject;
        wave.SetActive(false);
        gameOver = true;
    }

    public void EnableDoubleJump()
    {
        doubleJump = true;
    }

    public bool CanDoubleJump()
    {
        return doubleJump;
    }

}
