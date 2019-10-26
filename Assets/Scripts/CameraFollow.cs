using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float smoothTime = 0.3F;
    [SerializeField] float damping = 10;
    [SerializeField] float inAirDamping = 1.5f;
    [SerializeField] Player player;

    Vector3 velocity = Vector3.zero;

    void Update()
    {
        Transform playerTarget = player.transform;
        // Define a target position above and behind the target transform
        Vector3 targetPosition = new Vector3(playerTarget.position.x, playerTarget.position.y, -10);

        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        GameObject currentPlanet = player.CurrentPlanet;
        Debug.Log(currentPlanet.name);
        if (currentPlanet == null) return;

        Vector3 frocurrentPlanet = playerTarget.position - currentPlanet.transform.position;
        float desiredRot = Mathf.Atan2(frocurrentPlanet.y, frocurrentPlanet.x) * Mathf.Rad2Deg - 90;

        float currDamp;
        if (player.Grounded)
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
