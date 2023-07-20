using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using InfimaGames.LowPolyShooterPack;
using System;
using GenifyStudio.Scripts.Manager;

public class AnimationControl : QuantumCallbacks
{
    [Tooltip("Determines how smooth the turning animation is.")]
    [SerializeField]
    private float dampTimeTurning = 0.4f;
    [SerializeField]
    private float dampTimeLocomotion = 0.15f;
    [Tooltip("How smoothly we play aiming transitions. Beware that this affects lots of things!")]
    [SerializeField]
    private float dampTimeAiming = 0.3f;
    [SerializeField]
    EntityView entityView;
    [SerializeField]
    Animator animator;
    [SerializeField]
    Transform body;

    [SerializeField]
    AudioSource runningAudio;

    private int layerOverlay;
    private int layerActions;
    private bool isRunningAudio;
    public void OnInit(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        layerOverlay = animator.GetLayerIndex("Layer Overlay");
        layerActions = animator.GetLayerIndex("Layer Actions");
        //f.Get<MovementInfo>(entityView.EntityRef).LookRotation = body.rotation;
        QuantumEvent.Subscribe<EventOnFire>(this, OnFire);
        QuantumEvent.Subscribe<EventOnReload>(this, OnReload);
    }

    private void OnReload(EventOnReload callback)
    {
        if (entityView.EntityRef == callback.entityRef)
        {
            string stateName = false ? "Reload Open" : (callback.hasAmmo ? "Reload" : "Reload Empty");
            //Play the animation state!
            animator.Play(stateName, layerActions, 0.0f);

            //Set Reloading Bool. This helps cycled reloads know when they need to stop cycling.
            animator.SetBool(AHashes.Reloading, true);
        }
    }

    private void OnFire(EventOnFire callback)
    {
        if (entityView.EntityRef == callback.entityRef)
        {
            const string stateName = "Fire";
            animator.CrossFade(stateName, 0.05f, layerOverlay, 0);
            //AudioManager.Instance.PlayOneHit(SoundFX.FireSilence);
        }

    }

    public override void OnUpdateView(QuantumGame game)
    {
        Look(game);
        UpdateAnimator(game);
        UpdateAudio(game);
    }
    private void UpdateAnimator(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        var axisMovement = f.Get<MovementInfo>(entityView.EntityRef).directionMove.ToUnityVector3();
        float movementValue = Mathf.Clamp01(Mathf.Abs(axisMovement.x) + Mathf.Abs(axisMovement.z));
        var axisLook = f.Get<MovementInfo>(entityView.EntityRef).rotationCharacter.ToUnityVector2();
        float leaningValue = Mathf.Clamp01(axisMovement.z);
        bool running = f.Get<MovementInfo>(entityView.EntityRef).running;
        bool aiming = f.Get<MovementInfo>(entityView.EntityRef).aiming;
        bool isGround = f.Get<MovementInfo>(entityView.EntityRef).IsGround;

        //Leaning. Affects how much the character should apply of the leaning additive animation.
        animator.SetFloat(AHashes.LeaningForward, leaningValue, 0.5f, Time.deltaTime);

        animator.SetFloat(AHashes.Movement, movementValue, dampTimeLocomotion, Time.deltaTime);
        //Horizontal Movement Float.
        animator.SetFloat(AHashes.Horizontal, axisMovement.x, dampTimeLocomotion, Time.deltaTime);
        //Vertical Movement Float.
        animator.SetFloat(AHashes.Vertical, axisMovement.z, dampTimeLocomotion, Time.deltaTime);
        //Update the aiming value, but use interpolation. This makes sure that things like firing can transition properly.
        animator.SetFloat(AHashes.AimingAlpha, Convert.ToSingle(aiming), dampTimeAiming, Time.deltaTime);
        //Turning Value. This determines how much of the turning animation to play based on our current look rotation.
        animator.SetFloat(AHashes.Turning, Mathf.Abs(axisLook.x), dampTimeTurning, Time.deltaTime);


        //Set the locomotion play rate. This basically stops movement from happening while in the air.
        const string playRateLocomotionBool = "Play Rate Locomotion";
        animator.SetFloat(playRateLocomotionBool, isGround ? 1.0f : 0.0f, 0.2f, Time.deltaTime);
        #region Movement Play Rates

        //Update Forward Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
        animator.SetFloat(AHashes.PlayRateLocomotionForward, 1, 0.2f, Time.deltaTime);
        //Update Sideways Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
        animator.SetFloat(AHashes.PlayRateLocomotionSideways, 1, 0.2f, Time.deltaTime);
        //Update Backwards Multiplier. This allows us to change the play rate of our animations based on our movement multipliers.
        animator.SetFloat(AHashes.PlayRateLocomotionBackwards, 1, 0.2f, Time.deltaTime);

        #endregion

        //Update Animator Aiming.
        animator.SetBool(AHashes.Aim, aiming);
        //Update Animator Running.
        animator.SetBool(AHashes.Running, running);
    }
    private void UpdateAudio(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        bool running = f.Get<MovementInfo>(entityView.EntityRef).running;
        bool isGround = f.Get<CharacterController3D>(entityView.EntityRef).Grounded;

        if (running && isGround && running != isRunningAudio)
        {
            if (!SettingConfig.Instance.IsMusicOn) return;
            runningAudio.Play();
            isRunningAudio = true;
        }
        else if (running && !isGround)
        {
            runningAudio.Stop();
            isRunningAudio = false;
        }
        else if(!running && running != isRunningAudio)
        {
            runningAudio.Stop();
            isRunningAudio = false;
        }
        //else
        //{
        //    isRunningAudio = running;
        //    AudioManager.Instance.StopSfxLoop();
        //}
    }
    private void Look(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        var rotation = body.localRotation;
        body.localRotation = f.Get<MovementInfo>(entityView.EntityRef).bodyRotation.ToUnityQuaternion();
    }
    private Quaternion Clamp(Quaternion rotation)
    {
        rotation.x /= rotation.w;
        rotation.y /= rotation.w;
        rotation.z /= rotation.w;
        rotation.w = 1.0f;

        //Pitch.
        float pitch = 2.0f * Mathf.Rad2Deg * Mathf.Atan(rotation.x);

        //Clamp.
        pitch = Mathf.Clamp(pitch, -89, 89);
        rotation.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * pitch);

        //Return.
        return rotation;
    }
}
