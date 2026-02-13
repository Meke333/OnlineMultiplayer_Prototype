using UnityEngine;

public class InitialTransformInformation : MonoBehaviour
{
    public static InitialTransformInformation Instance;
    
    [field: SerializeField] public Transform PlayerOneInformation { get; private set; }
    [field: SerializeField] public Transform PlayerTwoInformation { get; private set; }

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
}