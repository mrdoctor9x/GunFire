using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : QuantumCallbacks
{
    [SerializeField]
    TextMeshProUGUI magazineTxt;
    [SerializeField]
    TextMeshProUGUI hpTxt;
    [SerializeField]
    GameObject deadPanel;
    [SerializeField]
    Image OnDamageUI;

    Color OnDamageColor;
    private void Awake()
    {
        OnDamageColor = OnDamageUI.color;
        QuantumEvent.Subscribe<EventOnDamage>(this, OnDamage);
    }

    private void OnDamage(EventOnDamage callback)
    {
        if(callback.entityRef == GameManager.LocalPlayerEntity)
        {
            OnDamageColor.a = 1;
        }

    }
    private void Update()
    {
        OnDamageColor.a = Mathf.Lerp(OnDamageColor.a, 0, Time.deltaTime * 5);
        OnDamageUI.color = OnDamageColor;
    }

    public override void OnUpdateView(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        if(f.TryGet<WeaponInfo>(GameManager.LocalPlayerEntity,out var weaponInfo))
        {
            var mag = weaponInfo.Magazine;
            magazineTxt.text = $"{mag}/30";
        }
        if (f.TryGet<HealthCompoment>(GameManager.LocalPlayerEntity, out var healthCompoment))
        {
            var hp = healthCompoment.hp;
            hpTxt.text = $"HP:{hp}";
            deadPanel.SetActive(healthCompoment.isDead);
        }

    }
}
