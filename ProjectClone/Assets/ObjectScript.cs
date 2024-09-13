using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public string type;
    public bool on;
    public Sprite unactivated;
    public Sprite activated;
    public SpriteRenderer sr;
    public int amnt;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Clone")
        {
            if (type == "Lever")
            {
                if (!on)
                {
                    on = true;
                    sr.sprite = activated;
                }
            }
            else if (type == "Plate")
            {
                on = true;
                sr.sprite = activated;
                amnt++;
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (type == "Plate" && (other.gameObject.tag == "Player" || other.gameObject.tag == "Clone"))
        {
            amnt--;
            if (amnt <= 0)
            {
                off();
            }
        }
    }
    public void off()
    {
        on = false;
        sr.sprite = unactivated;
    }
}
