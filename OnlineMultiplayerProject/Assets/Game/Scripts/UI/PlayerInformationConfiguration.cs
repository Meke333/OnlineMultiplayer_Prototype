using UnityEngine;

[CreateAssetMenu(fileName = nameof(PlayerInformationConfiguration), menuName = "GameConfig/Information")]
public class PlayerInformationConfiguration : ScriptableObject
{
    [field: SerializeField] public Sprite[] PlayerSprite { get; private set; }
}