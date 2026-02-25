using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public static PlayerAudio Instance;

    [SerializeField] private AudioClipConfigurations _configuration;

    private Camera _camera;
    
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if  (Instance != this &&  Instance != null)
            Destroy(this.gameObject);
        
        _camera = Camera.main;
    }
    
    private void PlaySoundEffect(AudioClip clip)
    {
        if (!clip)
            return;
        
        AudioSource.PlayClipAtPoint(clip, _camera.transform.position);
    }

    [ClientRpc]
    public void PlaySoundEffectClientRpc(AudioType type)
    {
        PlaySoundEffect(_configuration.GetClip(type));
    }

    [ClientRpc]
    public void PlayCountVoiceClientRpc(int count)
    {
        AudioClip result = count switch
        {
            1 => _configuration.GetClip(AudioType.Voice_One),
            2 => _configuration.GetClip(AudioType.Voice_Two),
            3 => _configuration.GetClip(AudioType.Voice_Three),
            4 => _configuration.GetClip(AudioType.Voice_Four),
            5 => _configuration.GetClip(AudioType.Voice_Five),
            6 => _configuration.GetClip(AudioType.Voice_Six),
            7 => _configuration.GetClip(AudioType.Voice_Seven),
            8 => _configuration.GetClip(AudioType.Voice_Eight),
            9 => _configuration.GetClip(AudioType.Voice_Nine),
            _ => null
        };
        
        PlaySoundEffect(result);
    }
    
    
}