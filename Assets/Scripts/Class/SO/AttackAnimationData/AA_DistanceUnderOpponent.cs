using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

[CreateAssetMenu(menuName = "AA/DistanceUnderOpponent")]
public class AA_DistanceUnderOpponent : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        attacker.DoCastLaser();

        var fx = Instantiate(fxPrefab, defender.transform.position, Quaternion.identity);
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
