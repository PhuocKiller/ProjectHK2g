using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeAttackObject: NetworkBehaviour
{
    PlayerController player;
    CalculateTriggerEnter trigger;
    private Vector3 direction, velocity;
    private NetworkRigidbody rb;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public float throwForce, heightRigid, velocityDecrease;
    Vector3? posMouseUp; Vector3 targetDir;
    bool isActiveBomb;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        rb = GetComponent<NetworkRigidbody>();
        trigger = GetComponent<CalculateTriggerEnter>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            ThrowAxe();
        }
    }
    public void ThrowAxe()
    {
        if (rb != null & HasStateAuthority)
        {
            Vector3 targetPosMouseUp= new Vector3(((Vector3)posMouseUp).x,transform.position.y, ((Vector3)posMouseUp).z);
            // Get the direction and distance to the target
            targetDir = targetPosMouseUp - transform.position;
            // Calculate the horizontal distance (ignore y axis to focus on xz plane)
            float horizontalDistance = new Vector3(targetDir.x, 0, targetDir.z).magnitude;
            float initialVelocity = Mathf.Sqrt(2 * Mathf.Abs(20) * horizontalDistance);
            float timeToTarget =initialVelocity /20f;
            timer = TickTimer.CreateFromSeconds(Runner,2* timeToTarget);
            velocity = targetDir.normalized * initialVelocity;
            rb.Rigidbody.velocity = velocity;
            transform.forward = direction;
            transform.SetParent(null);
        }
    }

    public void SetUp(PlayerController player, int levelDamage, bool isPhysicDamage, Transform parentObject = null,
        bool isMakeStun = false, bool isMakeSlow = false, bool isMakeSilen = false, float timeTrigger = 0f,
        float timeEffect = 0f, Vector3? posMouseUp = null, bool isDestroyWhenCollider = false)
    {
        this.player = player;
        transform.SetParent(parentObject);
        damage = levelDamage;
        this.isPhysicDamage = isPhysicDamage;
        this.isMakeStun = isMakeStun;
        this.isMakeSlow = isMakeSlow;
        this.isMakeSilen = isMakeSilen;
        this.timeEffect = timeEffect;
        this.posMouseUp = posMouseUp;
        this.isDestroyWhenCollider = isDestroyWhenCollider;
        timerDespawn = timeTrigger;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!HasStateAuthority) return;
        if( timer.Expired(Runner))
        {
            Destroy(gameObject);
            player.GetComponent<Viking>().ActiveAxeRPC();
        }
        Vector3 dragForce = -targetDir.normalized * Mathf.Abs(20); 
        rb.Rigidbody.AddForce(dragForce, ForceMode.Acceleration);
        if (rb.Rigidbody.velocity.magnitude < 0.3f) 
        {
            rb.Rigidbody.velocity = -rb.Rigidbody.velocity;  // Đảo chiều vận tốc
        }

    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            if (other.gameObject.layer == 7 && !collisions.Contains(other)
            && other.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false
            && other.gameObject.GetComponent<PlayerController>().state != 3
            && other.gameObject.GetComponent<PlayerController>().playerTeam != player.playerTeam)
            {
                trigger.ControlTriggerPlayer(other, collisions, player, damage, timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority);
            }
            if (other.gameObject.layer == 8 && !collisions.Contains(other)
            && other.gameObject.GetComponent<CreepController>().state != 3
            && other.gameObject.GetComponent<CreepController>().playerTeam != player.playerTeam)
            {
                trigger.ControlTriggerPlayer(other, collisions, player, damage, timeEffect, isPhysicDamage,
                isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider, Object.InputAuthority);
                if (player.playerType == Player_Types.DumbleDore) collisions.Clear();
            }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (HasStateAuthority
           && other.gameObject.layer == 7
           && other.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false)
                   {
            collisions.Remove(other);
        }
    }

}
