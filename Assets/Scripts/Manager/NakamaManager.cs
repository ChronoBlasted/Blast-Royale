using BaseTemplate.Behaviours;
using Nakama;
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

    public IClient Client { get; private set; }
    public ISession Session { get; private set; }
    public ISocket Socket { get; private set; }

    public void Init()
    {
        NakamaAuth.Init();
    }

    public async void AuthUser(IClient client, ISession session, ISocket socket)
    {
        Client = client;
        Session = session;
        Socket = socket;

        await NakamaUserAccount.Init(Client, Session);

        GameManager.Instance.UpdateStateToMenu();
    }
}
