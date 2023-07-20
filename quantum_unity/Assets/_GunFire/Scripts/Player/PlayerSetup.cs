using Quantum;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [SerializeField]
    Transform camPosition;
    [SerializeField]
    EntityView entityView;
    [SerializeField]
    WeaponGunFire[] weapons;
    [SerializeField]
    InventoryGunFire inventory;
    [SerializeField]
    AnimationControl animationControl;
    [SerializeField]
    GameObject firstPersonVisual;
    [SerializeField]
    GameObject modelVisual;
    public void Init(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        var playerRef = f.Get<PlayerLink>(entityView.EntityRef).Player;
        var cameraPlayer = GameObject.Find("CameraPlayer");
        if (!game.PlayerIsLocal(playerRef))
        {
            int newLayer = UnityEngine.LayerMask.NameToLayer("Unseen");
            SetLayerRecursively(firstPersonVisual, newLayer);

        }
        else
        {
            int newLayer = UnityEngine.LayerMask.NameToLayer("Unseen");
            SetLayerRecursively(modelVisual, newLayer);
        }

        var playerLink = f.Get<PlayerLink>(entityView.EntityRef);

        inventory.OnInit();
        foreach (var weapon in weapons)
        {
            weapon.OnInit(game, entityView.EntityRef);
        }
        animationControl.OnInit(game);

        if (game.PlayerIsLocal(playerLink.Player))
        {
            cameraPlayer.transform.position = camPosition.position;
            cameraPlayer.transform.rotation = camPosition.rotation;
            cameraPlayer.transform.parent = camPosition;
        }

    }
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

}
