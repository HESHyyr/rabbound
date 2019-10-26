using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    [SerializeField] private float initialDistanceFromOrigin = 0.0f;
    [SerializeField] private float speed = 0.0f;

    [SerializeField] private GameObject player = null;
    [SerializeField] private Transform targetPlanet = null;

    private Rigidbody2D rb = null;

    // Inherited from MonoBehavior

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        SetInitialRotation();
        SetInitialPosition();
    }

    void Update()
    {
        MoveTowardsTarget();
        MoveLateralWithPlayer();
        DrawDebugLines();
    }

    // Methods

    private void SetInitialRotation()
    {
        Vector3 zAxis = Vector3.forward;
        float angle = Vector3.SignedAngle(Vector3.up, targetPlanet.position, zAxis);
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

    private void DrawDebugLines()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.red);
    }
}
