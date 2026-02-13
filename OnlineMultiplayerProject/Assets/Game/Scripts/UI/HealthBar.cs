using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    public static HealthBar Instance;
    [SerializeField] private Slider playerOneSlider;
    [SerializeField] private Slider playerTwoSlider;
    
    NetworkVariable<float> playerOneHealth = new NetworkVariable<float>(20, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    NetworkVariable<float> playerTwoHealth = new NetworkVariable<float>(20,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (!Instance && Instance != this)
            Destroy(this.gameObject);
        
        
        SetUpRpc();

        playerOneHealth.OnValueChanged += UpdateHealthBar;
        playerTwoHealth.OnValueChanged += UpdateHealthBar;
    }

    
    public override void OnNetworkDespawn()
    {
        playerOneHealth.OnValueChanged += UpdateHealthBar;
        playerTwoHealth.OnValueChanged += UpdateHealthBar;
    }

    private void UpdateHealthBar(float previousValue, float newValue)
    {
        Debug.Log("ZZZ: 7; " + playerOneHealth.Value + "; " + playerTwoHealth.Value);
        Debug.Log("ZZZ: 8; " + playerOneSlider.value + "; " + playerTwoSlider.value);
        Debug.Log("ZZZ: 9; " + playerOneSlider.maxValue + "; " + playerTwoSlider.maxValue);
        playerOneSlider.value = playerOneHealth.Value;
        playerTwoSlider.value = playerTwoHealth.Value;
    }
    
    
    [Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Everyone)]
    private void SetUpRpc()
    {
        playerOneSlider.maxValue = playerOneHealth.Value;
        playerTwoSlider.maxValue = playerTwoHealth.Value;
        
        playerOneSlider.value = playerOneHealth.Value;
        playerTwoSlider.value = playerTwoHealth.Value;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void ChangeHealthRpc(PlayerNumberType playerNumber, float value, bool reduce = true)
    {
        Debug.Log("ZZZ: 6");
        
        NetworkVariable<float> temp = playerNumber switch
        {
            PlayerNumberType.PlayerOne => playerOneHealth,
            PlayerNumberType.PlayerTwo => playerTwoHealth,
        };
        
        if (reduce)
            temp.Value -= value;
        else
            temp.Value = value;
        
        //if value <= 0 then inform the statemachine of the corresponding player!!
        
        //UpdateClientsRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateClientsRpc()
    {
        Debug.Log("ZZZ: 7; " + playerOneHealth.Value + "; " + playerTwoHealth.Value);
        
        Debug.Log("ZZZ: 8; " + playerOneSlider.value + "; " + playerTwoSlider.value);
        Debug.Log("ZZZ: 9; " + playerOneSlider.maxValue + "; " + playerTwoSlider.maxValue);
        playerOneSlider.value = playerOneHealth.Value;
        playerTwoSlider.value = playerTwoHealth.Value;
    }
}
