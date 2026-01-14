using TMPro;
using UnityEngine;

public sealed class UIHandler : MonoBehaviour
{
    public static UIHandler Singleton { set; get; }

    /*
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    */

    public void UpdateUI(int variable)
    {
        TMP_Text outtext = GameObject.Find("NetworkUI/Counter").GetComponent<TMP_Text>();
        if (outtext != null)
            outtext.text = "Axes: " + variable;
    }
}
