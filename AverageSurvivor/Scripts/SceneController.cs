using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void SceneChanger(string name)
    {
        SceneManager.LoadScene(name);
        Time.timeScale = 1f;
    }

    public void QuitApplication()
    {
        Application.Quit();
        Debug.Log("Application has been quit.");
    } 
}
