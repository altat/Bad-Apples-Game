using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRootObject : MonoBehaviour {

    public GameObject UIRootObject1;
    /*private AsyncOperation sceneAsync;
    public string Loadscene;
    public string Unloadscene;

    void Start()
    {
        StartCoroutine(loadScene(Loadscene));
        StartCoroutine(unloadScene(Unloadscene));
    }

    IEnumerator loadScene(string index)
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        scene.allowSceneActivation = false;
        sceneAsync = scene;

        //Wait until we are done loading the scene
        while (scene.progress < 0.9f)
        {
            Debug.Log("Loading scene " + " [][] Progress: " + scene.progress);
            yield return null;
        }
        OnFinishedLoadingAllScene();
    }

    void enableScene(int index)
    {
        //Activate the Scene
        sceneAsync.allowSceneActivation = true;


        Scene sceneToLoad = SceneManager.GetSceneByBuildIndex(index);
        if (sceneToLoad.IsValid())
        {
            Debug.Log("Scene is Valid");
            SceneManager.MoveGameObjectToScene(UIRootObject1, sceneToLoad);
            SceneManager.SetActiveScene(sceneToLoad);
        }
    }

    IEnumerator unloadScene(string index)
    {
        AsyncOperation scene = SceneManager.UnloadSceneAsync(index);
        scene.allowSceneActivation = true;
        sceneAsync = scene;

        //Wait until we are done unloading the scene
        while (scene.progress < 0.9f)
        {
            Debug.Log("Unloading scene " + " [][] Progress: " + scene.progress);
            yield return null;
        }
    }


    void OnFinishedLoadingAllScene()
    {
        Debug.Log("Done Loading Scene");
        enableScene(2);
        Debug.Log("Scene Activated!");
    }*/
}
