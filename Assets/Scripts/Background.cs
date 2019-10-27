using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ParallaxCoefficient = System.Single;

public class Background : MonoBehaviour
{
    [SerializeField] private ParallaxCoefficient initial = 0.8f;
    private float starDensity = 10.0f;
    private CameraFollow camFollow = null;

    [SerializeField] List<Sprite> starSprites = new List<Sprite>();

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
        camFollow = Camera.main.GetComponent<CameraFollow>();
        CreateStars();
    }

    private void Update()
    {
        transform.position = (Vector2)Camera.main.transform.position;

        UpdateStars();

        prevCameraPos = Camera.main.transform.position;
    }

    // private methods

    private void CreateStars()
    {

        Transform cam = Camera.main.transform;

        float additional = 1.2f;
        float height = camFollow.maxOrthoSize * 2 * additional;
        float width = height * Camera.main.aspect * additional;

        // origin is the top left
        Vector3 origin = new Vector3(cam.position.x, cam.position.y, 0.0f);
        origin += cam.up * height / 2.0f;
        origin -= cam.right * width / 2.0f;

        float wInterval = width / starDensity;
        float hInterval = height / starDensity;

        int count = 0;

        for (float i = 0; i < width; i += wInterval) {
            for (float j = 0; j < height; j += hInterval) {

                Vector3 starPos = origin;
                starPos.x += i;
                starPos.y -= j;

                GameObject start = new GameObject("Star (" +  count.ToString() + ")");

                start.transform.parent = transform;

                start.transform.position = starPos;
                float randomDistance = Random.Range(0.0f, 3.0f);
                Vector3 offset = Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.forward) * Vector3.up * randomDistance;
                start.transform.position += offset;

                Vector3 rot = new Vector3(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
                start.transform.rotation = Quaternion.Euler(rot);

                SpriteRenderer sprite = start.AddComponent<SpriteRenderer>();
                sprite.sprite = starSprites[Random.Range(0, starSprites.Count - 1)];

                float randomScale = Random.Range(0.1f, 0.4f);
                start.transform.localScale *= randomScale;

                ParallaxData data = new ParallaxData {
                    transform = start.transform,
                    coef = Random.Range(0.0f, randomScale)
                };
                stars.Add(data);

                count++;
            }
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

        float additional = 2.0f;

        // temporarily make the camera larger when determining the viewport point
        float orthoTemp = Camera.main.orthographicSize * additional;
        Camera.main.orthographicSize = camFollow.maxOrthoSize;

        // get viewport point for largest possible size of camera
        Vector3 posInViewport = Camera.main.WorldToViewportPoint(pos);

        // then set the camera back
        Camera.main.orthographicSize = orthoTemp / additional;

        float height = camFollow.maxOrthoSize * 2 * additional;
        float width = height * Camera.main.aspect * additional;

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

    private Vector2 GetMaxScreenDimensions()
    {
        return Vector2.zero;
    }
}
