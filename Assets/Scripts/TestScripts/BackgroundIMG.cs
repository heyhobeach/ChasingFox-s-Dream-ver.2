using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundIMG : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, transform.position.y, 0);
    }
}
