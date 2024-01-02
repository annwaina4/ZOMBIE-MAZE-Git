using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public int itemCounter = 0;
    GameObject counterText;
    GameObject clearText;
    GameObject gameOverText;

    void Start()
    {
        counterText = GameObject.Find("counterText");
        clearText = GameObject.Find("clearText");
        gameOverText = GameObject.Find("gameOverText");
        GameObject.Find("start").GetComponent<Text>().text = "START";
    }


    void Update()
    {
        if(itemCounter==7)
        {
            clearText.GetComponent<Text>().text = "GAME  CLEAR";
            //playerController.isEnd = true;
        }

        
    }

    public void Addscore()
    {
        counterText.GetComponent<Text>().text = "GET : "+itemCounter+"  /  7";
    }

    public void gameOver()
    {
        gameOverText.GetComponent<Text>().text = "GAME  OVER";
    }
}
