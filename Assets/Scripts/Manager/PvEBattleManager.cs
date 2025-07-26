using BaseTemplate.Behaviours;
using Nakama;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PvEBattleManager : BattleBase
{
    public int IndexProgression;
    public int BlastDefeated;
    public int BlastCatched;
    public int BossEncounter;
    public int ShinyEncounter;

    List<Reward> _currentOffers;

    public override void StartBattle(StartStateData startData)
    {
        _serverBattle = NakamaManager.Instance.NakamaBattleManager.PveBattle;

        base.StartBattle(startData);

        IndexProgression = 1;
        BlastDefeated = 0;
        BossEncounter = 0;
        ShinyEncounter = 0;
        BlastCatched = 0;
        CoinGenerated = 0;
        GemGenerated = 0;

        _gameView.UpdateGameviewState(BattleMode.PvE);
        _gameView.SetProgression(IndexProgression);

        _gameView.ExpProgressionLayout.SetSprite(_dataUtils.GetBlastDataRef(_playerMeInfo.ActiveBlast.data_id).Sprite);

    }

    public override Task ShowOpponentBlast()
    {
        _playerOpponentInfo = new PlayerBattleInfo("", BlastOwner.Wild, null, _nextOpponentBlast, new List<Blast> { _nextOpponentBlast }, null);

        return base.ShowOpponentBlast();
    }


    public override async Task StopTurnHandler()
    {
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast) == false || _turnStateData.catched)
        {
            IndexProgression++;

            if (_turnStateData.catched)
            {
                Reward newBlast = new Reward
                {
                    type = RewardType.Blast,
                    blast = _playerOpponentInfo.ActiveBlast,
                };

                BattleReward.Add(newBlast);

                NakamaManager.Instance.NakamaUserAccount.AddPlayerBlast(newBlast.blast);

                string version = "";

                if (newBlast.blast.shiny) version = "3";
                else if (newBlast.blast.boss) version = "2";
                else version = "1";

                NakamaManager.Instance.NakamaBlastTracker.AddBlastTrackerEntry(newBlast.blast.data_id.ToString(), version);

                NakamaManager.Instance.NakamaQuest.UpdateQuest(QuestType.CatchBlast);

                BlastCatched++;
            }
            else
            {
                BlastDefeated++;

                NakamaManager.Instance.NakamaQuest.UpdateQuest(QuestType.DefeatBlast);

                if (_playerOpponentInfo.ActiveBlast.boss) BossEncounter++;
                if (_playerOpponentInfo.ActiveBlast.shiny) ShinyEncounter++;
            }

            CoinGenerated += 200;

            await Task.Delay(500);

            _gameView.AddProgress();

            await Task.Delay(2000);

            if (IndexProgression % 5 == 0 && IndexProgression % 10 != 0)
            {
                ShowOffers();
            }
            else
            {
                await ShowOpponentBlast();
            }
        }
        else if (NakamaLogic.IsAllBlastFainted(_playerMeInfo.Blasts))
        {
            await PlayerLeave();

            return;
        }

        EndTurn();

        bool isOfferStep = IndexProgression % 5 == 0 && IndexProgression % 10 != 0;

        if (isOfferStep == false && NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast))
        {
            NakamaManager.Instance.NakamaBattleManager.CurrentBattle.PlayerReady();
        }
    }


    public void SetNewOffers(List<Reward> newOffers)
    {
        _currentOffers = newOffers;

        UIManager.Instance.WildBattleOfferPopup.Init(newOffers);
    }

    public void ShowOffers()
    {
        UIManager.Instance.WildBattleOfferPopup.OpenPopup(true, false);
    }

    public async void PlayerChooseOffer(int indexOffer)
    {
        try
        {
            NakamaPvEBattle serverBattle = (NakamaPvEBattle)_serverBattle;
            await serverBattle.PlayerChooseOffer(indexOffer);

            switch (_currentOffers[indexOffer].type)
            {
                case RewardType.Coin:
                    CoinGenerated += _currentOffers[indexOffer].amount;
                    break;
                case RewardType.Gem:
                    GemGenerated += _currentOffers[indexOffer].amount;
                    break;
                case RewardType.Blast:
                case RewardType.Item:
                    BattleReward.Add(_currentOffers[indexOffer]);
                    break;
                default:
                    break;
            }

            CoinGenerated += 200;

            IndexProgression++;

            await Task.Delay(500);

            _gameView.AddProgress();

            await Task.Delay(2000);

            await ShowOpponentBlast();

            NakamaManager.Instance.NakamaBattleManager.CurrentBattle.PlayerReady();
        }
        catch (ApiResponseException e) { Debug.LogError(e); }
    }


    public override async Task PlayerLeave()
    {
        if (BonusAds)
        {
            UIManager.Instance.MenuView.FightPanel.PvEBattleBonusAds.RefreshAd();
        }

        await base.PlayerLeave();
    }
}
