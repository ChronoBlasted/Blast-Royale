using UnityEngine;
using DG.Tweening;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "AA/DistanceProjectile")]
public class AA_DistanceProjectile : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        Vector3 direction = (defender.transform.position - attacker.transform.position).normalized;

        attacker.DoCastProjectile(direction * 0.5f);

        var fx = Instantiate(fxPrefab, attacker.transform.position, Quaternion.identity);
        fx.transform.DOMove(defender.transform.position, 0.5f);

        await Task.Delay(500);

        onHitCallback?.Invoke();
    }
}
