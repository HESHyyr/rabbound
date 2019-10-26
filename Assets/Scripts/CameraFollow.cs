using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform mTarget;
    Player mPlayer;
    GameObject mPlanet;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    private float desiredRot;
    public float damping = 10;
    public float inAirDamping = 1.5f;

    private void Start()
    {
        mPlayer = GameObject.Find("Player").GetComponent<Player>();
        mTarget = mPlayer.transform;
    }

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = new Vector3(mTarget.position.x, mTarget.position.y, -10);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        mPlanet = mPlayer.mPlanet;
        Debug.Log(mPlanet.name);
        if (mPlanet == null) return;

        Vector3 fromPlanet = mTarget.position - mPlanet.transform.position;
        float desiredRot = Mathf.Atan2(fromPlanet.y, fromPlanet.x) * Mathf.Rad2Deg - 90;

        float currDamp;
        if (mPlayer.grounded)
        {
            currDamp = damping;
        }
        else {
            currDamp = inAirDamping;
        }
        var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * currDamp);
    }
}
