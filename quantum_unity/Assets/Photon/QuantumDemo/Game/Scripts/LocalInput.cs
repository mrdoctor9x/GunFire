using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInput : MonoBehaviour
{
    bool isJump;

    [Tooltip("If true, the running input has to be held to be active.")]
    [SerializeField]
    private bool holdToRun = true;
    [Tooltip("If true, the aiming input has to be held to be active.")]
    [SerializeField]
    private bool holdToAim = true;

    public static Transform fireAnchor;


    #region FIELDS
    private Vector2 axisMovement;
    private Vector2 axisLook;
    private bool holdingButtonRun;
    private bool holdingButtonAim;
    private bool holdingButtonFire;
    private bool aiming;
    private bool holstered;
    private bool running;
    private int shotsFired;
    private float lastShotTime;
    private bool cursorLocked;
    private bool reloading;
    #endregion

    #region Getter
    public Vector2 GetInputMovement() => axisMovement;
    public bool IsAiming() => aiming;
    public bool IsHolstered() => holstered;
    public bool IsRunning() => running;
    public int GetShotsFired() => shotsFired;
    #endregion

    private void Start()
    {
        cursorLocked = true;
        //Update cursor visibility.
        Cursor.visible = !cursorLocked;
        //Update cursor lock state.
        Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;

        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.P))
        //{

        //}
    }

    public void PollInput(CallbackPollInput callback)
    {
        Quantum.Input input = new Quantum.Input();

        // Note: Use GetButton not GetButtonDown/Up Quantum calculates up/down itself.
        input.Jump = isJump;
        input.reload = reloading;
        input.AxisLook = axisLook.ToFPVector2();
        input.Direction = axisMovement.ToFPVector2();
        input.holdingButtonRun = holdingButtonRun;
        input.holdingButtonAim = holdingButtonAim;
        input.holdingButtonFire = holdingButtonFire;

        if(fireAnchor != null)
        {
            input.StartFireAnchor = fireAnchor.position.ToFPVector3();
            input.DirFireAnchor = fireAnchor.forward.ToFPVector3();
        }


        running = holdingButtonRun && CanRun();
        aiming = holdingButtonAim && CanAim();
        isJump = false;
        reloading = false;

        callback.SetInput(input, DeterministicInputFlags.Repeatable);

    }
    #region ACTION CHECKS
    public void OnTryPlayReload(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
            return;

        //if (!CanPlayAnimationReload())
        //    return;

        switch (context)
        {
            case { phase: InputActionPhase.Performed }:
                reloading = true;
                break;
        }
    }
    public void OnLockCursor(InputAction.CallbackContext context)
    {
        //Switch.
        switch (context)
        {
            //Performed.
            case { phase: InputActionPhase.Performed }:
                cursorLocked = !cursorLocked;
                //Update cursor visibility.
                Cursor.visible = !cursorLocked;
                //Update cursor lock state.
                Cursor.lockState = cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
                break;
        }
    }
    public void OnTryJump(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
            return;
        //Block while the cursor is unlocked.
        //if (!cursorLocked)
        //    return;

        //Switch.
        switch (context.phase)
        {
            //Performed.
            case InputActionPhase.Performed:
                //Jump.
                isJump = true;
                //movementBehaviour.Jump();
                break;
        }
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
        {
            axisMovement = Vector2.zero;
            return;
        }

        //Read.
        axisMovement = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
            return;
        //Read.
        axisLook = context.ReadValue<Vector2>();
    }
    public void OnTryRun(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
            return;
        //Switch.
        switch (context.phase)
        {
            //Performed.
            case InputActionPhase.Performed:
                //Use this if we're using run toggle.
                if (!holdToRun)
                    holdingButtonRun = !holdingButtonRun;
                break;
            //Started.
            case InputActionPhase.Started:
                //Start.
                if (holdToRun)
                    holdingButtonRun = true;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Stop.
                if (holdToRun)
                    holdingButtonRun = false;
                break;
        }
    }
    public void OnTryAiming(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
            return;
        //Switch.
        switch (context.phase)
        {
            //Started.
            case InputActionPhase.Started:
                //Started.
                if (holdToAim)
                    holdingButtonAim = true;
                break;
            //Performed.
            case InputActionPhase.Performed:
                //Performed.
                if (!holdToAim)
                    holdingButtonAim = !holdingButtonAim;
                break;
            //Canceled.
            case InputActionPhase.Canceled:
                //Canceled.
                if (holdToAim)
                    holdingButtonAim = false;
                break;
        }
    }
    public void OnTryFire(InputAction.CallbackContext context)
    {
        if (!cursorLocked)
            return;
        //Switch.
        switch (context)
        {
            //Started.
            case { phase: InputActionPhase.Started }:
                //Hold.
                holdingButtonFire = true;

                //Restart the shots.
                shotsFired = 0;
                break;
            //Performed.
            //case { phase: InputActionPhase.Performed }:
            //    //Ignore if we're not allowed to actually fire.
            //    if (!CanPlayAnimationFire())
            //        break;

            //    //Check.
            //    if (HasAmmunition())
            //    {
            //        //Check.
            //        if (IsAutomatic())
            //        {
            //            //Reset fired shots, so recoil/spread does not just stay at max when we've run out
            //            //of ammo already!
            //            shotsFired = 0;

            //            //Break.
            //            break;
            //        }

            //        //Has fire rate passed.
            //        if (Time.time - lastShotTime > 60.0f / 610)
            //            Fire();
            //    }
            //    //Fire Empty.
            //    else
            //        FireEmpty();
            //    break;
            //Canceled.
            case { phase: InputActionPhase.Canceled }:
                //Stop Hold.
                holdingButtonFire = false;

                //Reset shotsFired.
                shotsFired = 0;
                break;
        }
    }
    #endregion
    private bool CanRun()
    {
        if (aiming)
            return false;
        if (holdingButtonFire)
            return false;
        if (axisMovement.y <= 0 || Math.Abs(Mathf.Abs(axisMovement.x) - 1) < 0.01f)
            return false;
        return true;
    }
    private bool CanAim()
    {

        return true;
    }
}
