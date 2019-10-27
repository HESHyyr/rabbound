using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TryAgainButton()
    {
        SceneManager.LoadScene("MasterScene");
    }

    public void GiveUpButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
