using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] float smoothTime = 0.3F;
    [SerializeField] float damping = 10;
    [SerializeField] float inAirDamping = 1.5f;
    [SerializeField] Player player;

    [SerializeField] GameObject master;

    public float maxOrthoSize { get; private set; } = 10;
    private float minOrthoSize = 5;

    Vector3 velocity = Vector3.zero;

    void Update()
    {
        if(master.name == "Player")
        {
            Transform playerTarget = player.transform;
            PanMoveCamera(playerTarget);
            RotateCamera(playerTarget);
            ZoomOutOnClick();
        }
        else
            PanMoveCamera(master.transform);

    }

    void PanMoveCamera(Transform target) {
        // Define a target position above and behind the target transform
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, -10);
        // Smoothly move the camera towards that target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    void RotateCamera(Transform target) {
        Planet currentPlanet = player.CurrentPlanet;
        if (currentPlanet == null) return;
        Vector3 fromPlanet = target.position - currentPlanet.transform.position;
        float desiredRot = Mathf.Atan2(fromPlanet.y, fromPlanet.x) * Mathf.Rad2Deg - 90;
        float currDamp = player.Grounded ? damping : inAirDamping;
        var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, desiredRot);
        transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotQ, Time.deltaTime * currDamp);
    }

    void ZoomOutOnClick() {
        if (Input.GetKey(KeyCode.H) && GetComponent<Camera>().orthographicSize < maxOrthoSize)
            GetComponent<Camera>().orthographicSize += 1;
        if (!Input.GetKey(KeyCode.H) && GetComponent<Camera>().orthographicSize > minOrthoSize)
            GetComponent<Camera>().orthographicSize -= 1;
    }
}
