using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Scene state")]
    public int backScene;
    public int currentScene;
    public int nextScene;
    private int menuScene = 2; //ACUERDATE DEL LOGO QUE VA DELANTE Y SI ES POSIBLE PANTALLA DE SELECCIÓN DE IDIOMA
    private int managerScene = 0;
    private int sceneCountInBuildSettings; //número de escenas dentro de la build Index INCLUYENDO MANAGER

    [Header("LoadParameters")]
    private AsyncOperation asynLoad = null;  //Variables que permite ver progreso
    private AsyncOperation asynUnload = null;
    private bool loading = false;
    private int sceneToLoad;

    // Use this for initialization
    void Start()
    {
        //PARA TESTING
        if(SceneManager.sceneCount >= 2) SceneManager.SetActiveScene(SceneManager.GetSceneAt(1)); //JERARQUIA DE ESCENAS SEGUN ORDEN EN EDITOR

        UpdateSceneState();

        if(currentScene == managerScene) Load(nextScene);
    }

    // Update is called once per frame
    void Update()
    {
        //AÑADIR A INPUT MANAGER
        if(Input.GetKey(KeyCode.AltGr))
        {
            if(Input.GetKeyDown(KeyCode.N)) Load(nextScene);
            if(Input.GetKeyDown(KeyCode.B)) Load(backScene);
            if(Input.GetKeyDown(KeyCode.R)) Load(currentScene);
            if(Input.GetKeyDown(KeyCode.M)) Load(menuScene);
        }
    }

    void UpdateSceneState()
    {
        sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;

        currentScene = SceneManager.GetActiveScene().buildIndex; //GetActiveScene solo da la escena como asset, AÑADIR buildIndex para recibir el numero dentro del Index

        if(currentScene + 1 >= sceneCountInBuildSettings) nextScene = managerScene + 1;
        else nextScene = currentScene + 1;

        if(currentScene - 1 <= managerScene) backScene = sceneCountInBuildSettings - 1;
        else backScene = currentScene - 1;
    }

    void Load(int index)
    {
        if(loading)
        {
            Debug.LogError("ALREADY LOADING A SCENE");
            return;
            //evita que se cargen escenas AL MISMO TIEMPO
        }

        loading = true;
        sceneToLoad = index;
        if (currentScene != managerScene) asynUnload = SceneManager.UnloadSceneAsync(currentScene);
        asynLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        while(loading)
        {
            Debug.Log(asynLoad.progress);

            if((asynUnload == null || asynUnload.isDone) && asynLoad.isDone)
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneToLoad));
                UpdateSceneState();
                loading = false;
            }

            yield return null;
        }
    }
}