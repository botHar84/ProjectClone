using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    public float hp;
    public float mhp;
    public float timer;
    public float timer_;
    public float count;
    public int last;
    public Animator an;
    public float[] pos;
    public GameObject spike;
    public GameObject randomspike;
    public float spikespeed;
    public GameObject player;
    public GameObject weakpoint;
    public GameObject reversedweakpoint;
    public GameObject shield;
    public bool shielded;
    public Slider bar;
    public TextMeshProUGUI countdown;
    public AudioClip music;
    public AudioClip bang;
    public AudioClip hurtsfx;
    public AudioClip attacksfx;
    public AudioClip tpsfx;
    // Start is called before the first frame update
    void Start()
    {
        tp();
        bar.gameObject.SetActive(true);
        bar.value = hp;
        bar.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (((float)(hp/mhp))*100)+"%";
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        timer_ += Time.deltaTime;
        if (timer > 20+hp)
        {
            timer = 0;
            an.SetTrigger("Teleport");
        }
        if (timer_ > 5+hp)
        {
            timer_ = 0;
            an.SetTrigger("Attack");
        }
        count -= Time.deltaTime;
        if ((int)count%60 <= 9)
        {
            countdown.text = (int)count/60+ ":0"+(int)count%60;
        }
        else
        {
            countdown.text = (int)count/60+ ":"+(int)count%60;
        }
        if (count <= 0)
        {
            SceneManager.LoadScene("SampleScene");
        }
        player = GameObject.Find("Player");
        if (player.transform.position.x > transform.position.x && an.gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            an.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (player.transform.position.x < transform.position.x && !an.gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            an.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    public void tp()
    {
        SoundFXManager.instance.PlaySoundFXClip(tpsfx, gameObject, 1f);
        timer = 0;
        int random = 0;
        if (hp > 0)
        {
            random = UnityEngine.Random.Range(0, 3);
            while(random == last)
            {
                random = UnityEngine.Random.Range(0, 3);
            }
        }
        else
        {
            random = 1;
        }
        transform.localPosition = new UnityEngine.Vector2(pos[random], transform.localPosition.y);
        ws();
        last = random;
    }
    public void attack()
    {
        if (hp > 0)
        {
            ws();
            SoundFXManager.instance.PlaySoundFXClip(attacksfx, gameObject, .25f);
            timer_ = 0;
            int randomatk = UnityEngine.Random.Range(0, 3);
            if (randomatk == 0)
            {
                GameObject s = Instantiate(spike, player.transform.position+new UnityEngine.Vector3(0, 5, 0), quaternion.identity);
                s.GetComponent<ObjectScript>().amnt = spikespeed;
                s.GetComponent<ObjectScript>().change = new UnityEngine.Vector3(0, -10, 0);
                s.GetComponent<ObjectScript>().turnon();
                Destroy(s, 1.5f);
            }
            else
            {
                for (int i = 0; i < 6-hp; i++)
                {
                    int x = UnityEngine.Random.Range(-2, 2);
                    int y = UnityEngine.Random.Range(0, 3);
                    GameObject s = Instantiate(randomspike, player.transform.position+new UnityEngine.Vector3(x, y, 0), quaternion.identity);
                    Destroy(s, 1.5f);
                }
            }
            if (hp <= 3)
            {
                if (!shielded)
                {
                    int random = UnityEngine.Random.Range(0, 4);
                    if (random == 0)
                    {
                        shielded = true;
                        shield.SetActive(true);
                    }
                }
            }
        }
    }
    public void hurt()
    {
        if (!shielded)
        {
            hp -= 1;
            bar.value = hp;
            bar.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (hp/mhp*100)+"%";
            if (hp <= 0)
            {
                GameObject.Find("EndDoor").GetComponent<ObjectScript>().turnon();
                an.SetTrigger("Shrink");
            }
            else
            {
                SoundFXManager.instance.PlaySoundFXClip(hurtsfx, gameObject, 1f);
                if (hp == 1)
                {
                    GameObject.Find("Global Light 2D").GetComponent<Light2D>().color = new Color(1, 117/255, 117/255, 1);
                    GameObject.Find("Global Light 2D").GetComponent<Light2D>().intensity = .5f;
                    GameObject.Find("Level 10").GetComponent<AudioSource>().clip = music;
                    GameObject.Find("Level 10").GetComponent<AudioSource>().Play();
                    SoundFXManager.instance.PlaySoundFXClip(bang, gameObject, 1f);
                }
                an.SetTrigger("Hurt");
            }
        }
        else
        {
            an.SetTrigger("Teleport");
        }
    }
    public void ws()
    {
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Weakpoint"))
        {
            Destroy(g);
        }
        int random_ = UnityEngine.Random.Range(0, (int)hp+1);
        float x = UnityEngine.Random.Range(-2, 2);
        float y = UnityEngine.Random.Range(3, 5);
        if (random_ == 0)
        {
            Instantiate(reversedweakpoint, transform.position+new UnityEngine.Vector3(x, y), quaternion.identity);
        }
        else
        {
            Instantiate(weakpoint, transform.position+new UnityEngine.Vector3(x, y), quaternion.identity);
        }
    }
}
