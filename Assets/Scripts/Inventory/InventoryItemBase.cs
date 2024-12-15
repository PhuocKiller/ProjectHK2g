using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : NetworkBehaviour, IInventoryItem
{
    public bool isSpawn;
    public override void Spawned()
    {
        base.Spawned();
        if (isSpawn) return;
        isSpawn = true;
        GetComponent<Collider>().enabled = HasStateAuthority;
        MeshRenderer[] meshItems= transform.GetComponentsInChildren<MeshRenderer>();
        foreach(var mesh in meshItems)
        {
            mesh.enabled = HasStateAuthority;
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }
    public virtual string Name
    {
        get;
    }
    
    public Sprite _Image;
    public Sprite Image
    {
        get
        {
            return _Image;
        }
    }
    public int _Price;
    public int Price
    {
        get
        {
            return _Price;
        }
    }
    public bool isDecreaseAlpha, isIncreaseAlpha;
    public virtual ItemTypes itemTypes { get; set; }
    public InventorySlot Slot
    {
        get; set;
    }
    public SkillName skillName;
    public virtual int maxStack
    {
        get { return 1; }
    }
    public virtual string Info {  get; set; }
    public void Start()
    {
      //  StartCoroutine(FadeItemNoPick());
    }
    public void FixedUpdate()
    {
        if (isDecreaseAlpha)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GetComponent<SpriteRenderer>().color.a - 0.1f);
            if (GetComponent<SpriteRenderer>().color.a < 0.2f)
            {
                isDecreaseAlpha = false;
                isIncreaseAlpha = true;
            }
        }
        if (isIncreaseAlpha)
        {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GetComponent<SpriteRenderer>().color.a + 0.1f);
            if (GetComponent<SpriteRenderer>().color.a > 1)
            {
                isDecreaseAlpha = true;
                isIncreaseAlpha = false;
            }
        }
    }
    public IEnumerator FadeItemNoPick()
    {
        yield return new WaitForSeconds(2);
        isDecreaseAlpha = true;
        StartCoroutine(DestroyItemNoPick());
    }
    public IEnumerator DestroyItemNoPick()
    {
        yield return new WaitForSeconds(2);

        Destroy(gameObject);
    }
   
    public virtual void OnPickUp()
    {
        Destroy(gameObject);
    }
    
    public virtual void OnDrop()
    {
        
    }
    public virtual ItemTypes GetItemTypes()
    {
        return itemTypes;
    }
    public virtual void OnUse(int indexSlot)
    {

    }
    public InventoryItemBase Clone()
    {
        return (InventoryItemBase)this.MemberwiseClone();
    }

}
