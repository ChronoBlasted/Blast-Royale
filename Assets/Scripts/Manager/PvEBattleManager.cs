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

    public int CoinGenerated;
    public int GemGenerated;

    List<Offer> _currentOffers;

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
        _playerOpponentInfo = new PlayerBattleInfo("", BlastOwner.Wild, _nextOpponentBlast, new List<Blast> { _nextOpponentBlast }, null);

        return base.ShowOpponentBlast();
    }


    public override async Task StopTurnHandler()
    {
        if (NakamaLogic.IsBlastAlive(_playerOpponentInfo.ActiveBlast) == false || _turnStateData.catched)
        {
            IndexProgression++;

            if (_turnStateData.catched)
            {
                Offer newBlast = new Offer
                {
                    type = OfferType.Blast,
                    blast = _playerOpponentInfo.ActiveBlast,
                };

                BattleReward.Add(newBlast);

                BlastCatched++;
            }
            else
            {
                BlastDefeated++;

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
            await PlayerLeave(false);
        }

        EndTurn();

        bool isOfferStep = IndexProgression % 5 == 0 && IndexProgression % 10 != 0;

        if (isOfferStep == false && NakamaLogic.IsBlastAlive(_playerMeInfo.ActiveBlast))
        {
            NakamaManager.Instance.NakamaBattleManager.CurrentBattle.PlayerReady();
        }
    }


    public void SetNewOffers(List<Offer> newOffers)
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
            await _serverBattle.PlayerChooseOffer(indexOffer);

            switch (_currentOffers[indexOffer].type)
            {
                case OfferType.Coin:
                    CoinGenerated += _currentOffers[indexOffer].coinsAmount;
                    break;
                case OfferType.Gem:
                    GemGenerated += _currentOffers[indexOffer].gemsAmount;
                    break;
                case OfferType.Blast:
                case OfferType.Item:
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


    public override async Task PlayerLeave(bool leaveMatch)
    {
        if (leaveMatch) await _serverBattle.LeaveMatch();

        if (CoinGenerated > 0)
        {
            if (BonusAds)
            {
                Offer coinBonus = new Offer();

                coinBonus.type = OfferType.Coin;
                coinBonus.coinsAmount = CoinGenerated / 2;
                coinBonus.isBonus = true;

                BattleReward.Insert(0, coinBonus);
            }

            Offer coinReward = new Offer();

            coinReward.type = OfferType.Coin;
            coinReward.coinsAmount = CoinGenerated;

            BattleReward.Insert(0, coinReward);
        }

        if (GemGenerated > 0)
        {
            if (BonusAds)
            {
                Offer gemBonus = new Offer();

                gemBonus.type = OfferType.Gem;
                gemBonus.coinsAmount = GemGenerated / 2;
                gemBonus.isBonus = true;

                BattleReward.Insert(0, gemBonus);
            }

            Offer gemReward = new Offer();

            gemReward.type = OfferType.Gem;
            gemReward.gemsAmount = GemGenerated;

            BattleReward.Insert(0, gemReward);
        }

        if (BonusAds)
        {
            UIManager.Instance.MenuView.FightPanel.PvEBattleBonusAds.RefreshAd();
        }

        UIManager.Instance.ChangeView(UIManager.Instance.EndViewPve);
    }



}
