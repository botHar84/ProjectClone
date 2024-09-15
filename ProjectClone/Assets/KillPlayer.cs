using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public void OnTriggerEnter2D (Collider2D other) {
        if (other.gameObject.tag == "Player"){
            Destroy(other.gameObject);
        }
    }
}
