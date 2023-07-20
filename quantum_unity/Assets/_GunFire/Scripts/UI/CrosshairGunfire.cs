using InfimaGames;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairGunfire : QuantumCallbacks
{
    #region FIELDS SERIALIZED
    [Title(label: "References")]

    [Tooltip("Object to which all crosshair pieces are parented.")]
    [SerializeField, NotNull]
    private CanvasGroup crosshairCanvasGroup;

    [Tooltip("Little Dot!")]
    [SerializeField, NotNull]
    private CanvasGroup dotCanvasGroup;

    [Tooltip("This is the rect transform of the object that actually gets scaled to make the crosshair " +
             "look bigger.")]
    [SerializeField, NotNull]
    private RectTransform mainRectTransform;
    [SerializeField, NotNull]
    private LocalInput input;

    [Title(label: "Settings")]

    [Tooltip("Minimum and maximum scales for the crosshair.")]
    [SerializeField]
    private Vector2 minMaxScale = new Vector2(50.0f, 200.0f);

    [Tooltip("Default size of the crosshair. This is the size at which the crosshair stays when nothing is really " +
                 "happening.")]
    [SerializeField]
    private float defaultScale = 50.0f;

    [Title(label: "Interpolation")]

    [Tooltip("Interpolation speed of the crosshair' size.")]
    [SerializeField]
    private float interpolationSpeed = 7.0f;

    [Tooltip("Interpolation speed of the dot's visibility.")]
    [SerializeField]
    private float interpolationSpeedDot = 50.0f;

    [Tooltip("Delta size interpolation settings.")]
    [SerializeField]
    private SpringSettings interpolationSizeDelta = SpringSettings.Default();

    [Title(label: "Scale Additions")]

    [Tooltip("Value used to increase the crosshair' scale while moving.")]
    [SerializeField]
    private float movementScaleAddition = 25.0f;

    [Title(label: "Running")]

    [Tooltip("Determines the alpha value of the crosshair while the character is performing some action that disables it.")]
    [SerializeField]
    private float disabledVisibility = 0.6f;

    [Tooltip("Value used to increase the crosshair' scale while running.")]
    [SerializeField]
    private float runningScaleAddition = 15.0f;

    [Title(label: "Spread")]

    [Tooltip("Animation curve dictating how the crosshair scales as the character shoots more and more.")]
    [SerializeField]
    private AnimationCurve spreadIncrease;
    #endregion

    #region FIELDS

    /// <summary>
    /// Crosshair Local Scale.
    /// </summary>
    private float crosshairLocalScale;
    /// <summary>
    /// Crosshair Visibility.
    /// </summary>
    private float crosshairVisibility;
    /// <summary>
    /// Dot Visibility.
    /// </summary>
    private float dotVisibility;

    /// <summary>
    /// springCrosshairSizeDelta. Spring used to change the Crosshair's delta size.
    /// </summary>
    private Spring springCrosshairSizeDelta;


    #endregion

    void Awake()
    {

        //Initialize Spring.
        springCrosshairSizeDelta = new Spring();

        //Visibility.
        crosshairVisibility = 1.0f;
    }
    //private void Update()
    //{
    //    Tick();
    //}
    public override void OnUpdateView(QuantumGame game)
    {
        Tick(game);

    }
    void Tick(QuantumGame game)
    {
        var f = game.Frames.Predicted;
        //Get the shotsFired value from the character.
        int shotsFired = f.TryGet<WeaponInfo>(GameManager.LocalPlayerEntity, out var weaponInfo) ? weaponInfo.shotsFired : 0;
        //int shotsFired = 0;
        //Scale of the character's movement. We use this to scale the crosshair while moving around.
        float movementScale = input.GetInputMovement().sqrMagnitude * movementScaleAddition;
        //Size Target. This is the value that we're going to be interpolating to this frame.
        float sizeDeltaTarget = defaultScale + spreadIncrease.Evaluate(shotsFired);
        //Crosshair Scale Target.
        var crosshairLocalScaleTarget = 1.0f;
        //Crosshair Visibility.
        var crosshairVisibilityTarget = 1.0f;
        //Dot Scale Target.
        var dotVisibilityTarget = 1.0f;

        if (input.IsAiming())
        {
            crosshairLocalScaleTarget = dotVisibilityTarget = crosshairVisibilityTarget = 0.0f;
        }
        else
        {
            if (input.IsHolstered())
            {
                //Hide Crosshair.
                crosshairLocalScaleTarget = crosshairVisibilityTarget = 0.0f;
                //Show Dot.
                dotVisibilityTarget = 1.0f;
            }
            else
            {
                if (input.IsRunning())
                {
                    //sizeDeltaTarget += true ? default : default;
                    //This changes the crosshair's alpha visibility. We do this because it looks
                    //cool.
                    crosshairVisibilityTarget = disabledVisibility;
                    //Scale.
                    crosshairLocalScaleTarget = 1.0f;
                    //Update the size too!
                    sizeDeltaTarget += runningScaleAddition;
                }
                else
                {
                    sizeDeltaTarget += movementScale;
                    //Setup.
                    crosshairLocalScaleTarget = dotVisibilityTarget = 1.0f;
                }
            }
        }
        //Interpolate the dot visibility value to this frame's target.
        dotVisibility = Mathf.Lerp(dotVisibility, Mathf.Clamp01(dotVisibilityTarget), Time.deltaTime * interpolationSpeedDot);
        //Interpolate the crosshair local scale value to this frame's target.
        crosshairLocalScale = Mathf.Lerp(crosshairLocalScale, Mathf.Clamp01(crosshairLocalScaleTarget), Time.deltaTime * interpolationSpeed);
        //Interpolate the crosshair visibility value to this frame's target.
        crosshairVisibility = Mathf.Lerp(crosshairVisibility, Mathf.Clamp01(crosshairVisibilityTarget), Time.deltaTime * interpolationSpeed);

        //Clamp the crosshair size. Without this, it can get pretty crazy at times!
        sizeDeltaTarget = Mathf.Clamp(sizeDeltaTarget, minMaxScale.x, minMaxScale.y);

        //Update size delta end value.
        springCrosshairSizeDelta.UpdateEndValue(sizeDeltaTarget * Vector3.one);

        //Scale.
        mainRectTransform.sizeDelta = springCrosshairSizeDelta.Evaluate(interpolationSizeDelta);
        mainRectTransform.localScale = crosshairLocalScale * Vector3.one;

        //Alpha.
        crosshairCanvasGroup.alpha = crosshairVisibility;
        dotCanvasGroup.alpha = dotVisibility;
    }
}
