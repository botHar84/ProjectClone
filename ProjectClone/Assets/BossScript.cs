using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BossScript : MonoBehaviour
{
    public float hp;
    public float mhp;
    public float timer;
    public int last;
    public Animator an;
    public float[] pos;
    public GameObject player;
    public GameObject weakpoint;
    public bool shield;
    public Slider bar;
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
        if (timer > 30)
        {
            an.SetTrigger("Teleport");
        }
        player = GameObject.Find("Player");
        if (player.transform.position.x > transform.position.x && gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (player.transform.position.x < transform.position.x && !gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    public void tp()
    {
        timer = 0;
        int random = UnityEngine.Random.Range(0, 3);
        while(random == last)
        {
            random = UnityEngine.Random.Range(0, 3);
        }
        transform.localPosition = new Vector2(pos[random], transform.localPosition.y);
        float x = UnityEngine.Random.Range(-2, 2);
        float y = UnityEngine.Random.Range(2, 6);
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Weakpoint"))
        {
            Destroy(g);
        }
        Instantiate(weakpoint, transform.position+new Vector3(x, y), quaternion.identity);
        last = random;
    }
    public void attack()
    {

        if (hp <= 3) // chance of shield
        {

        }
    }
    public void hurt()
    {
        hp -= 1;
        bar.value = hp;
        bar.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (hp/mhp*100)+"%";
        if (hp <= 0)
        {
            GameObject.Find("EndDoor").GetComponent<ObjectScript>().turnon();
        }
        else
        {
            an.SetTrigger("Hurt");
        }
    }
}
