using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeController : NetworkBehaviour
{
    PlayerController player;
    private Vector3 direction, velocity;
    private NetworkRigidbody rb;
    private List<Collider> collisions = new List<Collider>();
    private TickTimer timer;
    public float timerDespawn, timeEffect;
    public int damage;
    public bool isPhysicDamage, isMakeStun, isMakeSlow, isMakeSilen, isDestroyWhenCollider;
    public float throwForce, heightRigid, velocityDecrease;
    Vector3? posMouseUp;
    bool isActiveBomb;
    public override void Spawned()
    {
        base.Spawned();
        collisions.Clear();
        rb = GetComponent<NetworkRigidbody>();
        if (HasStateAuthority && HasInputAuthority)
        {
            timer = TickTimer.CreateFromSeconds(Runner, timerDespawn);
            
        }
    }
    public void ThrowGrenade()
    {
        if (rb != null &HasStateAuthority)
        {
                // Get the direction and distance to the target
                Vector3 targetDir = (Vector3)posMouseUp - transform.position;
                // Calculate the horizontal distance (ignore y axis to focus on xz plane)
                float horizontalDistance = new Vector3(targetDir.x, 0, targetDir.z).magnitude;
                // Calculate the required velocity for the throw to reach the target
                float timeToTarget = horizontalDistance / throwForce;  // Time it takes to reach the target horizontally
                float verticalVelocity = Mathf.Abs(-20f) * timeToTarget / 2.2f; // Initial velocity to reach the target height

                // Calculate the initial velocity components
                Vector3 velocity = targetDir.normalized * throwForce;
                velocity.y = verticalVelocity; // Set the vertical component of the velocity
            rb.Rigidbody.velocity = velocity;
            transform.forward = direction;
            gameObject.transform.SetParent(null);
           rb.Rigidbody.useGravity = true;
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
        if (HasStateAuthority && timer.Expired(Runner)
            )
        {
            Destroy(gameObject);
        }
        
      //  velocity += new Vector3(direction.x*forceRigid, velocityDecrease * Runner.DeltaTime, direction.z * forceRigid) ;
       // rb.Rigidbody.velocity = velocity * Runner.DeltaTime;
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!HasStateAuthority || isActiveBomb) return;
        if ((collision.gameObject.layer == 7
            && collisions.Count == 0
            && collision.gameObject.GetComponent<NetworkObject>().HasStateAuthority == false)
           || collision.gameObject.layer == 3)

        {
            isActiveBomb=true;
            StartCoroutine(DelayGrenadeActive());
        }
    }
    IEnumerator DelayGrenadeActive()
    {
        yield return new WaitForSeconds(0.5f);
        NetworkObject obj = Runner.Spawn(player.networkObjs.listNetworkObj[5], transform.position,
                transform.rotation, Object.InputAuthority,
            onBeforeSpawned: (NetworkRunner runner, NetworkObject obj) =>
            {
                obj.GetComponent<AttackObjects>().SetUpPlayer(player, damage, isPhysicDamage, null,
             isMakeStun, isMakeSlow, isMakeSilen, timerDespawn, timeEffect);
            });
        Destroy(gameObject,0.1f);
    }
}
