using System.CodeDom;
using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum.Weapon;

public unsafe class WeaponSystem: SystemMainThreadFilter<WeaponSystem.Filter>
{
    public struct Filter
    {
        public EntityRef        Entity;
        public WeaponInfo*      weaponInfo;
        public HealthCompoment* hpControl;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        if(filter.hpControl->isDead)
            return;
        Input input = default;
        if(f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
        {
            input = *f.GetPlayerInput(playerLink->Player);
        }

        TimeUpdate(f, filter);
        filter.weaponInfo->isAiming = input.holdingButtonAim;
        if (input.reload.WasPressed && !filter.weaponInfo->isReloading)
        {
            Reload(f,filter,input);
        }
        if (input.holdingButtonFire && filter.weaponInfo->Magazine > 0 && CanFire(f,filter))
        {
            Fire(f,filter, input);
        }
        else
        {
            filter.weaponInfo->shotsFired = 0;
        }
    }

    private void TimeUpdate(Frame f, Filter filter)
    {
        filter.weaponInfo->TimeFire   += f.DeltaTime;

        if (filter.weaponInfo->isReloading)
        {
            filter.weaponInfo->timeReload -= f.DeltaTime;
            if (filter.weaponInfo->timeReload < 0)
            {
                filter.weaponInfo->isReloading = false;
            }
        }
        
    }

    private void Reload(Frame f, Filter filter, Input input)
    {
        
        filter.weaponInfo->isReloading = true;
        filter.weaponInfo->timeReload  = filter.weaponInfo->Magazine > 0 ? 2 + FP._0_50 : 3 + FP._0_50;
        f.Events.OnReload(filter.Entity, filter.weaponInfo->Magazine > 0);
        filter.weaponInfo->Magazine = 30;
    }
    private unsafe void Fire(Frame f, Filter filter, Input input)
    {
        if (filter.weaponInfo->TimeFire > FP._0_10)
        {

            filter.weaponInfo->shotsFired++;
            filter.weaponInfo->TimeFire =  0;
            filter.weaponInfo->Magazine -= 1;
            var hit = f.Physics3D.Raycast(input.StartFireAnchor, input.DirFireAnchor, 50);
            
            var prototypeBullet = f.FindAsset<EntityPrototype>("Resources/DB/Prefab/BulletEffect/Bullet|EntityPrototype");
            var bulletEntity = f.Create(prototypeBullet);
            var bulletTrace = new BulletTrace()
            {
                Time = 0,
                startPos = input.StartFireAnchor,
                DirFire = input.DirFireAnchor
            };
            f.Add(bulletEntity, bulletTrace);
            f.Unsafe.GetPointer<Transform3D>(bulletEntity)->Position = input.StartFireAnchor;

            if (hit.HasValue)
            {
                if (f.Unsafe.TryGetPointer<HealthCompoment>(hit.Value.Entity, out var health))
                {
                    f.Events.OnDamage(hit.Value.Entity);
                    health->hp -= 10;
                }
                else
                {
                    var prototype = f.FindAsset<EntityPrototype>("Resources/DB/Prefab/BulletEffect/BulletEffect|EntityPrototype");
                    var effect = f.Create(prototype);
                    var bulletEffect = new BulletEffet()
                    {
                        TimeDestroy = FP._0_50,
                    };
                    f.Add(effect, bulletEffect);
            
                    if (f.Unsafe.TryGetPointer<Transform3D>(effect, out var transform))
                    {
                        transform->Position = hit.Value.Point;
                    }
                }
            
            
            }

            f.Events.OnFire(filter.Entity);
            
        }
    }
    private bool CanFire(Frame f, Filter filter)
    {
        if (filter.weaponInfo->isReloading)
            return false;
        return true; 
    }
}