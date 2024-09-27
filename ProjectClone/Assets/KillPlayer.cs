using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class KillPlayer : MonoBehaviour
{
    public bool cam;
    public int weak;
    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (cam)
            {
                GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 6;
            }
            else if (weak == 1)
            {
                GameObject.Find("Boss").GetComponent<BossScript>().hurt();
                Destroy(gameObject);
            }
            else if (weak == 2)
            {
                if (other.gameObject.GetComponent<PlayerScript>().reversed)
                {
                    GameObject.Find("Boss").GetComponent<BossScript>().hurt();
                    Destroy(gameObject);
                }
            }
            else
            {
                other.gameObject.GetComponent<PlayerScript>().die(false);
            }
        }
        else if (other.gameObject.tag == "Clone")
        {
            if (weak == 1) // any
            {
                GameObject.Find("Boss").GetComponent<BossScript>().hurt();
                Destroy(gameObject);
            }
            else if (weak == 2)
            {
                if (other.gameObject.name == "ReversedClone")
                {
                    GameObject.Find("Boss").GetComponent<BossScript>().hurt();
                    Destroy(gameObject);
                }
            }
        }
    }
}
