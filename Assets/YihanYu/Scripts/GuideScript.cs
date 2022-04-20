using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GuideScript : MonoBehaviour
{
    public GameObject help;
    // Start is called before the first frame update
    void Start()
    {
        help.SetActive(false);
    }
    public void showHelp(){
         help.SetActive(true);  
    }
    public void closeHelp(){
         help.SetActive(false);
    }
    public void LoadScene(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
