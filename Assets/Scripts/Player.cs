using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] float startSpeed = 4f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float maxJumpForce = 12;
    [SerializeField] float jumpOffset = 0.1f;
    [SerializeField] float chargeUpTime = 2f;
    [SerializeField] private float maxChargeDrainAmount = 15.0f;
    [SerializeField] Transform sprite;

    [SerializeField] AudioClip chargeAudio;
    [SerializeField] AudioClip deathAudio;
    [SerializeField] AudioClip jumpAudio;
    [SerializeField] AudioClip landAudio;
    [SerializeField] AudioSource mainMusicSource;

    [SerializeField] ParticleSystem chargeUpEffect;
    [SerializeField] ParticleSystem chargeFinishEffect;
    [SerializeField] ParticleSystem deathEffect;


    float chargeRate;
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

    public bool Win;

    // Start is called before the first frame update
    void Start()
    {
        Win = false;
        speed = startSpeed;
        fuel = GetComponent<FuelSystem>();
        body = GetComponent<Rigidbody2D>();
        animator = sprite.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentJumpForce = jumpForce;
        chargeRate = (maxJumpForce - jumpForce) / chargeUpTime;
        Debug.Log(chargeRate);
    }

    void Update()
    {
        animator.SetBool("Grounded", grounded);
        RotatePlayerOnPlanet();
        if (grounded)
        {
            CheckGameOver();
            Run();
            Jump();
            
        }
        else
        {
            Fly();
        }
    }

    void CheckGameOver() {
        if (!gameOver)
        {
            if (GetFuelTank().isEmpty())
            {
                Die();

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

        if (Input.GetKey("space") && startCharging) {
            if (!audioSource.isPlaying && !playedCharge) {
                audioSource.clip = chargeAudio;
                audioSource.Play();
                playedCharge = true;
                chargeUpEffect.Play();
            }
            if (currentJumpForce < maxJumpForce)
            {
                currentJumpForce = Mathf.Min(maxJumpForce, currentJumpForce + chargeRate * Time.deltaTime);
            }
            else
            {
                chargeUpEffect.Stop();
                chargeFinishEffect.Play();
            }
        }
            

        if (Input.GetKeyUp("space") && startCharging)
        {
            transform.position += transform.up * jumpOffset;
            body.velocity = (transform.up * currentJumpForce + velocity).normalized * currentJumpForce;
            grounded = false;
            animator.SetTrigger("Jumping");

            // draining fuel based on charge amount
            float drainFuelAmount = maxChargeDrainAmount * (currentJumpForce / maxJumpForce);
            GetFuelTank().Drain(drainFuelAmount);

            audioSource.Stop();
            audioSource.PlayOneShot(jumpAudio);
            playedCharge = false;
            startCharging = false;
            chargeUpEffect.Stop();
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

    public void Die()
    {
        if (!gameOver) {
            mainMusicSource.Stop();
            mainMusicSource.PlayOneShot(deathAudio);
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
        GameOver();
    }

    private void GameWin()
    {
        GameObject wave = GameObject.Find("Wave").gameObject;
        wave.SetActive(false);
        Win = true;
        GameOver();
    }

    private void GameOver() {
        gameOver = true;
    }

    public bool IsGameOver() {
        return gameOver;
    }
}
