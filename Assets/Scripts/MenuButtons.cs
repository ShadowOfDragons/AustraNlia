using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public Toggle fullScreenToggle;

    public OptionsManager optionsManager;

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
        fullScreenToggle.isOn = Screen.fullScreen;

        if(!Screen.fullScreen) Debug.Log("NotFulled");
        else Debug.Log("Fulled");
    }

    //GRAPHICS RESOLUTION
    public void ToasterGraphQuality()
    {
        QualitySettings.SetQualityLevel(0);
    }

    public void LowGraphQuality()
    {
        QualitySettings.SetQualityLevel(1);
    }

    public void MediumGraphQuality()
    {
        QualitySettings.SetQualityLevel(2);
    }

    public void HighGraphQuality()
    {
        QualitySettings.SetQualityLevel(3);
    }

    public void UltraGraphQuality()
    {
        QualitySettings.SetQualityLevel(4);
    }


    public void OnLoadOptions()
    {

    }

    public void OnSaveOptions()
    {

    }
}
