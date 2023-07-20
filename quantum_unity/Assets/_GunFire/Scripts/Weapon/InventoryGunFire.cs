using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGunFire : MonoBehaviour
{
    #region Field
    private WeaponGunFire currentWeapon;
    #endregion

    #region Field Serialize
    [SerializeField]
    private WeaponGunFire[] listWeapon;
    [SerializeField]
    private EntityView entityView;
    #endregion

    #region Getter
    public WeaponGunFire GetCurrentWeapon() => currentWeapon;
    #endregion
    public void OnInit()
    {
        currentWeapon = listWeapon[0];
        QuantumEvent.Subscribe<EventOnFire>(this, OnFire);
        QuantumEvent.Subscribe<EventOnReload>(this, OnReload);
    }

    private void OnFire(EventOnFire callback)
    {
        if (entityView.EntityRef == callback.entityRef)
        {
            currentWeapon.OnFire();
        }
    }

    private void OnReload(EventOnReload callback)
    {
        if (entityView.EntityRef == callback.entityRef)
        {
            currentWeapon.OnReload(callback);
        }
    }

    private void Update()
    {
        if(entityView.EntityRef == GameManager.LocalPlayerEntity)
            LocalInput.fireAnchor = currentWeapon.GetFireAnchor();
    }
}
