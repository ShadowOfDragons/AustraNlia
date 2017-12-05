using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
	public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    //RESOLUTION
    public void X1080()
    {
        Debug.Log("Done");
        Screen.SetResolution(1920, 1080, false);
    }
    public void X720()
    {
        Screen.SetResolution(1280, 720, false);
    }
    public void X1200()
    {
        Screen.SetResolution(1600, 1200, false);
    }
    public void FullScreen()
    {
        if(Screen.fullScreen == true)
        {
            Screen.fullScreen = false;
            Debug.Log("Fulled");
        }
        else if(Screen.fullScreen == false)
        {
            Screen.fullScreen = true;
            Debug.Log("NotFulled");
        }
    }

    //GRAPHIC QUALITY
    public void Ultra()
    {
        
    }
}
