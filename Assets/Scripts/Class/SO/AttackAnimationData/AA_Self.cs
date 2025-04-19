using UnityEngine;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "AA/Self")]
public class AA_Self : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        var fx = Instantiate(fxPrefab, attacker.transform);
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
