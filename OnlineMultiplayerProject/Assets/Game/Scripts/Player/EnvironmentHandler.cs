using TriInspector;
using Unity.Netcode;
using UnityEngine;

public class EnvironmentHandler : NetworkBehaviour
{
    public EnvironmentHandler Instance { get; private set; }
    
    [SerializeField] private Transform environment;
    
    [SerializeField, ReadOnly] private int _playerOneDirection;
    [SerializeField, ReadOnly] private int _playerTwoDirection;
    
    [SerializeField, ReadOnly] private int _environmentTurnDirection;
    
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

    private void Update()
    {
        
    }
}