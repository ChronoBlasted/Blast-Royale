using UnityEngine;
using System;
using System.Threading.Tasks;

[CreateAssetMenu(menuName = "AA/DistanceOverOpponent")]
public class AA_DistanceOverOpponent : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        var position = defender.transform.position + Vector3.up * 2f;
        var fx = Instantiate(fxPrefab, position, Quaternion.identity);
        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
