using System.Threading.Tasks;
using UnityEngine;

public class FightHandler : MonoBehaviour
{
    public static FightHandler Instance;
    
    private bool continueCount;
    
    
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
        if (continueCount)
            return;
        continueCount = true;

        int count = 0;
        
        while (continueCount && count < 10)
        {
            //wait one second
            await Task.Delay(1000);
            
            count++;
            //Update Text here:
            FightUIHandler.Instance.SetCountText(count);
        }
        
        
        FightUIHandler.Instance.SetCountText(0);
        continueCount = false;
    }

    public void CancelCount()
    {
        continueCount = false;
    }

    public void ToggleWaitText(bool isOn)
    {
        FightUIHandler.Instance.ToggleWaitText(isOn);
    }
    
    public void GetReady()
    {
        FightUIHandler.Instance.SetPromptText(TextType.GetReady);
        ResetText();
    }

    public void StartFight()
    {
        FightUIHandler.Instance.SetPromptText(TextType.Fight);
        ResetText();
    }

    public void Down()
    {
        
        FightUIHandler.Instance.SetPromptText(TextType.Down);
        ResetText();
    }

    private async void ResetText()
    {
        await Task.Delay(1000);
        FightUIHandler.Instance.SetPromptText(TextType.Nothing);
    }
}