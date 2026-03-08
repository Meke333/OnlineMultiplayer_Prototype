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
    
    private void PlaySoundEffect(AudioClip clip)
    {
        if (!clip)
            return;
        
        AudioSource.PlayClipAtPoint(clip, transform.position);
        //AudioSource.PlayClipAtPoint(clip, _camera.transform.position);
    }

    [ClientRpc]
    public void PlaySoundEffectClientRpc(AudioType type)
    {
        Debug.Log("PLAY THE AUDIO BITCH");
        PlaySoundEffect(_audioConfiguration.GetClip(type));
    }
    
    [ClientRpc]
    public void PlaySoundEffectClientRpc(CharacterVoiceType type, PlayerNumberType playerNumberType)
    {
        CharacterVoiceClipConfiguration temp = playerNumberType switch
        {
            PlayerNumberType.PlayerOne => InteractionBetweenPlayersMediator.Instance.playerOneVoiceConfig,
            PlayerNumberType.PlayerTwo => InteractionBetweenPlayersMediator.Instance.playerTwoVoiceConfig, 
            _ => null
        };

        if (temp == null)
            return;
        
        Debug.Log("PLAY THE AUDIO BITCH");
        PlaySoundEffect(temp.GetAudioClip(type));
    }

    public void PlaySoundEffect(AudioType type)
    {
        Debug.Log("Audio 1");
        
        PlaySoundEffectClientRpc(type);
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
        
        PlaySoundEffect(result);
    }

    public void EvaluatePlayerState(PlayerStateType type, PlayerNumberType player)
    {
        Debug.Log("Audio State");

        
        switch (type)
        {
            case PlayerStateType.Attack:
                PlaySoundEffectClientRpc(CharacterVoiceType.Attack, player);
                break;
            case PlayerStateType.Dodge:
                PlaySoundEffectClientRpc(AudioType.Dodge);
                break;
            case PlayerStateType.Hurt:
                PlaySoundEffectClientRpc(CharacterVoiceType.Hurt, player);
                PlaySoundEffectClientRpc(AudioType.Punch1);
                break;
            case PlayerStateType.Down:
                PlaySoundEffectClientRpc(CharacterVoiceType.KnockDown, player);
                break;
            case PlayerStateType.StepBack:
                break;
        }
    }

    public void GetUpSound(PlayerNumberType player)
    {
        PlaySoundEffectClientRpc(AudioType.Rumble);
        PlaySoundEffectClientRpc(CharacterVoiceType.GetUp, player);
        
    }
    
}