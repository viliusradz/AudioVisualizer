using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserController : MonoBehaviour
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadScene(0);

        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadScene(1);

        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SceneManager.LoadScene(2);

        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SceneManager.LoadScene(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SceneManager.LoadScene(4);
        
    }
}
