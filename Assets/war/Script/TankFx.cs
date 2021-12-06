#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.
using System.Collections.Generic;
using System;
using UnityEngine;

public class TankFx : MonoBehaviour
{
    public GameObject explodeFab;
    public void PlayExplodeFx(){
        var explodeVFX = Instantiate (explodeFab, transform.position, Quaternion.identity,transform.parent);
        var ps = explodeVFX.GetComponent<ParticleSystem>();
        if (ps != null){
            Destroy (explodeVFX, ps.main.duration);
        }
    }
}
