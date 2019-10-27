using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ParallaxCoefficient = System.Single;

public class Background : MonoBehaviour
{
    [SerializeField] private GameObject player = null;

    [SerializeField] private ParallaxCoefficient initial = 0.8f;

    private class ParallaxData
    {
        public Transform transform = null;
        public ParallaxCoefficient coef = 0.0f;
    }

    private List<ParallaxData> stars = new List<ParallaxData>();

    private Vector3 prevCameraPos = Vector3.zero;

    // inherited from monobehavior

    private void Start()
    {
        FillStarData();
    }

    private void Update()
    {
        transform.position = (Vector2)Camera.main.transform.position;

        UpdateStars();

        prevCameraPos = Camera.main.transform.position;
    }

    // private methods

    private void FillStarData()
    {
        foreach (Transform child in transform) {
            ParallaxData data = new ParallaxData {
                transform = child,
                coef = Random.Range(0.0f, initial)
            };
            stars.Add(data);
        }
    }

    private void UpdateStars()
    {
        foreach (ParallaxData star in stars) {

            Vector3 camChange = prevCameraPos - Camera.main.transform.position;

            Vector3 newPos = star.transform.position;
            newPos.x -= camChange.x * star.coef;
            newPos.y -= camChange.y * star.coef;

            star.transform.position = WarpStarFromScreen(newPos);
        }
    }

    private Vector2 WarpStarFromScreen(Vector2 pos)
    {
        Vector3 warpedPosition = pos;

        Vector3 posInViewport = Camera.main.WorldToViewportPoint(pos);

        float height = Camera.main.orthographicSize * 2;
        float width = height * Camera.main.aspect;

        if (posInViewport.x < 0.0f) {
            // left of screen
            warpedPosition += Camera.main.transform.right * width;
        }
        else if (posInViewport.x > 1.0f) {
            // right of screen
            warpedPosition -= Camera.main.transform.right * width;
        }

        if (posInViewport.y < 0.0f) {
            // below screen
            warpedPosition += Camera.main.transform.up * height;
        }
        else if (posInViewport.y > 1.0f) {
            // above screen
            warpedPosition -= Camera.main.transform.up * height;
        }

        return warpedPosition;
    }
}
