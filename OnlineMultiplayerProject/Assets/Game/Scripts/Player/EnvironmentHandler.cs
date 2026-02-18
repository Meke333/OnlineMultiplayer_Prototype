using System;
using TriInspector;
using Unity.Netcode;
using UnityEngine;

public class EnvironmentHandler : NetworkBehaviour
{
    public static EnvironmentHandler Instance { get; private set; }
    
    [SerializeField] private Transform environment;
    [SerializeField] private float anglePerSeconds = 20;
    
    [SerializeField, ReadOnly] private int _playerOneDirection;
    [SerializeField, ReadOnly] private int _playerTwoDirection;
    [SerializeField, ReadOnly] private PlayerStateType _playerOneState;
    [SerializeField, ReadOnly] private PlayerStateType _playerTwoState;
    
    [SerializeField, ReadOnly] private float _environmentTurnDirection;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (Instance && Instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;
        
        _environmentTurnDirection = CheckRotation() * Time.fixedDeltaTime * anglePerSeconds;

        if (Math.Abs(_environmentTurnDirection) < 0.001f)
            return;
        
        RotateEnvironmentClientRpc(_environmentTurnDirection);
    }

    [ClientRpc]
    private void RotateEnvironmentClientRpc(float angle)
    {
        environment.Rotate(Vector3.up, angle);
    }

    [ClientRpc]
    private void ResetEnvironmentTransformClientRpc()
    {
        environment.rotation = Quaternion.identity;
    }

    private int CheckRotation()
    {
        
        int direction = 0;

        bool isPlayerOneMoving = _playerOneState == PlayerStateType.Dodge 
                                 && Mathf.Abs(_playerOneDirection) > 0.1f;
        bool isPlayerTwoMoving = _playerTwoState == PlayerStateType.Dodge 
                                 &&  Mathf.Abs(_playerTwoDirection) > 0.1f;

        if (!isPlayerOneMoving && !isPlayerTwoMoving)
            return direction;
        
        if (isPlayerOneMoving && isPlayerTwoMoving)
        {
            //Debug.Log("Env: BOTH ARE MOVING");
            
            if (_playerOneDirection == _playerTwoDirection)
            {
                direction = _playerOneDirection;
            }
            else //_playerOneDirection != playerTwoDirection
            {
                direction = 0;
            }
        }
        else //isPlayerOneMoving != isPlayerTwoMoving
        {
            //Debug.Log("Env: ONE IS MOVING");
            
            if (isPlayerOneMoving)
            {
                direction = _playerOneDirection;
            }
            else //isPlayerTwoMoving
            {
                direction = _playerTwoDirection * -1;
            }
        }

        return direction;
    }

    public void SetState(PlayerNumberType number, PlayerStateType state)
    {
        switch (number)
        {
            case PlayerNumberType.PlayerOne:
                _playerOneState = state;
                break;
            case PlayerNumberType.PlayerTwo:
                _playerTwoState = state;
                break;
        }
    }

    public void SetDirection(PlayerNumberType number, int direction)
    {
        switch (number)
        {
            case PlayerNumberType.PlayerOne:
                _playerOneDirection = direction;
                break;
            case PlayerNumberType.PlayerTwo:
                _playerTwoDirection = direction;
                break;
        }
    }

    public void ResetEnvironmentTransform()
    {
        ResetEnvironmentTransformClientRpc();
    }
}