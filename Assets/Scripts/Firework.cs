using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : MonoBehaviour
{
    Rigidbody2D mBody;
    public float minTime = 1f;
    public float maxTime = 2f;
    float lifeTime;
    public ParticleSystem fireWork;

    // Start is called before the first frame update
    void Start()
    {
        mBody = gameObject.GetComponent<Rigidbody2D>();
        transform.rotation = Random.rotation;
        lifeTime = Random.Range(minTime, maxTime);
        mBody.velocity = transform.right * (5 / lifeTime);
        Invoke("End", lifeTime);
    }
    void End() {
        Instantiate(fireWork, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
