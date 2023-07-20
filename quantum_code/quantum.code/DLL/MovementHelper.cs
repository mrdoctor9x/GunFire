using Photon.Deterministic;

namespace Quantum
{
    public partial struct MovementInfo
    {
        public FPQuaternion LookRotaion(FPQuaternion rotation)
        {
            FPQuaternion rotationPitch = FPQuaternion.Euler(-rotationCharacter.Y, 0, 0);
            rotation *= rotationPitch;
            rotation =  Clamp(rotation);
            return rotation;
        }
        private FPQuaternion Clamp(FPQuaternion rotation)
        {
            rotation.X /= rotation.W;
            rotation.Y /= rotation.W;
            rotation.Z /= rotation.W;
            rotation.W =  1;

            //Pitch.
            FP pitch = 2 * FP.Rad2Deg * FPMath.Atan(rotation.X);

            //Clamp.
            pitch      = FPMath.Clamp(pitch, -89, 89);
            rotation.X = FPMath.Tan(FP._0_50 * FP.Deg2Rad * pitch);

            //Return.
            return rotation;
        }
    }
}