using GenifyStudio.Scripts.Manager;
using InfimaGames.LowPolyShooterPack;
using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGunFire : MonoBehaviour
{
    #region FIELDS SERIALIZE
    [SerializeField]
    private Transform fireAnchor;
    [SerializeField]
    private MuzzleBehaviour muzzleBehaviour;
    [SerializeField]
    private AudioSource fireAudio;
    [SerializeField]
    private AudioSource reloadAudio;
    [SerializeField]
    private AudioClip[] reloadClip;
    #endregion

    #region FIELDS

    /// <summary>
    /// Weapon Animator.
    /// </summary>
    private Animator animator;
    private EntityRef entityRef;


    #endregion

    #region GETTER
    public Transform GetFireAnchor() => fireAnchor;
    #endregion
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OnInit(QuantumGame game, EntityRef entityRef)
    {
        this.entityRef = entityRef;
    }

    public void OnReload(EventOnReload callback)
    {
        const string boolName = "Reloading";
        animator.SetBool(boolName, true);
        animator.Play(false ? "Reload Open" : (callback.hasAmmo ? "Reload" : "Reload Empty"), 0, 0.0f);
        if (!SettingConfig.Instance.IsSfxOn) return;
        if (callback.hasAmmo)
        {
            reloadAudio.PlayOneShot(reloadClip[0]);
        }
        else
        {
            reloadAudio.PlayOneShot(reloadClip[1]);
        }
    }

    //private void OnFire(EventOnFire callback)
    //{
    //    if(entityRef == callback.entityRef)
    //    {
    //        muzzleBehaviour.Effect();
    //        const string stateName = "Fire";
    //        animator.Play(stateName, 0, 0.0f);
    //    }

    //}
    public void OnFire()
    {
        muzzleBehaviour.Effect();
        const string stateName = "Fire";
        animator.Play(stateName, 0, 0.0f);
        if (!SettingConfig.Instance.IsSfxOn) return;
        fireAudio.Play();
    }
}
