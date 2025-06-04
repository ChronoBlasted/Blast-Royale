using DG.Tweening;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMoveDataRef", menuName = "ScriptableObjects/NakamaDataRef/NewMoveDataRef", order = 2)]
public class MoveDataRef : NakamaDataRef
{
    public ParticleSystem ParticleSystem;
    public AAData AA_Data;
    public AA_Type AttackAnimType;

}
