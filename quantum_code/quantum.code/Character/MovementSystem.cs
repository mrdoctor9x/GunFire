using Photon.Deterministic;

namespace Quantum.Character;

public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
{
    public struct Filter
    {
        public EntityRef              Entity;
        public CharacterController3D* CharacterController;
        public MovementInfo*          Info;
        public Transform3D*           Transform3D;
        public WeaponInfo*            weaponInfo;
        public HealthCompoment*       hpControl;
    }

    public override void Update(Frame f, ref Filter filter)
    {
        if(filter.hpControl->isDead)
            return;

        // gets the input for player 0
        Input input = default;
        if(f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
        {
            input = *f.GetPlayerInput(playerLink->Player);
        }

        filter.Info->running  = input.holdingButtonRun && CanRun(f, input, filter);
        filter.Info->aiming  = input.holdingButtonAim && CanAim(f, filter);
        filter.Info->IsGround = filter.CharacterController->Grounded;
        
        Jump(input, filter, f);

        Move(f, filter, input);

        Rotate(f, filter, input);

    }

    private void Jump(Input input, Filter filter, Frame f)
    {
        if (input.Jump.WasPressed)
        {
            filter.CharacterController->Jump(f);
        }
    }

    private void Move(Frame f, Filter filter, Input input)
    {
        if (filter.Info->running)
        {
            filter.CharacterController->MaxSpeed = filter.Info->speedRunning;
        }
        else
        {
            if (input.holdingButtonAim && CanAim(f, filter))
            {
                filter.CharacterController->MaxSpeed = filter.Info->speedAiming;
            }
            else
            {
                filter.CharacterController->MaxSpeed = filter.Info->speedWalking;
            }
        }
        filter.Info->directionMove           = input.Direction.XOY;
        var dirMove = filter.Transform3D->Forward * input.Direction.Y + filter.Transform3D->Right * input.Direction.X;
        filter.CharacterController->Move(f, filter.Entity, dirMove);
    }

    private void Rotate(Frame f, Filter filter, Input input)
    {
        filter.Info->rotationCharacter = input.AxisLook;
        FPQuaternion rotationYaw = FPQuaternion.Euler(0, input.AxisLook.X, 0);
        filter.Transform3D->Rotation *= rotationYaw;

        filter.Info->bodyRotation = filter.Info->LookRotaion(filter.Info->bodyRotation);
    }

    private bool CanRun(Frame f, Input input, Filter filter)
    {
        if (input.holdingButtonAim && CanAim(f, filter))
            return false;
        if (input.holdingButtonFire)
            return false;
        if (input.Direction.Y <= 0 || FPMath.Abs(FPMath.Abs(input.Direction.X) - 1) < FP._0_01)
            return false;
        if (filter.weaponInfo->isReloading)
            return false;
        return true;
    }
    private bool CanAim(Frame f, Filter filter)
    {
        if (filter.weaponInfo->isReloading)
            return false;
        return true;
    }
}