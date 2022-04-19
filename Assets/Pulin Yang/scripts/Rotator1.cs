using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator1 : MonoBehaviour
{
   


    void Update()
    {
        transform.Rotate(new Vector3(10, 30, 45) * Time.deltaTime);
    }
}
