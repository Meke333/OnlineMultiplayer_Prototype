using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class InteractionBetweenPlayersMediator : MonoBehaviour
{
    public static InteractionBetweenPlayersMediator Instance {get; private set;}
    
    public PlayerDamage playerOneDamage;
    public PlayerDamage playerTwoDamage;

    public PlayerStateMachine playerOneStateMachine;
    public PlayerStateMachine playerTwoStateMachine;

    public static int activePlayers = 0;

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

    public async void TransmitAttackInteraction(PlayerNumberType attacker, int direction, AttackType type, bool isDelayed = false)
    {
        PlayerStateMachine target = 
            attacker == PlayerNumberType.PlayerOne 
                ? playerTwoStateMachine 
                : playerOneStateMachine;

        if (isDelayed)
            await Task.Delay(300);

        target.TransmitEnemyAttack(type, direction);
        
    }
    
    public bool AttackInteraction(PlayerNumberType attacker, int direction, AttackType type)
    {
        PlayerDamage target = 
            attacker == PlayerNumberType.PlayerOne 
                ? playerTwoDamage 
                : playerOneDamage;
        
        bool result = target.DoesAttackLand(direction, type);
        
        if (result)
        {
            PlayerAudio.Instance.PlaySoundEffect(AudioType.Punch1);
            target.ProcessDamage(type, direction);
        }
        
        return result;
    }

    public void StepAwayInteraction(PlayerNumberType knockedDown)
    {
        PlayerStateMachine target = 
            knockedDown == PlayerNumberType.PlayerOne 
                ? playerTwoStateMachine 
                : playerOneStateMachine;
        
        target.GetStepAwayState();
        
        
        PlayerAudio.Instance.PlaySoundEffect(AudioType.Rumble);
        FightHandler.Instance?.Down();
        FightHandler.Instance?.Count();
    }

    public async void FightContinueInteraction()
    {

        float timer = 0.5f;
        while (timer > 0 && activePlayers >= 2)
        {
            playerOneStateMachine?.GetContinueState();
            playerTwoStateMachine?.GetContinueState();
            
            await Task.Yield();
            timer -= Time.deltaTime;
        }

        if (activePlayers < 2)
            return;
        
        playerOneStateMachine?.GetContinueState();
        playerTwoStateMachine?.GetContinueState();

        FightHandler.Instance?.ToggleWaitText(false);
        FightHandler.Instance?.StartFight();
    }

    public void InterruptFight()
    {
        playerOneStateMachine?.GetStepAwayState();
        playerTwoStateMachine?.GetStepAwayState();
    }

    public static void IncrementPlayerCount()
    {
        activePlayers++;

        if (activePlayers < 2)
            return;
        
        PlayerAudio.Instance.PlaySoundEffect(AudioType.Bell);
        Instance.FightContinueInteraction();
        
    }

    public static void DecrementPlayerCount()
    {
        activePlayers--;
        
        if (activePlayers >= 2)
            return;
        
        PlayerAudio.Instance.PlaySoundEffect(AudioType.TripleBell);
        Instance.InterruptFight();
        
        FightHandler.Instance?.ToggleWaitText(true);
        PostProcessingHandler.Instance?.ResetEffects();
    }

    public static void SetUpFight()
    {
        if (activePlayers < 1)
            return;
        
        FightHandler.Instance?.ToggleWaitText(false);
        FightHandler.Instance?.GetReady();
    }
}