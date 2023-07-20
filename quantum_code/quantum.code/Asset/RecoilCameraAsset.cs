using System;
using Photon.Deterministic;
using Quantum.Inspector;

namespace Quantum;

partial class RecoilCameraAsset
{
    public FP                    locationMultiplier;
    public SpringSettingsQuantum locationSpring;
    public FPAnimationCurve[]    locationCurves;
    public FP                    rotationMultiplier;
    public SpringSettingsQuantum rotationSpring;
    public FPAnimationCurve[] rocationCurves;
}
[Serializable]
 public struct SpringSettingsQuantum
 {

     [Tooltip("Determines how springy the spring is, the lower this value, the more bounce you will see.")]
     [Range(0, 100)]
     public FP damping;

     [Tooltip("Determines how stiff the interpolation looks. The lower the value, the stiffer it becomes.")]
     [Range(0, 200)]
     public FP stiffness;

     [Tooltip("Determines how heavy the interpolation looks.")]
     [Range(0, 100)]
     public FP mass;

     [Tooltip("Determines the speed of the interpolation. The higher the value, the faster the speed.")]
     [Range(1, 10)]
     public FP speed;

     public static SpringSettingsQuantum Default()
     {
         //Return.
         return new SpringSettingsQuantum()
         {
             damping   = 15,
             mass      = 1,
             stiffness = 150,
             speed     = 1
         };
     }
 }