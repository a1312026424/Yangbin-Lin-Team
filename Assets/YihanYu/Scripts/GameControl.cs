using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class GameControl : MonoBehaviour
{
    public GameObject player;
    public GameObject gameOver;
    int foodAll;
    // Start is called before the first frame update
    void Start()
    {
        gameOver.gameObject.SetActive(false);
        foodAll = 21;
    }

    // Update is called once per frame
    void Update()
    {
        //If win 
        if(player.GetComponent<PlayerControl>().scoreCnt==foodAll){
            gameOver.gameObject.SetActive(true);
            GameObject finalScore = GameObject.Find("FinalScore");
            finalScore.GetComponent<Text>().text = string.Format("Score:{0}", player.GetComponent<PlayerControl>().scoreCnt);
        }
    }
}
