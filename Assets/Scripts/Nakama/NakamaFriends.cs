using Nakama;
using System.Threading.Tasks;
using UnityEngine;

public class NakamaFriends : MonoBehaviour
{
    IClient _client;
    ISession _session;

    public async Task Init(IClient client, ISession session)
    {
        _client = client;
        _session = session;

        await ShowAllFriends();
    }

    public async void AddFriends(string friendName)
    {
        await NakamaManager.Instance.Client.AddFriendsAsync(NakamaManager.Instance.Session, null, new[] { friendName });
    }

    public async Task ShowAllFriends()
    {
        var limit = 100;
        var result = await NakamaManager.Instance.Client.ListFriendsAsync(NakamaManager.Instance.Session, null, limit, cursor: null);

        UIManager.Instance.FriendView.UpdateFriendList(result);
    }

    public async void AcceptFriend(string username)
    {
        await _client.AddFriendsAsync(_session, new[] { username });
    }

    public async void DeleteFriend(string username)
    {
        await _client.DeleteFriendsAsync(_session, null, new[] { username });
    }
}
