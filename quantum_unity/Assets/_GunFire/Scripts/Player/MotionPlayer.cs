using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionPlayer : QuantumCallbacks
{
    [SerializeField]
    private EntityView entityView;
    [SerializeField]
    private Transform recoilBody;
    public override void OnUpdateView(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        recoilBody.localEulerAngles = f.Get<WeaponInfo>(entityView.EntityRef).finaEulerAngles.ToUnityVector3();
    }
}
