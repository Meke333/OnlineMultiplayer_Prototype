using System.Threading.Tasks;
using UnityEngine;

public sealed class ClientSingleton : MonoBehaviour
{
    public static ClientSingleton Instance { get; set; }
    
    public ClientGameManager GameManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async Task<bool> CreateClient()
    {
        GameManager = new ClientGameManager();
        return await GameManager.InitAsync();
    }

    private void OnDestroy()
    {
        GameManager?.Dispose();
    }
}