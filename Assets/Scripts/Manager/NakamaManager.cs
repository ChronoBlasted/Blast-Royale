using BaseTemplate.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NakamaManager : MonoSingleton<NakamaManager>
{
    [field: SerializeField] public NakamaAuth NakamaAuth { get; protected set; }
    [field: SerializeField] public NakamaUserAccount NakamaUserAccount { get; protected set; }
    [field: SerializeField] public NakamaDailyReward NakamaDailyReward { get; protected set; }
    [field: SerializeField] public NakamaPedia NakamaPedia { get; protected set; }
    [field: SerializeField] public NakamaFriends NakamaFriends { get; protected set; }
    [field: SerializeField] public NakamaLeaderboards NakamaLeaderboards { get; protected set; }
    [field: SerializeField] public NakamaArea NakamaArea { get; protected set; }
    [field: SerializeField] public NakamaLogic NakamaLogic { get; protected set; }
    [field: SerializeField] public NakamaStore NakamaStore { get; protected set; }
    [field: SerializeField] public NakamaWildBattle NakamaWildBattle { get; protected set; }
    [field: SerializeField] public NakamaNotifications NakamaNotifications { get; protected set; }

    public void Init()
    {
        NakamaAuth.Init();

        NakamaUserAccount.Init();

        NakamaDailyReward.Init();

        NakamaPedia.Init();

        NakamaFriends.Init();

        NakamaLeaderboards.Init();

        NakamaArea.Init();

        NakamaStore.Init();

        NakamaWildBattle.Init();

        NakamaNotifications.Init();

        GameManager.Instance.UpdateStateToMenu();
    }
}
