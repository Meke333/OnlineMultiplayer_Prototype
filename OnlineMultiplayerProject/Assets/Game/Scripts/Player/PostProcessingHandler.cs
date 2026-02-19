using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingHandler : MonoBehaviour
{
    public static PostProcessingHandler Instance;
    
    [SerializeField] private Volume volume;

    [SerializeField] private float _vignetteMaxValue = 0.35f;
    [SerializeField] private float _motionBlurMaxValue = 0.255f;
    [SerializeField] private float _chromaticAberrationMaxValue = 1;
    [SerializeField] private float _screenSpaceLensFlareMaxValue = 50;
    
    private Vignette _vignette;
    private MotionBlur _motionBlur;
    private ChromaticAberration _chromaticAberration;
    private ScreenSpaceLensFlare _screenSpaceLensFlare;

    private CancellationTokenSource _damageCancelToken;
    private CancellationTokenSource _resetToken = new CancellationTokenSource();
    
    private bool _isDowned;
    private bool _isDodging;
    
    private PlayerStateType _playerStateType;
    
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if  (Instance != null && Instance != this)
            Destroy(this.gameObject);
        
        volume.profile.TryGet(out _vignette);
        volume.profile.TryGet(out _motionBlur);
        volume.profile.TryGet(out _chromaticAberration);
        volume.profile.TryGet(out _screenSpaceLensFlare);
    }
    
    public void OnDestroy()
    {
        ResetEffects();
    }
    
    private void SetOwnVignetteIntensity(float percentage)
    {
        _vignette.intensity.value = _vignetteMaxValue * percentage;
    }

    private void SetOwnMotionBlurIntensity(float percentage)
    {
        _motionBlur.intensity.value = _motionBlurMaxValue * Mathf.Clamp(percentage, 0, 1);
    }

    private void SetOwnChromaticAberrationIntensity(float percentage)
    {
        _chromaticAberration.intensity.value = _chromaticAberrationMaxValue * Mathf.Clamp(percentage, 0, 1);
    }

    private void SetOwnScreenSpaceLensFlareIntensityClientRpc(float percentage)
    {
        _screenSpaceLensFlare.intensity.value = _screenSpaceLensFlareMaxValue * Mathf.Clamp(percentage, 0, 1);
    }

    [ClientRpc]
    public void EvaluatePlayerStateClientRpc(PlayerStateType type)
    {
        if (_playerStateType == type)
            return;
        
        DodgingEffect(type == PlayerStateType.Dodge);
        
        switch (type)
        {
            case PlayerStateType.Idle:
                if (_playerStateType == PlayerStateType.Down)
                    DownedEffect(false);
                break;
            case PlayerStateType.Hurt:
                DamageEffect();
                break;
            case PlayerStateType.Down:
                DownedEffect(true);
                break;
        }
        
        _playerStateType = type;
    }
    
    public void DamageEffect()
    {
        _damageCancelToken?.Cancel();
        _damageCancelToken = new CancellationTokenSource();
        
        PerformDamageEffect(_damageCancelToken.Token);
    }
    
    private async void PerformDamageEffect(CancellationToken token)
    {
        //Vignette + Chromatic Aberration

        float vignetteValue = 1;
        float aberrationValue = 1;
        
        bool whileLoopCheck = true;
        
        SetOwnVignetteIntensity(1);
        SetOwnChromaticAberrationIntensity(1);
        
        while (whileLoopCheck)
        {
            SetOwnVignetteIntensity(vignetteValue);
            SetOwnChromaticAberrationIntensity(aberrationValue);

            vignetteValue -= 0.5f * Time.deltaTime;
            aberrationValue -= Time.deltaTime;
            
            await Task.Yield();

            whileLoopCheck = _resetToken.Token.IsCancellationRequested
                             || token.IsCancellationRequested 
                             || vignetteValue < 1
                             || aberrationValue < 1;
        }

        
        if (_resetToken.IsCancellationRequested)
        {
            SetOwnVignetteIntensity(0);
            SetOwnChromaticAberrationIntensity(0);;
            return;
        }
        
        if (token.IsCancellationRequested) 
            return;
        
        SetOwnVignetteIntensity(0);
        SetOwnChromaticAberrationIntensity(0);
    }

    public async void DownedEffect(bool isDown)
    {
        if (_isDowned == isDown)
            return;
        
        _isDowned = isDown;

        if (isDown)
            DamageEffect();
        
        //ScreenSpaceLensFlare
        float value = _screenSpaceLensFlare.intensity.value / _screenSpaceLensFlareMaxValue;

        bool whileLoopCheck = true;
        while (whileLoopCheck)
        {
            SetOwnScreenSpaceLensFlareIntensityClientRpc(value);
            value += isDown ? 0.5f * Time.deltaTime: -2 * Time.deltaTime;

            await Task.Yield();
            
            whileLoopCheck = _resetToken.Token.IsCancellationRequested
                             || _isDowned != isDown 
                             || (isDown && value < 1) 
                             || (!isDown && value > 0);
        }

        
        if (_resetToken.IsCancellationRequested)
        {
            SetOwnScreenSpaceLensFlareIntensityClientRpc(0);
            return;
        }
        
        if (_isDowned == isDown)
            SetOwnScreenSpaceLensFlareIntensityClientRpc(isDown ? 1 : 0);
    }

    public async void DodgingEffect(bool isDodging)
    {
        if (_isDodging == isDodging)
            return;
        
        _isDodging = isDodging;
        
        //MotionBlur
        float motionBlurValue = _motionBlur.intensity.value / _motionBlurMaxValue;

        bool whileLoopCheck = true;
        while (whileLoopCheck)
        {
            SetOwnMotionBlurIntensity(motionBlurValue);
            motionBlurValue += isDodging ? Time.deltaTime : -Time.deltaTime;
            
            await Task.Yield();

            whileLoopCheck = _resetToken.Token.IsCancellationRequested
                             || _isDodging != isDodging 
                             || (isDodging && motionBlurValue < 1)
                             || (!isDodging && motionBlurValue > 0);
        }

        if (_resetToken.IsCancellationRequested)
        {
            SetOwnMotionBlurIntensity(0);
            return;
        }
        
        if (_isDodging == isDodging)
            SetOwnMotionBlurIntensity(isDodging ? 1 : 0);
    }

    public void ResetEffects()
    {
        DodgingEffect(false);
        DownedEffect(false);
        
        _resetToken.Cancel();
        _resetToken = new CancellationTokenSource();
    }
}