using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class voice : MonoBehaviour
{
    private AudioSource AS;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        AS.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
