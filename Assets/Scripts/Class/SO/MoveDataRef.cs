using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveDataRef", menuName = "ScriptableObjects/NakamaDataRef/NewMoveDataRef", order = 2)]
public class MoveDataRef : NakamaDataRef
{
    public ParticleSystem ParticleSystem;
    public AttackAnimType AttackAnimType;
    public Ease Ease;
}
