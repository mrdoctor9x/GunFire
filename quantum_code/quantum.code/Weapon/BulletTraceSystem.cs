using Photon.Deterministic;

namespace Quantum.Weapon;

public unsafe class BulletTraceSystem : SystemMainThreadFilter<BulletTraceSystem.Filter>
{
    public struct Filter
    {
        public EntityRef    Entity;
        public BulletTrace* trace;
        public Transform3D* transform3D;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        var hit = f.Physics3D.Raycast(filter.trace->startPos, filter.trace->DirFire, 50);
        var endPos = filter.trace->startPos + filter.trace->DirFire.Normalized;
        FPVector3 hitPos = hit.HasValue ? hit.Value.Point : endPos * 50;
        
        if (filter.trace->Time < 1)
        {
            filter.transform3D->Position =
                FPVector3.Lerp(filter.trace->startPos, hitPos, filter.trace->Time);
            filter.trace->Time += f.DeltaTime * 7 ;
        }

        if (filter.trace->Time >= 1)
            f.Destroy(filter.Entity);
    }
}