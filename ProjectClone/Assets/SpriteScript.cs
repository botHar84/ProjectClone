using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteScript : MonoBehaviour
{
    public BossScript bs;
    public void atk()
    {
        bs.attack();
    }
    public void tp()
    {
        bs.tp();
    }
    public void end()
    {
        bs.endscene();
    }
}
