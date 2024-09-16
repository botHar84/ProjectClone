using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public string type;
    public bool on;
    public bool default_;
    public Sprite unactivated;
    public Sprite activated;
    public SpriteRenderer sr;
    public float amnt;
    public ObjectScript subject;
    public Vector3 og;
    public Vector3 change;
    public void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        if (type == "Platform")
        {
            og = transform.position;
            StartCoroutine("move");
        }
        if (on)
        {
            default_ = on;
            turnon();
        }
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
            if (other.gameObject.tag == "Player")
            {
                // load next level & display "Level 5" (eg.)
                if (type == "EndDoor" && on)
                {
                    if (PlayerPrefs.GetInt("CurrentLevel")>PlayerPrefs.GetInt("Complete"))
                    {
                        PlayerPrefs.SetInt("Complete", PlayerPrefs.GetInt("CurrentLevel"));
                    }
                }
                else if (type == "TimeCrystal")
                {
                    other.gameObject.GetComponent<PlayerScript>().reversed = true;
                    other.gameObject.transform.Find("TimeParticles").GetComponent<ParticleSystem>().Play();
                }
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
    public IEnumerator move()
    {
        while (true)
        {
            while(on)
            {
                if (transform.position != og+change)
                {
                    transform.position = Vector3.Lerp(transform.position, og+change, amnt);
                }
                yield return new WaitForSeconds(.01f);
            }
            while (!on)
            {
                if (transform.position != og)
                {
                    transform.position = Vector3.Lerp(transform.position, og, amnt);
                }
                yield return new WaitForSeconds(.01f);
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.transform.position.y > transform.position.y)
        {
            collision.gameObject.transform.SetParent(this.gameObject.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.transform.position.y > transform.position.y)
        {
            collision.gameObject.transform.SetParent(null);
        }
    }
}
