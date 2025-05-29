using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoKnockbackFarRangeGun : MonoBehaviour
{
    public DetectHit detectHit;

    // Start is called before the first frame update
    void Start()
    {
        detectHit.knockbackFormula = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
