using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveMovement : MonoBehaviour
{
    [SerializeField] private Vector2 origin = Vector3.zero;
    [SerializeField] private float angle = 0.0f;
    [SerializeField] private float speed = 0.0f;

    private Rigidbody2D rb = null;

    // Inherited from MonoBehavior

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        transform.position = origin;
    }

    void Update()
    {
        SetTransformWithAngle();
        MoveWave();

        DrawDebugLines();
    }

    // Methods

    private void SetTransformWithAngle()
    {
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void MoveWave()
    {
        rb.velocity = transform.up * speed;
    }

    private void DrawDebugLines()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up, Color.green);
        Debug.DrawLine(transform.position, transform.position + transform.right, Color.red);
    }
}
