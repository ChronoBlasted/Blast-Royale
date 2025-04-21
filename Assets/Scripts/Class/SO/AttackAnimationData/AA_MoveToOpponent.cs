

using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "AA/MoveToOpponent")]
public class AA_MoveToOpponent : AAData
{
    public override async Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, System.Action onHitCallback)
    {
        attacker.BlastRender.transform.DOMove(defender.BlastRender.transform.position, 0.5f).SetEase(Ease.OutSine).SetLoops(2, LoopType.Yoyo);

        await Task.Delay(500);

        var fx = Instantiate(fxPrefab, defender.transform);

        onHitCallback?.Invoke();
    }
}
