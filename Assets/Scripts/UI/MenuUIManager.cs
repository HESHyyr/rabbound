﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene("MasterScene");
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void CreditButton()
    {
        SceneManager.LoadScene("GameWin");
    }
}
