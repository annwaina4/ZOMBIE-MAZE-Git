using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    private float cameraDistance;
    GameObject player;
    void Start()
    {
        player = GameObject.Find("player");
        cameraDistance = player.transform.position.x - transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3((player.transform.position.x - cameraDistance - transform.position.x)*Time.deltaTime*3, 0, 0);
    }
}
