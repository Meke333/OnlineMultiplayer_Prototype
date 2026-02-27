using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{
    public static PlayerAudio Instance;

    [SerializeField] private AudioClipConfigurations _configuration;

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
        PlaySoundEffect(_configuration.GetClip(type));
    }

    public void PlaySoundEffect(AudioType type)
    {
        Debug.Log("Audio 1");
        
        PlaySoundEffectClientRpc(type);
    }

    public void PlayCountVoice(int count)
    {
        Debug.Log("Audio Count");
        
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

    public void EvaluatePlayerState(PlayerStateType type)
    {
        Debug.Log("Audio State");

        
        switch (type)
        {
            case PlayerStateType.Dodge:
                PlaySoundEffectClientRpc(AudioType.Dodge);
                break;
            case PlayerStateType.Hurt:
                PlaySoundEffectClientRpc(AudioType.Punch1);
                break;
            case PlayerStateType.Down:
                break;
            case PlayerStateType.StepBack:
                break;
        }
    }
    
}