using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

[CreateAssetMenu(menuName = "AA/Self")]
public class AA_Self : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        attacker.DoSelfRenderPunchPosition(new Vector3(0, .25f, 0));

        if (fxPrefab != null)
        {
            var fx = Instantiate(fxPrefab, attacker.transform);
        }
        else
        {
            Debug.LogWarning("FX Prefab is null, no visual effect will be played.");
        }
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
