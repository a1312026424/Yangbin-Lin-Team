using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    GameObject go;
    public float speed;
    public float look = 100;
        void Update()
    {
        go = GameObject.FindGameObjectWithTag("Player");
        Vector3 vec = go.transform.position- this.transform.position ;
        float length = vec.sqrMagnitude;
        vec.y= 0;
        if(length < look)
        {
            transform.rotation = Quaternion.Slerp(this.transform.rotation,Quaternion.LookRotation(vec),0.3f);
            transform.position = transform.position + this.transform.forward * speed * Time.deltaTime;
        }

    }
}
