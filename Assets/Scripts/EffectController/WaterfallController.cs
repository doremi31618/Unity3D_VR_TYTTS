using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
use alpha strength to chenge the state of this particle effet 

*/
[ExecuteInEditMode]
public class WaterfallController : MonoBehaviour
{
    [Range(0, 0.8f)] public float alphaStrength = 0.8f;
    private float oldValue;
    // [Range(10, 20)] public 
    // float radius = 17;
    ParticleSystem[] particleEffect;
    MeshRenderer[] maskMesh;
    public struct ParticleEffectAttribute
    {
        // public float emmisionRateMax;
        // public float steamEmmisionRateMax;
        public float[] emmisionRateMax;
        // public float radius;

    }
    ParticleEffectAttribute waterfallAttribute;
    

    // Start is called before the first frame update
    void Start()
    {
        oldValue=alphaStrength;
        
        Transform particle = transform.Find("ParticleEffects");
        if(particle!=null)
            particleEffect = particle.transform.GetComponentsInChildren<ParticleSystem>();

        Transform mask = transform.Find("Mask");
        if(mask!=null)
            maskMesh=mask.transform.GetComponentsInChildren<MeshRenderer>();
        InitParticleStructure();
    }

    //Init Waterfall first state
    void InitParticleStructure(){
        // waterfallAttribute.radius = radius;
        waterfallAttribute.emmisionRateMax=new float[particleEffect.Length];
        for(int i=0; i<particleEffect.Length; i++){
            waterfallAttribute.emmisionRateMax[i]=GetEmissionRate(particleEffect[i]);
        }


    }
    float GetEmissionRate(ParticleSystem ps){
        return ps.emission.rateOverTime.constant;
    }
   
    private void FixedUpdate() {

        //processing on value change
        if(oldValue==alphaStrength)return;
        oldValue=alphaStrength;


        for(int i=0; i<particleEffect.Length; i++){
            var emmision =particleEffect[i].emission;
            emmision.rate=Mathf.Lerp(0,waterfallAttribute.emmisionRateMax[i],alphaStrength);
        }

        for(int i=0; i<maskMesh.Length; i++){
            maskMesh[i].material.SetFloat("_Alpha",alphaStrength);
        }
        
    }
    

}
