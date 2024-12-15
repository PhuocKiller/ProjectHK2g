using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICanTakeDamage
{
    public void ApplyDamage(int damage, bool isPhysicDamage, PlayerController playerMakeDamage,
        Action<int,bool> counter = null, Action<int, List<PlayerController> > isKillPlayer = null,
        Action<Vector3,float> isKillCreep = null,
        Action<int> lifeSteal=null, bool activeInjureAnim = true, bool isCritPhysic=false);
    public void ApplyEffect(PlayerRef player, bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false,
        float TimeEffect = 0f,  Action callback = null);
}
