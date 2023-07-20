using System;
using Photon.Deterministic;

namespace Quantum;

public partial struct SpringQuantum
{
    public void UpdateEndValue(FPVector3 value) => UpdateEndValue(value, currentVelocity);
    public void UpdateEndValue(FPVector3 value, FPVector3 velocity)
    {
        end             = value;
        currentVelocity = velocity;
    }

    public FPVector3 Evaluate(Frame f)
    {
        FP deltaTime = f.DeltaTime * springSetting.speed;
        
        FP c = springSetting.damping;
        FP m = springSetting.mass;
        FP k = springSetting.stiffness;
        
        FPVector3 x = currentValue;
        FPVector3 v = currentVelocity;
        FPVector3 a = currentAcceleration;
        
        FP _stepSize = deltaTime > FP._0_03 ? FP._0_03 : deltaTime - FP._0_01 / 10;
        FP steps = FPMath.Ceiling(deltaTime / _stepSize);
        for (var i = 0; i < steps; i++)
        {
            FP dt = FPMath.Abs(i - (steps - 1)) < FP._0_01 ? deltaTime - i * _stepSize : _stepSize;

            x += v * dt + a * (dt * dt * FP._0_50);
            var _a = (-k * (x - (end + FPVector3.Zero)) + -c * v) / m;
            v += (a + _a) * (dt * FP._0_50);
            a =  _a;
        }
        currentValue        = x;
        currentVelocity     = v;
        currentAcceleration = a;
        return currentValue;
    }
    public FPVector3 Evaluate(Frame f ,SpringSettingsQuantum newSettings)
    {
        //Adjust Settings.
        Adjust(newSettings);

        //Evaluate.
        return Evaluate(f);
    }

    private void Adjust(SpringSettingsQuantum newSettings)
    {
        springSetting.damping   = newSettings.damping;
        springSetting.mass      = newSettings.mass;
        springSetting.speed     = newSettings.speed;
        springSetting.stiffness = newSettings.stiffness;
    }
}