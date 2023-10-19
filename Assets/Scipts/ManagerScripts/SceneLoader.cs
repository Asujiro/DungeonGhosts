using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Load the "LevelOne" scene
    public void LoadLevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }

    // Load the "LevelTwo" scene
    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("LevelTwo");
    }

    // Load the "LevelThree" scene
    public void LoadLevelThree()
    {
        SceneManager.LoadScene("LevelThree");
    }

    // Load the "MainMenu" scene
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}