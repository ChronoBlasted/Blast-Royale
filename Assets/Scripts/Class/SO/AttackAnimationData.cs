using System.Threading.Tasks;
using UnityEngine;

public abstract class AAData : ScriptableObject // Attack Animation Data
{
    public abstract Task PlayAnimation(BlastInWorld attacker, BlastInWorld defender, ParticleSystem fxPrefab, System.Action onHitCallback);
}
