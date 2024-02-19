using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : PersistantSingleton<SceneController>
{
    public void ChangeScene()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    } 
    public void ChangeScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
