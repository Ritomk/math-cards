using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UniversalInputEvents", menuName = "Events/UniversalInputEvents")]
public class SoUniversalInputEvents : ScriptableObject
{
    public delegate void MouseMoveHandler();
    public event MouseMoveHandler OnMouseMove;

    public delegate void LookAroundHandler(Vector2 input);
    public event LookAroundHandler OnLookAround;

    public delegate void CardPickHandler(bool state);
    public event CardPickHandler OnPickCard;

    public delegate void CameraResetHandler(bool reset);
    public event CameraResetHandler OnCameraReset;

    public void RaiseMouseMove() => OnMouseMove?.Invoke();
    public void RaiseLookAround(Vector2 input) => OnLookAround?.Invoke(input);
    public void RaiseCardPick(bool state) => OnPickCard?.Invoke(state);
    public void RaiseCameraReset(bool reset) => OnCameraReset?.Invoke(reset);
}
