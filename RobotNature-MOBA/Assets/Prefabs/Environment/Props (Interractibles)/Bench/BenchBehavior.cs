using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchBehavior : MonoBehaviour
{
    public Animation AnBench;
    public Collider BenchCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        AnBench.Play();
        BenchCollider.enabled = false;
    }
}
