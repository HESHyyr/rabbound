using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    [SerializeField] private float initialDistanceFromOrigin = 0.0f;
    [SerializeField] private float speed = 0.0f;

    [SerializeField] private Player player = null;
    [SerializeField] private Planet targetPlanet = null;

    [SerializeField] private GameObject edge = null;

    private Rigidbody2D rb = null;
    private BoxCollider2D col = null;
    private CapsuleCollider2D colPlayer = null;

    // Public methods

    // Return true if the given position plus offset is behind the edge of the wave
    public bool IsBehindWave(Vector3 planetPosition, float offset)
    {
        Vector3 toPlanet = planetPosition - transform.position;

        float verticalDistance = Vector3.Dot(toPlanet, transform.up);

        return (verticalDistance + offset < 0.0f);
    }

    // Inherited from MonoBehavior

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        col = edge.GetComponent<BoxCollider2D>();
        colPlayer = player.GetComponent<CapsuleCollider2D>();

        SetInitialRotation();
        SetInitialPosition();
    }

    void Update()
    {
        MoveTowardsTarget();
        MoveLateralWithPlayer();
        DrawDebugLines();

        CheckIfCollidingWithPlayer();
    }

    // Methods

    private void SetInitialRotation()
    {
        Vector3 zAxis = Vector3.forward;
        float angle = Vector3.SignedAngle(Vector3.up, targetPlanet.transform.position, zAxis);
        transform.rotation = Quaternion.AngleAxis(angle, zAxis);
    }

    private void SetInitialPosition()
    {
        Vector3 origin = Vector3.zero;
        transform.position = origin + transform.up * initialDistanceFromOrigin * -1.0f;
    }

    private void MoveTowardsTarget()
    {
        rb.velocity = transform.up * speed;
    }

    private void MoveLateralWithPlayer()
    {

        Vector3 toPlayer = player.transform.position - transform.position;

        float lateralDistance = Vector3.Dot(toPlayer, transform.right);

        transform.position += transform.right * lateralDistance;
    }

    private void CheckIfCollidingWithPlayer()
    {
        bool isColliding = col.IsTouching(colPlayer);

        if (isColliding) {
            player.GameOver();
        }
    }

    private void DrawDebugLines()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.red);
    }
}
