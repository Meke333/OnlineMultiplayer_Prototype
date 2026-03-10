using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAudio : NetworkBehaviour
{
    public static PlayerAudio Instance;

    [FormerlySerializedAs("_configuration")] [SerializeField] private AudioClipConfigurations _audioConfiguration;

    
    //private Camera _camera;
    
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if  (Instance != this &&  Instance != null)
            Destroy(this.gameObject);
        
        //_camera = Camera.main;
    }
    
    private void PlaySoundEffect(AudioClip clip, float volume = 1)
    {
        if (!clip)
            return;

        Debug.Log("Audio: " + clip.ToString());
        
        AudioSource.PlayClipAtPoint(clip, transform.position, volume);
        //AudioSource.PlayClipAtPoint(clip, _camera.transform.position);
    }

    [ClientRpc]
    public void PlaySoundEffectClientRpc(AudioType type, float volume = 1)
    {
        Debug.Log("PLAY THE AUDIO BITCH");
        PlaySoundEffect(_audioConfiguration.GetClip(type), volume);
    }
    
    
    [ClientRpc]
    public void PlayVoiceClipClientRpc(CharacterVoiceType type, PlayerNumberType playerNumberType)
    {
        Debug.Log("AUDIO CLIENT?: " + IsServer + IsClient + playerNumberType);
        
        CharacterVoiceClipConfiguration temp = playerNumberType switch
        {
            PlayerNumberType.PlayerOne => TransmitConfiguration.Instance.playerOneVoiceConfig,
            PlayerNumberType.PlayerTwo => TransmitConfiguration.Instance.playerTwoVoiceConfig, 
            _ => TransmitConfiguration.Instance.playerTwoVoiceConfig
        };

        Debug.Log("AUDIO 1; " + (temp));
        if (temp == null)
            return;
        
        Debug.Log("AUDIO 2");
        
        Debug.Log("PLAY THE VOICE BITCH");
        Debug.Log("AUDIO 3: " + (temp.GetAudioClip(type)).ToString());
        PlaySoundEffect(temp.GetAudioClip(type));
    }

    public void PlaySoundEffect(AudioType type, float volume = 1)
    {
        PlaySoundEffectClientRpc(type, volume);
    }

    public void PlayCountVoice(int count)
    {
        Debug.Log("Audio Count");

        if (count <= 0 || count > 9)
            return;
        
        AudioType result = count switch
        {
            1 => AudioType.Voice_One,
            2 => AudioType.Voice_Two,
            3 => AudioType.Voice_Three,
            4 => AudioType.Voice_Four,
            5 => AudioType.Voice_Five,
            6 => AudioType.Voice_Six,
            7 => AudioType.Voice_Seven,
            8 => AudioType.Voice_Eight,
            9 => AudioType.Voice_Nine,
        };
        
        PlaySoundEffect(result, 0.75f);
    }

    public void EvaluatePlayerState(PlayerStateType type, PlayerNumberType player)
    {
        Debug.Log("Audio State: " + IsServer + IsClient + "; " + player);
        
        switch (type)
        {
            case PlayerStateType.Attack:
                PlayVoiceClipClientRpc(CharacterVoiceType.Attack, player);
                break;
            case PlayerStateType.Dodge:
                PlaySoundEffectClientRpc(AudioType.Dodge);
                break;
            case PlayerStateType.Hurt:
                PlayVoiceClipClientRpc(CharacterVoiceType.Hurt, player);
                //PlaySoundEffectClientRpc(AudioType.Punch2);
                break;
            case PlayerStateType.Down:
                PlayVoiceClipClientRpc(CharacterVoiceType.KnockDown, player);
                break;
            case PlayerStateType.StepBack:
                break;
        }
    }

    public void GetUpSound(PlayerNumberType player)
    {
        PlaySoundEffectClientRpc(AudioType.Rumble);
        PlayVoiceClipClientRpc(CharacterVoiceType.GetUp, player);
        
    }
    
}