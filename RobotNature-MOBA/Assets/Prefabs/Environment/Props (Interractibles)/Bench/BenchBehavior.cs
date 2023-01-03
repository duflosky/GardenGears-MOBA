using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenchBehavior : MonoBehaviour
{
    public Animation An_bench;
    public Collider BenchCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        An_bench.Play();
        BenchCollider.enabled = false;
    }
}

