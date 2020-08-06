using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WaterfallController : MonoBehaviour
{
    [Range(0, 0.8f)] public float alphaStrength = 0.8f;
    // [Range(10, 20)] public 
    float radius = 17;
    ParticleSystem particleEffect;
    MeshRenderer maskMesh;
    public struct ParticleEffectAttribute
    {
        public float emmisionRateMax;
        public float radius;

    }
    ParticleEffectAttribute waterfallAttribute;


    // Start is called before the first frame update
    void Start()
    {
        particleEffect = transform.Find("ParticleEffect").GetComponent<ParticleSystem>();
        maskMesh = particleEffect.transform.Find("Mask").GetComponent<MeshRenderer>();
        InitParticleStructure();
    }

    //Init Waterfall first state
    void InitParticleStructure(){
        waterfallAttribute.radius = radius;
        waterfallAttribute.emmisionRateMax=particleEffect.emission.rateOverTime.constant;
    }

    private void FixedUpdate() {
        var emmision =particleEffect.emission;
        emmision.rate=Mathf.Lerp(0,waterfallAttribute.emmisionRateMax,alphaStrength);

        maskMesh.material.SetFloat("_Alpha", alphaStrength);
    }

}
