using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : NetworkBehaviour
{
    [Header("Target References")]
    [SerializeField] private Transform handSlotTransform;
    [SerializeField] private Transform weaponTransform;

    [Header("Weapon Settings")]
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;

    private void Start()
    {
        if (!handSlotTransform || !weaponTransform)
        {
            Debug.LogWarning("Please assign hand slot and weapon transforms in the inspector");
            enabled = false;
            return;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            UpdateWeaponTransform();
        }
    }

    private void UpdateWeaponTransform()
    {
        weaponTransform.position = handSlotTransform.position; //+ handSlotTransform.TransformDirection(positionOffset);
        weaponTransform.rotation = handSlotTransform.rotation; //* Quaternion.Euler(rotationOffset);
    }
}
