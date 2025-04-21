using UnityEngine;
using System;
using System.Threading.Tasks;
using DG.Tweening;

[CreateAssetMenu(menuName = "AA/DistanceLaser")]
public class AA_DistanceLaser : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, Action onHitCallback)
    {
        attacker.transform.DOShakePosition(.2f, new Vector3(.2f, .2f));

        var fx = Instantiate(fxPrefab, attacker.transform.position, Quaternion.identity);

        Vector2 direction = (defender.transform.position - attacker.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        fx.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);

        await Task.Delay(500);
        onHitCallback?.Invoke();
    }
}
