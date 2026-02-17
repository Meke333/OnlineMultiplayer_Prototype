using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : NetworkBehaviour
{
    public const float MaxHealth = 20f;
    public static HealthBar Instance;
    [SerializeField] private Slider playerOneSlider;
    [SerializeField] private Slider playerTwoSlider;
    [SerializeField] private TMPro.TMP_Text playerOneScoreText;
    [SerializeField] private TMPro.TMP_Text playerTwoScoreText;
    [SerializeField] private TMPro.TMP_Text playerOneNameText;
    [SerializeField] private TMPro.TMP_Text playerTwoNameText;
    [SerializeField] private Image playerOneImage;
    [SerializeField] private Image playerTwoImage;
    
    [SerializeField] private TMPro.TMP_Text joinCode;

    [SerializeField] private PlayerInformationConfiguration _configuration;
    
    NetworkVariable<float> playerOneHealth = new NetworkVariable<float>(MaxHealth, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);

    NetworkVariable<float> playerTwoHealth = new NetworkVariable<float>(MaxHealth,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
    
    NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private int _playerOneImageIndex = -1;
    private int _playerTwoImageIndex = -1;
    private FixedString32Bytes _playerOneName;
    private FixedString32Bytes _playerTwoName;
    
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
        playerOneScore.OnValueChanged += UpdateScore;
        playerTwoScore.OnValueChanged += UpdateScore;
        
        joinCode.text = HostSingleton.Instance.GameManager.JoinCode;

    }

    private void LateUpdate()
    {
        //joinCode.text = HostSingleton.Instance.GameManager.JoinCode;
    }


    public override void OnNetworkDespawn()
    {
        playerOneHealth.OnValueChanged -= UpdateHealthBar;
        playerTwoHealth.OnValueChanged -= UpdateHealthBar;
        playerOneScore.OnValueChanged -= UpdateScore;
        playerTwoScore.OnValueChanged -= UpdateScore;
        
    }

    private void UpdateHealthBar(float previousValue, float newValue)
    {
        playerOneSlider.value = playerOneHealth.Value;
        playerTwoSlider.value = playerTwoHealth.Value;
    }

    private void UpdateScore(int previousValue, int newValue)
    {
        Debug.Log("KD: UpdateScore");
        
        playerOneScoreText.text = playerOneScore.Value.ToString();
        playerTwoScoreText.text = playerTwoScore.Value.ToString();
    }

    public void IncreaseScore(PlayerNumberType type, int score)
    {
        NetworkVariable<int> target = type switch
        {
            PlayerNumberType.PlayerOne => playerOneScore,
            PlayerNumberType.PlayerTwo => playerTwoScore,
        };
        
        target.Value += score;
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
        ChangeHealth(playerNumber, value, reduce);
    }

    public void ChangeHealth(PlayerNumberType playerNumber, float value, bool reduce = true)
    {
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
        /*if (!IsServer)
            return;

        if (temp.Value <= 0)
        {
            //stateMachine.GetKnockedDown();
        }
        */
    }

    public bool CheckKnockdownStatus(PlayerNumberType playerNumber)
    {
        NetworkVariable<float> temp = playerNumber switch
        {
            PlayerNumberType.PlayerOne => playerOneHealth,
            PlayerNumberType.PlayerTwo => playerTwoHealth,
        };

        return temp.Value <= 0;
    }

    //May change it out later.....
    //Call by Spawn
    [Rpc(SendTo.Everyone)]
    public void ChangePlayerImageRpc(PlayerNumberType playerNumber, int index)
    {
        if (index >= _configuration.PlayerSprite.Length)
        {
            Debug.LogError("ChangePlayerImageRpc: index out of range");
            return;
        }

        Image target = null;
        
        switch (playerNumber)
        {
            case PlayerNumberType.PlayerOne:
                target = playerOneImage;
                _playerOneImageIndex = index;
                break;
            case PlayerNumberType.PlayerTwo:
            default:
                target = playerTwoImage;
                _playerTwoImageIndex = index;
                break;
        }
        
        if (!target)
            return;
        
        target.sprite = index < 0 ? null : _configuration.PlayerSprite[index];
    }
    
    [Rpc(SendTo.Everyone)]
    public void ChangePlayerNameRpc(PlayerNumberType playerNumber, FixedString32Bytes playerName)
    {
        TMPro.TMP_Text targetText;
        

        switch (playerNumber)
        {
            case PlayerNumberType.PlayerOne:
                targetText = playerOneNameText;
                _playerOneName = playerName;
                break;
            case PlayerNumberType.PlayerTwo:
            default:
                targetText = playerTwoNameText;
                _playerTwoName = playerName;
                break;
        }   

        targetText.text = playerName.Value;
    }

    //IM ALREADY CHECKING FOR OWNERSHIP WHY DO YOU NOT WORK???
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void RetrieveInformationServerRpc()
    {
        Debug.Log("INIT: " + _playerOneImageIndex + "; " 
                  + _playerTwoImageIndex + "; " 
                  + _playerOneName.Value + "; " 
                  + _playerTwoName.Value);
        if (!IsOwner)
            return;
        
        SendInformation(
            _playerOneImageIndex, 
            _playerTwoImageIndex,  
            _playerOneName.Value, 
            _playerTwoName.Value);;
    }
    
    private void SendInformation(int index1, int index2, FixedString32Bytes name1, FixedString32Bytes name2)
    {
        ChangePlayerImageRpc(PlayerNumberType.PlayerOne, index1);
        ChangePlayerImageRpc(PlayerNumberType.PlayerTwo, index2);
        ChangePlayerNameRpc(PlayerNumberType.PlayerOne, name1);
        ChangePlayerNameRpc(PlayerNumberType.PlayerTwo, name2);
    }
    
    
}