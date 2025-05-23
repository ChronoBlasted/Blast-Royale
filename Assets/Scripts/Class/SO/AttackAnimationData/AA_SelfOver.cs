using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

[CreateAssetMenu(menuName = "AA/SelfOver")]
public class AA_SelfOver : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        attacker.DoSelfRender();

        var position = attacker.transform.position + Vector3.up * 2f;
        var fx = Instantiate(fxPrefab, position, Quaternion.identity);
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
