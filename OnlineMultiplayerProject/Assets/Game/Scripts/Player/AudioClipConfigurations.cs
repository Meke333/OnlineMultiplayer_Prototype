using UnityEngine;

[CreateAssetMenu(fileName = nameof(AudioClipConfigurations), menuName = "GameConfig/AudioConfig")]
public class AudioClipConfigurations : ScriptableObject
{
    public AudioClip Punch;
    
    public AudioClip Voice_GetSet;
    public AudioClip Voice_Fight;
    public AudioClip Voice_KnockDown;
    public AudioClip Voice_One;
    public AudioClip Voice_Two;
    public AudioClip Voice_Three;
    public AudioClip Voice_Four;
    public AudioClip Voice_Five;
    public AudioClip Voice_Six;
    public AudioClip Voice_Seven;
    public AudioClip Voice_Eight;
    public AudioClip Voice_Nine;
    public AudioClip Voice_Ten;

    public AudioClip GetClip(AudioType type)
    {
        return type switch
        {
            AudioType.Punch => Punch,
            AudioType.Voice_GetSet => Voice_GetSet,
            AudioType.Voice_Fight => Voice_Fight,
            AudioType.Voice_KnockDown => Voice_KnockDown,
            AudioType.Voice_One => Voice_One,
            AudioType.Voice_Two => Voice_Two,
            AudioType.Voice_Three => Voice_Three,
            AudioType.Voice_Four => Voice_Four,
            AudioType.Voice_Five => Voice_Five,
            AudioType.Voice_Six => Voice_Six,
            AudioType.Voice_Seven => Voice_Seven,
            AudioType.Voice_Eight => Voice_Eight,
            AudioType.Voice_Nine => Voice_Nine,
            AudioType.Voice_Ten => Voice_Ten,
            _ => null
        };
    }
    
}