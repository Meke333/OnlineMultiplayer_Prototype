using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AttackDamageConfiguration), menuName = "GameConfig/AttackDamage")]
public class AttackDamageConfiguration : ScriptableObject
{
    public float jab_straight;
    public float hook;
    public float gut;

    public float DetermineDamage(AttackType type)
    {
        return type switch
        {
            AttackType.Jab_Straight => jab_straight,
            AttackType.Hook => hook,
            AttackType.Gut => gut,
            _ => throw new InvalidOperationException("AttackType does not exist"),
        };
    }
    
}