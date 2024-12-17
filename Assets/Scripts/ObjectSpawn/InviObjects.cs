using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InviObjects : NetworkBehaviour
{
    PlayerController player;
    private Vector3 direction;
    private NetworkRigidbody rb;
    private List<Collider> collisions = new List<Collider>();
    public TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        if (HasStateAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
        }
    }
    public void SetUp(PlayerController player, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, bool isDestroyWhenCollider = false)
    {
        this.player = player;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDespawn = timeTrigger;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!HasStateAuthority) return;
        if (timer.ExpiredOrNotRunning(Runner) ||(player.playerStat.isVisible==false&& player.state!=0))
        {
           StartCoroutine(VisibleAgain());
        }

    }
    IEnumerator VisibleAgain()
    {
        yield return new WaitForSeconds(0.5f);
        player.playerStat.isVisible = true;
        player.playerStat.isStartFadeInvi=false;
       // BackDefaultMatRPC();
        Destroy(gameObject);
    }
    [Rpc(RpcSources.All, RpcTargets.All)] public void BackDefaultMatRPC()
    {
        player.statusCanvas.GetComponent<InviManager>().BackDefaultMaterial();
    }
    /*public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        if (gameObject != null) player.playerStat.isVisible = true;
    }
    private void OnDestroy()
    {
        if (gameObject != null) player.playerStat.isVisible = true;
    }*/
}
