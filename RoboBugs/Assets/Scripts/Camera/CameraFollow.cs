using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;

/// <summary>
/// This is a Camera Follow Script Written by Patrick Emmons for the game "Pin Brawl" in 2024
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] [Range(0f, 1f)] private float damping = 1f;
    [SerializeField] [Range(0f, 1f)] private float zoomDamping = 1f;

    private Vector3 _movementVelocity = Vector3.zero;
    private float _scaleVelocity;
    private Camera _camera;

    private float _baseCameraZoom;

    public void UpdateDampening(float value)
    {
        damping = value;
    }
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _baseCameraZoom = _camera.orthographicSize;
    }

    //private Tuple<Vector3, float> GetTargetPosition()
    //{
    //    var targets = PointOfInterestManager.Instance.GetTargets();
    //    if (targets.Count <= 0) return Tuple.Create(transform.position, _baseCameraZoom);

    //    var summedPosition = targets.Aggregate(
    //        Vector3.zero,
    //        (current, position) => current + position
    //    );
    //    var averagePosition = summedPosition / targets.Count;

    //    var horizontalSpacing = targets.Max(it => it.x) - targets.Min(it => it.x);
    //    var zoom = Mathf.Max(horizontalSpacing / 2f, _baseCameraZoom);

    //    return Tuple.Create(averagePosition + offset, zoom);
    //}

    //private void FixedUpdate()
    //{
    //    var target = GetTargetPosition();
    //    UpdateCameraPosition(target.Item1, target.Item2);
    //}

    private void UpdateCameraPosition(Vector3 newTarget, float scale)
    {
        transform.position = Vector3.SmoothDamp(
            current: transform.position,
            target: newTarget.Copy(z: transform.position.z),
            currentVelocity: ref _movementVelocity,
            smoothTime: damping
        );

        _camera.orthographicSize = Mathf.SmoothDamp(
            current: _camera.orthographicSize,
            target: scale,
            currentVelocity: ref _scaleVelocity,
            smoothTime: zoomDamping
        );
    }
}