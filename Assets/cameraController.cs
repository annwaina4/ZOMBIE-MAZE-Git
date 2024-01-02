using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    private float cameraDistanceX;
    private float cameraDistanceZ;
    GameObject player;
    void Start()
    {
        player = GameObject.Find("player");
        cameraDistanceX = player.transform.position.x - transform.position.x;
        cameraDistanceZ = player.transform.position.z - transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += new Vector3((player.transform.position.x - cameraDistanceX - transform.position.x)*Time.deltaTime*3, 0, 0);
        transform.position += new Vector3(0, 0, (player.transform.position.z - cameraDistanceZ - transform.position.z) * Time.deltaTime * 3);
    }
}
