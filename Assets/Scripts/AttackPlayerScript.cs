using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayerScript : IAttack
{
    private float range;
    private float aaCooldown;
    private float timeAfterAA = 0;

    public AttackPlayerScript(float range, float aaCooldown)
    {
        this.range = range;
        this.aaCooldown = aaCooldown;
    }

    public int TryAttack(Vector3 currentPosition, Vector3 targetPosition)
    {
        if (!this.IsInRange(currentPosition, targetPosition))
        {
            Debug.Log("Move to attack");
            return (int) Enums.AttackResult.OutOfRange;
        }

        else if (!this.IsAttackOnCooldown())
        {
            Debug.Log("Attack");
            return (int) Enums.AttackResult.CanAttack;
        }

        else
        {
            //Debug.Log("Attack on cd");
            return (int)Enums.AttackResult.OnCooldown;
        }
    }

    private bool IsInRange(Vector3 currentPosition, Vector3 targetPosition)
    {
        return range >= Vector3.Distance(currentPosition, targetPosition);
    }

    private bool IsAttackOnCooldown()
    {
        bool isOnCd = this.timeAfterAA < this.aaCooldown;
        this.AdjustTimeAfterAA(isOnCd);
        return isOnCd;
    }

    private void AdjustTimeAfterAA(bool isOnCd)
    {
        timeAfterAA = isOnCd ? timeAfterAA + Time.deltaTime : 0;
    }

    // zadanie obrazen - done +/-
    // rzucenie jakiegos projectile, ktory tylko cel moze trafic
    // otwartosc na blockowanie projectile
    // jezeli poza zasiegiem to idz do celu, sprawdzenie czy nie za szybko (aa cd) - done
    // sprawdzenie czy w zasiegu - done
    // aa cooldown - done
    // nie rusza sie do czasu wypuszczenia projectile albo zaatakowania
}