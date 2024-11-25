using Controllers;
using UnityEngine;

public class RiceEffectController : MonoBehaviour, IControllable
{
    [Header("References")] [SerializeField]
    private ParticleSystem _particle;

    public void Initialize()
    {
        _particle.Play(false);
    }

    public void DoUpdate()
    {
    }

    public void DoFixedUpdate()
    {
        
    }
}
