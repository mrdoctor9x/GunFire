using System;
using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum.Weapon;

public unsafe class MotionSystem:SystemMainThreadFilter<MotionSystem.Filter>
{
    public struct Filter
    {
        public EntityRef   Entity;
        public WeaponInfo* weaponInfo;
    }


    public override void Update(Frame f, ref Filter filter)
    {
        filter.weaponInfo->finalLocation   = default;
        filter.weaponInfo->finaEulerAngles = default;
        RecoilMotion(f, filter);

        var recoilCurves = f.FindAsset<RecoilCameraAsset>("Resources/DB/Asset/Recoil_Camera_Asset");
        filter.weaponInfo->finalLocation   += filter.weaponInfo->recoilSpringLocation.Evaluate(f, recoilCurves.locationSpring);
        filter.weaponInfo->finaEulerAngles += filter.weaponInfo->recoilSpringRotation.Evaluate(f, recoilCurves.rotationSpring);
    }

    public void RecoilMotion(Frame f, Filter filter)
    {
        int shotsFired = filter.weaponInfo->shotsFired;
        FP recoilDataMultiplier = 1;
        FPVector3 recoilLocation = default;
        FPVector3 recoilRotation = default;
        var recoilCurves = f.FindAsset<RecoilCameraAsset>("Resources/DB/Asset/Recoil_Camera_Asset");
        if (filter.weaponInfo->isAiming)
        {
            //recoilDataMultiplier = 1;
        }

        if (recoilCurves != null)
        {
            if (recoilCurves.locationCurves.Length == 3)
            {
                
            }

            if (recoilCurves.rocationCurves.Length == 3)
            {
                recoilRotation.X = recoilCurves.rocationCurves[0].Evaluate(shotsFired);
                recoilRotation.Y = recoilCurves.rocationCurves[1].Evaluate(shotsFired);
                recoilRotation.Z = recoilCurves.rocationCurves[2].Evaluate(shotsFired);
            }
            
            recoilLocation *= recoilCurves.locationMultiplier * recoilDataMultiplier;
            //Add Multipliers.
            recoilRotation *= recoilCurves.rotationMultiplier * recoilDataMultiplier;
        }
        filter.weaponInfo->recoilSpringLocation.UpdateEndValue(recoilLocation);
        filter.weaponInfo->recoilSpringRotation.UpdateEndValue(recoilRotation);
    }
}
