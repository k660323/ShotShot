using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalScript : SkillScript
{
    public float CriticalRate;

    public float Critical()
    {
        return (Random.Range(0f, 1f) <= CriticalRate) ? 2f : 1f;
    }
}
