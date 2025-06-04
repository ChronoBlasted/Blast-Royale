using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

[CreateAssetMenu(menuName = "AA/Default")]
public class AA_Default : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        attacker.DoMoveToOpponentRender(defender.BlastRender.transform.position);

        await Task.Delay(500);

        onHitCallback?.Invoke();
    }
}
