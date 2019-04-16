using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatToTarget : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 2;
    [SerializeField] private float _rotateSpeed = 5f;
    [SerializeField] private float _trackingCameraProximityCutoff;

    private Transform _defaultTransform;
    private Transform _moveTarget;
    private Transform _cameraTransform;
    private Vector3 _currentTarget;

    private void Awake()
    {
        _defaultTransform = new GameObject(this.gameObject.name + " Default Target").transform;
        _defaultTransform.position = this.transform.position;
        _defaultTransform.rotation = this.transform.rotation;
        SetTargetToDefault();

        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    void Move()
    {
        if (_moveTarget != _defaultTransform && Vector3.Distance(_moveTarget.position, _cameraTransform.position) > _trackingCameraProximityCutoff) _currentTarget = _moveTarget.position;

        this.transform.position = Vector3.Lerp(this.transform.position, _currentTarget, _moveSpeed * Time.deltaTime);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, _moveTarget.rotation, _rotateSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform target)
    {
        _moveTarget = target;
        _currentTarget = _moveTarget.position;
        Debug.Log("Set");
    }

    public void SetTargetToDefault()
    {
        _moveTarget = _defaultTransform;
        _currentTarget = _moveTarget.position;
        Debug.Log("Unset");
    }
}