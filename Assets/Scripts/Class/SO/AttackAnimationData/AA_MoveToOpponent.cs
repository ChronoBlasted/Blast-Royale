

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "AA/MoveToOpponent")]
public class AA_MoveToOpponent : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, System.Action onHitCallback)
    {
        attacker.DoMoveToOpponentRender(defender.BlastRender.transform.position);

        await Task.Delay(500);

        var fx = Instantiate(fxPrefab, defender.transform);

        onHitCallback?.Invoke();
    }
}
