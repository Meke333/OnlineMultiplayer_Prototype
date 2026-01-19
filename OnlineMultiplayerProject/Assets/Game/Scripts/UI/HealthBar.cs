using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    public HealthBar Instance;
    [SerializeField] private Slider playerOneSlider;
    [SerializeField] private Slider playerTwoSlider;
    
    NetworkVariable<float> playerOneHealth = new NetworkVariable<float>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    NetworkVariable<float> playerTwoHealth = new NetworkVariable<float>(0,
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
    }

    public void ChangeHealth(PlayerNumberType playerNumber, float value)
    {
        NetworkVariable<float> temp = playerNumber switch
        {
            PlayerNumberType.PlayerOne => playerOneHealth,
            PlayerNumberType.PlayerTwo => playerTwoHealth,
        };
        
        temp.Value = value;
        
        //if value <= 0 then inform the statemachine of the corresponding player!!
        
        UpdateClientClientRpc();
    }

    [ClientRpc]
    private void UpdateClientClientRpc()
    {
        playerOneSlider.value = playerOneHealth.Value;
        playerTwoSlider.value = playerTwoHealth.Value;
    }
}
