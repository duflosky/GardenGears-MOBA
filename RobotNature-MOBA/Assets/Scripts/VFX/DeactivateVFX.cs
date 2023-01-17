using UnityEngine;

public class DeactivateVFX : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}
