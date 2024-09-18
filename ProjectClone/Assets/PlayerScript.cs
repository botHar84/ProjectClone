using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class Frame
{
    public UnityEngine.Vector2 pos;
    public float timeStamp;
}
public class PlayerScript : MonoBehaviour
{
    public float horizontal;
    public float speed = 8f;
    public float jump = 16f;
    public bool right = true;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject cameraobj;
    public float cameraspeed;
    public int checkpoint;
    public List<Frame> frames = new List<Frame>();
    public List<Frame> current = new List<Frame>();
    public float time;
    public GameObject cloneobj;
    public int count;
    public bool reversed;
    void Start()
    {
        print(PlayerPrefs.GetInt("CurrentLevel"));
    }
    void Update()
    {
        time += Time.deltaTime;
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && grounded())
        {
            rb.velocity = new UnityEngine.Vector2(rb.velocity.x, jump);
        }
        if (Time.timeScale == 1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                die();
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                die();
                StopCoroutine("clone");
                current.Clear();
                frames.Clear();
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

        if (right && horizontal < 0f || !right && horizontal > 0f)
        {
            right = !right;
            UnityEngine.Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

        cameraobj.transform.position = UnityEngine.Vector3.Lerp(cameraobj.transform.position, new UnityEngine.Vector3(transform.position.x, transform.position.y, -10), cameraspeed);
        
        if (count==5)
        {
            current.Add(new Frame{pos = transform.position, timeStamp = time});
            count=1;
        }
        else
        {
            count++;
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new UnityEngine.Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool grounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
    public void die()
    {
        rb.velocity = new UnityEngine.Vector2(0, 0);
        transform.Find("TimeParticles").GetComponent<ParticleSystem>().Stop();
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Clone"))
        {
            if (g != null)
            {
                Destroy(g);
            }
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Lever"))
        {
            g.GetComponent<ObjectScript>().turnoff();
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("Door"))
        {
            if (!g.GetComponent<ObjectScript>().default_)
            {
                g.GetComponent<ObjectScript>().turnoff();
            }
        }
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("SpikeCeiling"))
        {
            g.transform.position = g.GetComponent<ObjectScript>().og;
            g.GetComponent<ObjectScript>().turnoff();
        }
        frames.Clear();
        foreach (Frame f in current)
        {
            frames.Add(f);
        }
        current.Clear();
        GameObject point = GameObject.Find("Point"+checkpoint);
        transform.position = point.transform.position;
        StartCoroutine("clone", reversed);
        reversed = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Point")
        {
            int order = int.Parse(other.gameObject.name.Substring(5, 1));
            if (order > checkpoint)
            {
                checkpoint = order;
            }
        }
    }
    public IEnumerator clone(bool reverse)
    {
        yield return new WaitForSeconds(1);
        GameObject point = GameObject.Find("Point"+checkpoint);
        GameObject current = Instantiate(cloneobj, point.transform.position, quaternion.identity);
        if (reverse)
        {
            current.GetComponent<SpriteRenderer>().color = Color.blue;
            for (int i = frames.Count-2; i > 0; i--)
            {
                if (frames.Count > i)
                {
                    Frame startFrame = frames[i];
                    Frame endFrame = frames[i - 1];
                    float elapsedTime = 0f;
                    float timeBetweenFrames = startFrame.timeStamp - endFrame.timeStamp;
                    while (elapsedTime < timeBetweenFrames)
                    {
                        if (current != null)
                        {
                            current.transform.position = UnityEngine.Vector2.Lerp(startFrame.pos, endFrame.pos, elapsedTime / timeBetweenFrames);
                        }
                        else
                        {
                            break;
                        }
                        elapsedTime += Time.deltaTime*.7f;
                        yield return null;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < frames.Count - 1; i++)
            {
                Frame startFrame = frames[i];
                Frame endFrame = frames[i + 1];
                float elapsedTime = 0f;
                float timeBetweenFrames = endFrame.timeStamp - startFrame.timeStamp;

                while (elapsedTime < timeBetweenFrames)
                {
                    if (current != null)
                    {
                        current.transform.position = UnityEngine.Vector2.Lerp(startFrame.pos, endFrame.pos, elapsedTime / timeBetweenFrames);
                    }
                    else
                    {
                        break;
                    }
                    elapsedTime += Time.deltaTime*.7f;
                    yield return null;
                }
            }
        }
        Destroy(current);
    }
}
