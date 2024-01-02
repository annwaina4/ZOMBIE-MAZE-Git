using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startTextController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("Destroy", 1.0f);        
    }

    private void Destroy()
    {
        Destroy(gameObject); 
    }
}
