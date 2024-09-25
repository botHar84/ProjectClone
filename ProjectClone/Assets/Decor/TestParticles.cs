using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestParticles : MonoBehaviour
{
    public ParticleSystem Jump;

    private void Update(){
        if(Input.GetKeyDown(KeyCode.G)){
            Jump.Play();
        }
    }
}
