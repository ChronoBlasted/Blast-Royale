using Nakama;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardRowLayout : MonoBehaviour
{
    [SerializeField] TMP_Text _playerRank, _playerName, _playerStat;
    [SerializeField] Image _playerIco, _currencyIco;


    public void Init(IApiLeaderboardRecord result, LeaderboardType leaderBoardType)
    {
        _playerRank.text = result.Rank;
        _playerName.text = result.Username;
        _playerStat.text = result.Score;

        switch (leaderBoardType)
        {
            case LeaderboardType.Trophy:
                _currencyIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.Trophy).Sprite;
                break;
            case LeaderboardType.BlastDefeated:

                _currencyIco.sprite = ResourceObjectHolder.Instance.GetResourceByType(ResourceType.BlastDefeated).Sprite;
                break;
        }
    }
}
