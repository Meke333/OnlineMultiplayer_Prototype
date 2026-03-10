using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = nameof(CharacterVoiceClipConfiguration), menuName = "GameConfig/VoiceConfig")]
public class CharacterVoiceClipConfiguration : ScriptableObject
{
    public AudioClip[] Attack;
    public AudioClip[] Hurt;
    public AudioClip[] KnockDown;
    public AudioClip[] GetUp;
    
    public AudioClip GetAudioClip(CharacterVoiceType type)
    {
	    Debug.Log("Voice: " + type);
	    
	    AudioClip[] voiceClip = type switch
	    {
		    CharacterVoiceType.Attack => Attack,
		    CharacterVoiceType.Hurt => Hurt,
		    CharacterVoiceType.KnockDown => KnockDown,
		    CharacterVoiceType.GetUp => GetUp,
		    _ => null
	    };

	    Debug.Log("Voice: " + (voiceClip));
	    
	    if (voiceClip == null)
		    return null;
	    
	    if (voiceClip.Length == 0)
		    return null;
	    
	    return voiceClip[Random.Range(0, voiceClip.Length)];
	}
}