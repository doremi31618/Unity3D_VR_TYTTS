using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCallback : MonoBehaviour
{
    WindEffect effect;

    public void OnParticleSystemStopped()
    {
        effect.SetActive();
    }
    private void Start()
    {
        effect = FindObjectOfType<WindEffect>();
    }
}
