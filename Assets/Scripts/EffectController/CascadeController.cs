using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LakePolygon))]
public class CascadeController : MonoBehaviour
{
    LakePolygon InstanceLake;
    public float maxFlowSpeed =1;
    public float maxFloatSpeed =1;

    [Range(0, 1)] public float strength = 0f;
    private float oldValue=-1;
    // [Range(10, 20)] public 
    ParticleSystem[] particleEffect;
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
        InstanceLake = GetComponent<LakePolygon>();
        
        Transform particle = transform.Find("ParticleEffects");
        if(particle!=null)
            particleEffect = particle.transform.GetComponentsInChildren<ParticleSystem>();


        InitParticleStructure();
    }

    //Init Waterfall first state
    void InitParticleStructure(){
        waterfallAttribute.emmisionRateMax=new float[particleEffect.Length];
        for(int i=0; i<particleEffect.Length; i++){
            waterfallAttribute.emmisionRateMax[i]=GetEmissionRate(particleEffect[i]);
        }
        //processing lake value adjust
        UpdateParticleEffect(strength);
    }
    float GetEmissionRate(ParticleSystem ps){
        return ps.emission.rateOverTime.constant;
    }
    // Update is called once per frame
    private void FixedUpdate() {
        UpdateParticleEffect(strength);
    }
    void UpdateParticleEffect(float _strength)
    {
        //processing on value change
        if(oldValue==_strength)return;
        oldValue=_strength;

        //processing lake value adjust
        InstanceLake.automaticFlowMapScale = Mathf.Lerp(0,maxFlowSpeed,_strength);
        InstanceLake.floatSpeed = Mathf.Lerp(0,maxFloatSpeed,_strength);


        for(int i=0; i<particleEffect.Length; i++){
            var emmision =particleEffect[i].emission;
            emmision.rate=Mathf.Lerp(0,waterfallAttribute.emmisionRateMax[i],_strength);
        }

        InstanceLake.GeneratePolygon();
        // InstanceLake.flow
    }
}
