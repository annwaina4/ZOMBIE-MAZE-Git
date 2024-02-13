using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Titlestart : MonoBehaviour
{
    public void startBtn()
    {
        SceneManager.LoadScene("main");
    }
}
