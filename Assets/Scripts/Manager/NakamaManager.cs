using BaseTemplate.Behaviours;
using Nakama;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
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
    [field: SerializeField] public NakamaBattleManager NakamaBattleManager { get; protected set; }
    [field: SerializeField] public NakamaNotifications NakamaNotifications { get; protected set; }
    [field: SerializeField] public NakamaBlastTracker NakamaBlastTracker { get; protected set; }
    [field: SerializeField] public NakamaQuest NakamaQuest { get; protected set; }

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

        await NakamaBlastTracker.Init(Client, Session);

        await NakamaPedia.Init(Client, Session);

        await NakamaUserAccount.Init(Client, Session);

        await NakamaArea.Init(Client, Session);

        await NakamaDailyReward.Init(Client, Session);

        await NakamaFriends.Init(Client, Session);

        await NakamaLeaderboards.Init(Client, Session);

        await NakamaStore.Init(Client, Session);

        await NakamaQuest.Init(Client, Session);

        NakamaNotifications.Init(Client, Session, Socket);

        NakamaBattleManager.Init(Client, Session, Socket);

        GameManager.Instance.AfterNakamaInit();
    }
}
