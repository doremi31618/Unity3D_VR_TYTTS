using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MeshParticleController : MonoBehaviour
{
    public MeshRenderer mesh;
    public ParticleSystem ps;
    public ParticleSystemForceField psf;
    public bool state=false;
    Color oldColor ;
    float endRange=1;

    // Start is called before the first frame update
    void Start()
    {
        oldColor = mesh.material.GetColor("_Color");
        ps.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            state=!state;
            if(state){
                oldColor.a = 0;
                ps.Play();
                Sequence fadeAnimation = DOTween.Sequence();
                fadeAnimation.Append(mesh.material.DOColor(oldColor, 5f));
                fadeAnimation.Insert(0,DOTween.To(()=>endRange, x=>endRange=x, 15f, 3f));
                
            }else{
                oldColor.a = 1;
                Sequence fadeAnimation = DOTween.Sequence();
                fadeAnimation.Append(mesh.material.DOColor(oldColor, 5f));
                fadeAnimation.Append(DOTween.To(()=>endRange, x=>endRange=x, 1, 3f));
            }
            
        }
        psf.endRange = endRange;
    }

    
}
