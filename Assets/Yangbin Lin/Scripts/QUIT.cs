using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QUIT : MonoBehaviour
{
    public void QuitGame()
    {
        Debug.Log("QUIT");
        
        Application.Quit();
    }
}