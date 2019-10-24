using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Image loadingScreen;

    void Start(){
        loadingScreen.enabled = false;
    }

    public void StartSim(){
        loadingScreen.enabled = true;
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }

    public void quit(){
        Application.Quit();
    }
}
