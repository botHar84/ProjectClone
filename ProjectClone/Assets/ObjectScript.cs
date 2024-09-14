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
    public ObjectScript subject;
    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Clone")
        {
            if (type == "Lever")
            {
                if (!on)
                {
                    turnon();
                    subject.turnon();
                }
            }
            else if (type == "Plate")
            {
                turnon();
                subject.turnon();
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
                turnoff();
                subject.turnoff();
            }
        }
    }
    public void turnon()
    {
        on = true;
        sr.sprite = activated;
        if (type == "Door")
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void turnoff()
    {
        on = false;
        sr.sprite = unactivated;
        if (type == "Door")
        {
            GetComponent<BoxCollider2D>().enabled = true;
        }
    }
}
