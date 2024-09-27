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

    [Header("SFX")]
    public AudioClip spikeCeilingSFX;
    public AudioClip platformMovingSFX;
    public AudioClip pressurePlateUpSFX;
    public AudioClip pressurePlateDownSFX;
    public AudioClip leverOnSFX;
    public AudioClip leverOffSFX;
    public AudioClip doorOpenSFX;
    public AudioClip timeCrystalSFX;

    public void Start()
    {
        if (name != "SpikeCeiling")
        {
            sr = GetComponent<SpriteRenderer>();
        }
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
                    SoundFXManager.instance.PlaySoundFXClip(leverOnSFX, gameObject, 1f);
                    //subject.turnon();
                }
            }
            else if (type == "Plate")
            {
                turnon();
                //subject.turnon();
                amnt++;
            }
            if (other.gameObject.tag == "Player")
            {
                if (type == "EndDoor" && on)
                {
                    if (PlayerPrefs.GetInt("CurrentLevel") > PlayerPrefs.GetInt("HighestLevel"))
                    {
                        PlayerPrefs.SetInt("HighestLevel", PlayerPrefs.GetInt("CurrentLevel"));
                    }
                    if (PlayerPrefs.GetInt("CurrentLevel") < 10)
                    {
                        PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel")+1);
                        other.gameObject.GetComponent<PlayerScript>().LoadLevel();
                    }
                    else
                    {
                        // win screen
                    }
                }
                else if (type == "TimeCrystal" && !other.gameObject.GetComponent<PlayerScript>().reversed)
                {
                    other.gameObject.GetComponent<PlayerScript>().reversed = true;
                    other.gameObject.transform.Find("TimeParticles").GetComponent<ParticleSystem>().Play();
                    SoundFXManager.instance.PlaySoundFXClip(timeCrystalSFX, gameObject, 1f);
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
                //subject.turnoff();
            }
        }
    }
    public void turnon()
    {
        on = true;
        if (type == "Plate" || type == "Lever")
        {
            if (subject != null)
            {
                subject.turnon();
            }
            else if (PlayerPrefs.GetInt("CurrentLevel") == 10)
            {
                if (GameObject.Find("Boss").GetComponent<BossScript>().shielded)
                {
                    GameObject.Find("Boss").GetComponent<BossScript>().shielded = false;
                    GameObject.Find("Boss").GetComponent<BossScript>().shield.SetActive(false);
                }
            }
        }
        if (type == "Lever" || type == "EndDoor")
        {
            gameObject.GetComponent<Animator>().SetTrigger("On");
            SoundFXManager.instance.PlaySoundFXClip(doorOpenSFX, gameObject, 1f);
        }
        else if (type == "Plate")
        {
            sr.sprite = activated;
            SoundFXManager.instance.PlaySoundFXClip(pressurePlateDownSFX, gameObject, 1f);
        }
        if (type == "Door")
        {
            GetComponent<BoxCollider2D>().enabled = false;
        }
        if (gameObject.tag == "SpikeCeiling")
        {
            SoundFXManager.instance.PlaySoundFXClip(spikeCeilingSFX, gameObject, 1f);
        }
    }
    public void turnoff()
    {
        on = false;
        if (name == "Plate" || name == "Lever")
        {
            if (subject != null)
            {
                subject.turnoff();
            }
        }
        if (name == "Lever" || name == "EndDoor")
        {
            gameObject.GetComponent<Animator>().SetTrigger("Off");
            SoundFXManager.instance.PlaySoundFXClip(leverOffSFX, gameObject, 1f);
        }
        else if (name == "Plate")
        {
            sr.sprite = unactivated;
            SoundFXManager.instance.PlaySoundFXClip(pressurePlateUpSFX, gameObject, 1f);
            if (SoundFXManager.instance.IsPlaying(platformMovingSFX))
            {
                SoundFXManager.instance.StopSoundFXClip(platformMovingSFX);
            }
        }
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
                if (!(gameObject.tag == "SpikeCeiling"))
                {
                    if (!SoundFXManager.instance.IsPlaying(platformMovingSFX))
                    {
                        SoundFXManager.instance.PlaySoundFXClip(platformMovingSFX, gameObject, 1f);
                    }
                }
                
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
        if (type == "Platform" && collision.gameObject.tag == "Player" && collision.gameObject.transform.position.y > transform.position.y && name != "SpikeCeiling")
        {
            if (gameObject.activeInHierarchy)
            {
                collision.gameObject.transform.SetParent(this.gameObject.transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (type == "Platform" && collision.gameObject.tag == "Player" && collision.gameObject.transform.position.y > transform.position.y && name != "SpikeCeiling")
        {
            if (gameObject.activeInHierarchy)
            {
                collision.gameObject.transform.SetParent(null);
            }
        }
    }
}
