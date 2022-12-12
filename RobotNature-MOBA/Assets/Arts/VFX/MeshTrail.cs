using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float activeTime = 2f;
    public bool isDistance; 

    [Header("Mesh Related")]
    public float meshRefreshRate = 0.1f;
    public Transform positionToSpawn;
    public float meshDestroyDelay = 3f;

    [Header("Shader Related")]
    public Material mat;
    public string shaderVarRef;
    public float shaderVarRate = 0.1f;
    public float shaderVarRefreshRate = 0.05f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;


    void Update()
    {
      if (isDistance == true)
        {
            if (Input.GetKeyDown(KeyCode.E) && !isTrailActive)
            {
                isTrailActive = true;
                StartCoroutine(ActivateTrail(activeTime));
            }
        }
    else
        {
            if (Input.GetKeyUp(KeyCode.Q) && !isTrailActive)
            {
                isTrailActive = true;
                StartCoroutine(ActivateTrail(activeTime));
            }
        }
    }

    IEnumerator ActivateTrail (float timeActive)
    {
        while(timeActive > 0)
        {
            timeActive -= meshRefreshRate;

            if (skinnedMeshRenderers == null)
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            for(int i=0; i<skinnedMeshRenderers.Length; i++)
            {
                GameObject gObj = new GameObject();
                gObj.transform.SetPositionAndRotation(positionToSpawn.position, positionToSpawn.rotation);

                MeshRenderer mr = gObj.AddComponent<MeshRenderer>();
                MeshFilter mf = gObj.AddComponent<MeshFilter>();

                Mesh mesh = new Mesh();
                skinnedMeshRenderers[i].BakeMesh(mesh);

                mf.mesh = mesh;
                mr.material = mat;

                StartCoroutine(AnimateMaterialFloat(mr.material, 0, shaderVarRate, shaderVarRefreshRate));

                Destroy(gObj, meshDestroyDelay);

            }


            yield return new WaitForSeconds(meshRefreshRate);
        }
        isTrailActive = false;
    }

    IEnumerator AnimateMaterialFloat(Material mat, float goal, float rate, float refreshRate)
    {
        float valueToAnimate = mat.GetFloat(shaderVarRef);

        while (valueToAnimate > goal)
        {
            valueToAnimate -= rate;
            mat.SetFloat(shaderVarRef, valueToAnimate);
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
