using Entities;

public class HitCollider : Entity
{
    private void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
    }
}
