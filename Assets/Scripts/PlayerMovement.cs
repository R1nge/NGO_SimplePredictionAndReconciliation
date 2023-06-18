using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float speed;
    private Vector3 _moveDirection;
    private NetworkVariable<Vector3> _lastServerPosition = new();

    private void Awake()
    {
        if(!IsOwner) return;
        _lastServerPosition.OnValueChanged += Validate;
    }

    private void Update()
    {
        if (!NetworkObject.IsOwner) return;
        GetInput();
        MoveLocally();
        MoveServerRpc(_moveDirection);
    }

    private void GetInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            _moveDirection = Vector3.left;
            SetLastServerPositionServerRpc(transform.position);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _moveDirection = Vector3.right;
            SetLastServerPositionServerRpc(transform.position);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            _moveDirection = Vector3.up;
            SetLastServerPositionServerRpc(transform.position);
        }
        else
        {
            _moveDirection = Vector3.zero;
            SetLastServerPositionServerRpc(transform.position);
        }
    }
    
    [ServerRpc]
    private void SetLastServerPositionServerRpc(Vector3 position)
    {
        _lastServerPosition.Value = position;
    }

    private void MoveLocally()
    {
        transform.position += _moveDirection * (speed * Time.deltaTime);
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 direction)
    {
        transform.position += direction * (speed * Time.deltaTime);
    }

    private void Validate(Vector3 previousPosition, Vector3 newPosition)
    {
        if ((newPosition - transform.position).sqrMagnitude >= 0.1f)
        {
            SetPositionServerRpc(newPosition);
        }
    }

    [ServerRpc]
    private void SetPositionServerRpc(Vector3 position)
    {
        transform.position = position;
        print("TELEPORTED");
    }
}