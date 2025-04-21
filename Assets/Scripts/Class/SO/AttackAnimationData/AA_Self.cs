using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

[CreateAssetMenu(menuName = "AA/Self")]
public class AA_Self : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        attacker.DoSelfRender();

        var fx = Instantiate(fxPrefab, attacker.transform);
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
