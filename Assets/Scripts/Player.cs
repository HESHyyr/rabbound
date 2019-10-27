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
    [SerializeField] Transform sprite;

    [SerializeField] AudioClip chargeAudio;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip jumpAudio;
    [SerializeField] AudioClip landAudio;
    [SerializeField] AudioSource BGmusicSource;

    [SerializeField] ParticleSystem Base;
    [SerializeField] ParticleSystem ChargeUp;
    [SerializeField] ParticleSystem ChargeFinish;
    [SerializeField] ParticleSystem Death;
    public float chargeUpTime = 0.6f;
    private float chargeRate;
    private bool chargeFinished = false;

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
    float currentJumpForce;
    AudioSource audioSource;
    bool playedCharge = false;
    bool startCharging = false;

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
        chargeRate = (maxJumpForce-jumpForce) / chargeUpTime;
        Debug.Log(chargeRate);
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
        if (grounded && Input.GetKeyDown("space")) {
            startCharging = true;
        }

        if (Input.GetKey("space")&&startCharging) {
            if (!audioSource.isPlaying&&!playedCharge) {
                audioSource.clip = chargeAudio;
                audioSource.Play();
                //audioSource.Play(chargeAudio);
                playedCharge = true;
                ChargeUp.Play();
            }
            if (currentJumpForce <= maxJumpForce)
            {
                currentJumpForce += chargeRate * Time.deltaTime;
                chargeFinished = false;
            }
            else if(!chargeFinished)
            {
                ChargeUp.Stop();
                ChargeFinish.Play();
                chargeFinished = true;
            }
        }
            

        if (Input.GetKeyUp("space")&&startCharging)
        {
            transform.position += transform.up * jumpOffset;
            body.velocity = (transform.up * currentJumpForce + velocity).normalized * currentJumpForce;
            grounded = false;
            animator.SetTrigger("Jumping");
            // testing fuel
            GetFuelTank().Drain(15);
            audioSource.Stop();
            audioSource.PlayOneShot(jumpAudio);
            playedCharge = false;
            startCharging = false;
            ChargeUp.Stop();
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
        bool pressedTraceBack = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool inField = gravityFields.Count > 0;

        bool hasTracedBack = false;

        if (pressedTraceBack && !prevDidTrace && inField) {

            body.velocity = Vector3.zero;

            Vector3 toClosest = FindClosestPlanet().transform.position - transform.position;
            toClosest.Normalize();

            body.velocity = toClosest * jumpForce;

            hasTracedBack = true;
        }

        prevDidTrace = pressedTraceBack;

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
            BGmusicSource.PlayOneShot(deathAudio);
            Instantiate(Death, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
            //GetFuelTank().RechargeFull();
        }

        //Time.timeScale = 0;
        gameOver = true;
    }

    private void GameWin()
    {
        GameObject wave = GameObject.Find("Wave").gameObject;
        wave.SetActive(false);
        gameOver = true;
    }
}
