using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Cinemachine;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
 
[System.Serializable]
public class Frame
{
    public UnityEngine.Vector2 pos;
    public float timeStamp;
    public int jump;
    public float speed;
    public float height;
    public float dir;
}
public class PlayerScript : MonoBehaviour
{
    public Transform playerTransform;
    public float horizontal;
    public float speed = 8f;
    public float jump = 16f;
    public bool right = true;
    public Rigidbody2D rb;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject cameraobj;
    public int checkpoint;
    public List<Frame> frames = new List<Frame>();
    public List<Frame> current = new List<Frame>();
    public float time;
    public GameObject cloneobj;
    public int count;
    public bool reversed;
    public GameObject[] levels;
    public CanvasScript cs;
    public Animator an;
    public GameObject deathsprite;
    private bool wasGrounded;

    [Header("SFX")]
    public AudioClip footFallSFX;
    public AudioClip jumpSFX;
    public AudioClip landSFX;
    public AudioClip checkpointSFX;
    public AudioClip dyingSFX;
    public AudioClip spawningSFX;
    public AudioClip spikeCeilingSFX;

    [Header("VFX")]
    public ParticleSystem footFallVFX;
    public ParticleSystem jumpVFX;
    public ParticleSystem landVFX;

    void Start()
    {
        print(PlayerPrefs.GetInt("CurrentLevel"));
        LoadLevel();
    }
    void Update()
    {
        time += Time.deltaTime;
        bool isGrounded = grounded();
        if (!wasGrounded && isGrounded){
            SoundFXManager.instance.PlaySoundFXClip(landSFX, playerTransform, 1f);
            landVFX.Play();
        }

        wasGrounded = isGrounded;
        horizontal = Input.GetAxisRaw("Horizontal");
        if (grounded() && Mathf.Abs(horizontal) > 0.1f)
        {
            if (!SoundFXManager.instance.IsPlaying(footFallSFX))  // Check if footstep sound is already playing
            {
                SoundFXManager.instance.PlaySoundFXClip(footFallSFX, playerTransform, 0.5f);
                footFallVFX.Play();
            }
        }
        else
        {
            SoundFXManager.instance.StopSoundFXClip(footFallSFX);  // Stop footsteps when not moving
            footFallVFX.Stop();
        }
        if (Input.GetButtonDown("Jump") && grounded())
        {
            rb.velocity = new UnityEngine.Vector2(rb.velocity.x, jump);
            an.SetTrigger("Jump");
            jumpVFX.Play();
            SoundFXManager.instance.PlaySoundFXClip(jumpSFX, playerTransform, 1f);
            current.Add(new Frame{pos = transform.position, timeStamp = time, jump = 1, height = rb.velocity.y, speed = Mathf.Abs(rb.velocity.x), dir = transform.localScale.x});
        }

        an.SetFloat("Height", rb.velocity.y);
        an.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        if (Time.timeScale == 1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                die(true);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                die(true);
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
        
        if (count==5)
        {
            current.Add(new Frame{pos = transform.position, timeStamp = time, jump = 0, height = rb.velocity.y, speed = Mathf.Abs(rb.velocity.x), dir = transform.localScale.x});
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
        Collider2D[] objs = Physics2D.OverlapCircleAll(groundCheck.position, .2f, groundLayer);
        bool slope = false;
        foreach (Collider2D c in objs)
        {
            if (c.tag == "Slope")
            {
               slope = true;
            }
        }
        if (slope)
        {
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 3;
        }
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    public void die(bool self)
    {
        if (PlayerPrefs.GetInt("CurrentLevel") == 8)
        {
            cameraobj.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 10;
        }
        GameObject d = Instantiate(deathsprite, transform.position, quaternion.identity);
        d.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Die");
        SoundFXManager.instance.PlaySoundFXClip(dyingSFX, playerTransform, 1f);
        Destroy(d, .8f);
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
            if (g.GetComponent<ObjectScript>().on)
            {
                g.GetComponent<ObjectScript>().turnoff();
            }
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
            if (!g.GetComponent<ObjectScript>().default_ || !self)
            {
                g.transform.position = g.GetComponent<ObjectScript>().og;
                g.GetComponent<ObjectScript>().turnoff();
            }
        }
        frames.Clear();
        foreach (Frame f in current)
        {
            frames.Add(f);
        }
        current.Clear();
        GameObject point = GameObject.Find("Point"+checkpoint);
        transform.position = point.transform.position;
        an.SetTrigger("Spawn");
        if (SoundFXManager.instance.IsPlaying(spikeCeilingSFX))
        {
            SoundFXManager.instance.StopSoundFXClip(spikeCeilingSFX);
        }
        SoundFXManager.instance.PlaySoundFXClip(spawningSFX, playerTransform, 1f);
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
                SoundFXManager.instance.PlaySoundFXClip(checkpointSFX, playerTransform, 1f);
                StartCoroutine("fade", other.gameObject);
                checkpoint = order;
            }
        }
    }
    public IEnumerator fade(GameObject obj)
    {
        while (obj.transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>().color != new Color(1, 1, 1))
        {
            obj.transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>().color = Color.Lerp(obj.transform.Find("Canvas").Find("Text").GetComponent<TextMeshProUGUI>().color, new Color(1, 1, 1),.5f);
            yield return new WaitForSeconds(.1f);
        }
    }
    public IEnumerator clone(bool reverse)
    {
        yield return new WaitForSeconds(1);
        GameObject point = GameObject.Find("Point"+checkpoint);
        GameObject current = Instantiate(cloneobj, point.transform.position, quaternion.identity);
        if (reverse)
        {
            current.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Die");
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
                            if (startFrame.dir < 0 && current.transform.localScale.y > 0)
                            {
                                current.transform.localScale = new UnityEngine.Vector3(current.transform.localScale.y, current.transform.localScale.y, current.transform.localScale.z);
                            }
                            else
                            {
                                current.transform.localScale = new UnityEngine.Vector3(current.transform.localScale.y*-1, current.transform.localScale.y, current.transform.localScale.z);
                            }
                            current.transform.Find("Sprite").GetComponent<Animator>().SetFloat("Height", startFrame.height);
                            current.transform.Find("Sprite").GetComponent<Animator>().SetFloat("Speed", startFrame.speed);
                            if (startFrame.jump == 1)
                            {
                                current.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Jump");
                            }
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
            if (current != null)
            {
                current.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Spawn");
            }
        }
        else
        {
            current.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Spawn");
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
                        if (startFrame.dir < 0 && current.transform.localScale.y > 0)
                        {
                            current.transform.localScale = new UnityEngine.Vector3(current.transform.localScale.y*-1, current.transform.localScale.y, current.transform.localScale.z);
                        }
                        else
                        {
                            current.transform.localScale = new UnityEngine.Vector3(current.transform.localScale.y, current.transform.localScale.y, current.transform.localScale.z);
                        }
                        current.transform.Find("Sprite").GetComponent<Animator>().SetFloat("Speed", startFrame.speed);
                        current.transform.Find("Sprite").GetComponent<Animator>().SetFloat("Height", startFrame.height);
                        if (startFrame.jump == 1)
                        {
                            current.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Jump");
                        }
                    }
                    else
                    {
                        break;
                    }
                    elapsedTime += Time.deltaTime*.7f;
                    yield return null;
                }
            }
            if (current != null)
            {
                current.transform.Find("Sprite").GetComponent<Animator>().SetTrigger("Die");
            }
        }
        Destroy(current, .8f);
    }
    public void LoadLevel()
    {
        rb.velocity = new UnityEngine.Vector2(0, 0);
        reversed = false;
        transform.Find("TimeParticles").GetComponent<ParticleSystem>().Stop();
        checkpoint = 0;
        frames.Clear();
        current.Clear();

        foreach(GameObject g in levels)
        {
            g.SetActive(false);
        }
        levels[PlayerPrefs.GetInt("CurrentLevel")-1].SetActive(true);
        if (PlayerPrefs.GetInt("CurrentLevel") == 10)
        transform.position = GameObject.Find("Point"+checkpoint).transform.position;
        cs.extHelp("Level "+PlayerPrefs.GetInt("CurrentLevel"));
        if (PlayerPrefs.GetInt("CurrentLevel") == 8)
        {
            cameraobj.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 10;
        }
        cameraobj.GetComponent<CinemachineConfiner>().m_BoundingShape2D = GameObject.Find("Camera bound").GetComponent<PolygonCollider2D>();
    }
}
