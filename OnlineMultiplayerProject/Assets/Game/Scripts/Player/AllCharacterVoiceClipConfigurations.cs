using UnityEngine;

[CreateAssetMenu(fileName = nameof(AllCharacterVoiceClipConfigurations), menuName = "GameConfig/AllVoiceConfigs")]
public class AllCharacterVoiceClipConfigurations : ScriptableObject
{
    [field: SerializeField] public CharacterVoiceClipConfiguration[] VoiceClips { get; private set; }
}