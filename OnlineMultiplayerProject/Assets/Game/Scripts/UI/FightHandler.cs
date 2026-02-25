using System.Threading.Tasks;
using UnityEngine;

public class FightHandler : MonoBehaviour
{
    public static FightHandler Instance;
    
    private bool _continueCount;
    
    
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
    
    public async void Count()
    {
        if (_continueCount)
            return;
        _continueCount = true;

        int count = 0;
        
        while (_continueCount && count < 10)
        {
            //wait one second
            await Task.Delay(1000);
            
            count++;
            //Update Text here:
            FightUIHandler.Instance.SetCountText(count);
            PlayerAudio.Instance.PlayCountVoiceClientRpc(count);
        }
        
        
        FightUIHandler.Instance.SetCountText(0);
        _continueCount = false;
    }

    public void CancelCount()
    {
        _continueCount = false;
    }

    public void ToggleWaitText(bool isOn)
    {
        FightUIHandler.Instance.ToggleWaitText(isOn);
    }
    
    public void GetReady()
    {
        FightUIHandler.Instance.SetPromptText(TextType.GetReady);
        PlayerAudio.Instance.PlaySoundEffectClientRpc(AudioType.Voice_GetSet);
        ResetText();
    }

    public void StartFight()
    {
        FightUIHandler.Instance.SetPromptText(TextType.Fight);
        PlayerAudio.Instance.PlaySoundEffectClientRpc(AudioType.Voice_Fight);
        ResetText();
    }

    public void Down()
    {
        FightUIHandler.Instance.SetPromptText(TextType.Down);
        PlayerAudio.Instance.PlaySoundEffectClientRpc(AudioType.Voice_KnockDown);
        ResetText();
    }

    private async void ResetText()
    {
        await Task.Delay(1000);
        FightUIHandler.Instance.SetPromptText(TextType.Nothing);
    }
}