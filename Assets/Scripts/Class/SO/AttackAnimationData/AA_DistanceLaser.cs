using UnityEngine;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "AA/DistanceLaser")]
public class AA_DistanceLaser : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        var fx = Instantiate(fxPrefab, attacker.transform.position, Quaternion.identity);
        fx.transform.LookAt(defender.transform.position);
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
