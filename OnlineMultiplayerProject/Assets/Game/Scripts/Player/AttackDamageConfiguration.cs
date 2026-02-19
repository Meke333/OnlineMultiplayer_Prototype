using System;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AttackDamageConfiguration), menuName = "GameConfig/AttackDamage")]
public class AttackDamageConfiguration : ScriptableObject
{
    public float left_jab;
    public float right_straight;
    public float left_hook;
    public float right_hook;
    public float left_gut;
    public float right_gut;

    public float DetermineDamage(AttackType type, int direction)
    {
        float result = 0;
        
        //left
        if (direction < 0)
        {
            //Debug.Log("Left Attack");
            result = type switch
            {
                AttackType.Jab_Straight => left_jab,
                AttackType.Hook => left_hook,
                AttackType.Gut => left_gut,
            };
        }
        //right
        else if (direction > 0)
        {
            //Debug.Log("Right Attack");
            result = type switch
            {
                AttackType.Jab_Straight => right_straight,
                AttackType.Hook => right_hook,
                AttackType.Gut => right_gut,
            };
        }

        return result;
    }
    
}