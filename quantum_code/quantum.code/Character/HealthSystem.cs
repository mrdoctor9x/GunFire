namespace Quantum.Character;

public unsafe class HealthSystem:SystemMainThreadFilter<HealthSystem.Filter>
{
    public struct Filter
    {
        public EntityRef        Entity;
        public HealthCompoment* hpControl;
        public Transform3D*     transform3D;
        public WeaponInfo*      weaponInfo;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        if (filter.hpControl->hp < 0 && !filter.hpControl->isDead)
            filter.hpControl->isDead = true;

        if (filter.hpControl->isDead)
        {
            filter.hpControl->timeRespawn += f.DeltaTime;
            if (filter.hpControl->timeRespawn > 3)
            {
                filter.hpControl->hp          = 100;
                filter.hpControl->isDead      = false;
                filter.hpControl->timeRespawn = 0;
                filter.weaponInfo->Magazine   = 30;
                Respawn(f, filter);
            }
        }

        if (filter.transform3D->Position.Y < -10)
        {
            Respawn(f, filter);
        }
        
    }

    private void Respawn(Frame f, Filter filter)
    {
        var rd = f.RNG->Next(0, 7);
        var spawnPoints = f.Unsafe.GetComponentBlockIterator<SpawnPoint>();
        foreach (var point in spawnPoints)
        {
            var pos = f.Unsafe.GetPointer<Transform3D>(point.Entity)->Position;
            if (point.Component->index == rd)
            {
                filter.transform3D->Position = pos;
            }
                
        }
    }
}