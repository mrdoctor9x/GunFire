namespace Quantum.Weapon;

public unsafe class BulletEffectSystem:SystemMainThreadFilter<BulletEffectSystem.Filter>
{
    public struct Filter
    {
        public EntityRef    Entity;
        public BulletEffet* Effect;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        filter.Effect->TimeDestroy -= f.DeltaTime;
        if (filter.Effect->TimeDestroy < 0)
        {
            f.Destroy(filter.Entity);
        }
    }
}