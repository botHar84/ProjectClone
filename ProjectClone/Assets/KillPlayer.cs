using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public bool cam;
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (cam)
            {
                GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6;
            }
            else
            {
                other.gameObject.GetComponent<PlayerScript>().die(false);
            }
        }
    }
}
