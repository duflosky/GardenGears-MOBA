using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShowSceneManager : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    private void Update()
    {
        Move();
    }

    public float playerSpeed = 1;
    private void Move()
    {
        var moveVector = Vector3.zero;
        if(Input.GetKey(KeyCode.Q))moveVector+= Vector3.left;
        if(Input.GetKey(KeyCode.Z))moveVector+= Vector3.forward;
        if(Input.GetKey(KeyCode.S))moveVector+= Vector3.back;
        if(Input.GetKey(KeyCode.D))moveVector+= Vector3.right;
        var boostSpeed = Input.GetKey(KeyCode.LeftControl) ?4 : 1;
        transform.position += moveVector.normalized * playerSpeed * boostSpeed * Time.deltaTime;
    }
}
