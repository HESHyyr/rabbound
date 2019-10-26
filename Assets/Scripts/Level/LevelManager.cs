using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> planetList;

    [SerializeField]
    private Vector2 targetPosition;

    [SerializeField]
    private GameObject targetPlanet;

    //Define the range of RGN for each planet. The number should between 0.0 and 1.0, last element must be 1.0. Eg {0.5, 0.7, 1.0}
    [SerializeField]
    private List<float> planetPossibility;

    //Level is defined by distance. After the player reach certain amount of distance, the level becomes harder and planet possibility need to red
    [SerializeField]
    private List<float> levelDistance;

    [SerializeField]
    private GameObject player;

    //Distance between player position and the old generate box center
    [SerializeField]
    private float distanceToRegenerate;

    //Varialble to determine how far each cell center is to its adjacent cell center
    [SerializeField]
    private float cellDistance;

    //How many cells on each line(line to center)
    [SerializeField]
    private float generateBoxCellPerLength;


    public float generatedPersentage;

    private Vector3 generateBoxCenter;
    private float generateBoxLength;

    private HashSet<Vector2> generatedCells;

    private void Awake()
    {
        generatedCells = new HashSet<Vector2>();
        generateBoxLength = generateBoxCellPerLength * cellDistance;
    }


    // Start is called before the first frame update
    void Start()
    {
        generateBoxCenter = player.transform.position;

        generatedCells.Add(new Vector2(0, 0));
        targetPlanet.transform.Translate(new Vector3(targetPosition.x - targetPlanet.transform.position.x, targetPosition.y - targetPlanet.transform.position.y, 0));
        generatedCells.Add(targetPosition);
        GeneratePlanets();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, generateBoxCenter) >= distanceToRegenerate)
            GeneratePlanets();
    }

    void GeneratePlanets()
    {
        generateBoxCenter = player.transform.position;
        for (int xcoord = Mathf.CeilToInt(generateBoxCenter.x - generateBoxLength); xcoord <= Mathf.FloorToInt(generateBoxCenter.x + generateBoxLength); xcoord++)
            //Todo: Add check for black-hole circle
            for (int ycoord = Mathf.CeilToInt(generateBoxCenter.y - generateBoxLength); ycoord <= Mathf.FloorToInt(generateBoxCenter.y + generateBoxLength); ycoord++)
                if (xcoord % (int)cellDistance == 0 && ycoord % (int)cellDistance == 0 && !generatedCells.Contains(new Vector2(xcoord, ycoord)))
                {
                    //Calculate the planet based on possibility we set
                    float number = Random.Range(0.0f, 1.0f);
                    int index = -1;
                    for (int i = 0; i < planetList.Count; i++)
                        if (number <= planetPossibility[i])
                        {
                            index = i;
                            break;
                        }
                    GameObject planetPrefab = planetList[index];

                    GeneratePlanet(xcoord, ycoord, planetPrefab);
                }


    }

    void GeneratePlanet(float x, float y, GameObject planetPrefab)
    {
        generatedCells.Add(new Vector2(x, y));

        float planetX = Random.Range(x - (cellDistance / 2 - planetPrefab.transform.localScale.x), x + (cellDistance / 2 - planetPrefab.transform.localScale.x));
        float planetY = Random.Range(y - (cellDistance / 2 - planetPrefab.transform.localScale.y), y + (cellDistance / 2 - planetPrefab.transform.localScale.y));
        GameObject planet = Instantiate(planetPrefab, new Vector3(planetX, planetY, 0), Quaternion.identity);
    }
}
