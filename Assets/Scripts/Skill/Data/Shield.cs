using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shield : NetworkBehaviour
{
    [Networked] public int currentDamageAbsorb {  get; set; }
    [Networked] public int maxDamageAbsorb { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority)
        {

        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (currentDamageAbsorb <= 0)
        {
           Destroy(gameObject);
        }
    }
    public void ReduceCurrentDamageAbsorb(int damageAbsorb, out int overBalanceDamage)
    {
        if(currentDamageAbsorb>= damageAbsorb)
        {
            currentDamageAbsorb -= damageAbsorb;
            overBalanceDamage=0;
        }
        else
        {
            overBalanceDamage = damageAbsorb - currentDamageAbsorb;
            currentDamageAbsorb = 0;
        }
        
    }
}


