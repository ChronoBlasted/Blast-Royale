using Nakama;
using Nakama.TinyJson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class NakamaPvEBattle : NakamaBattleBase
{

    public override void HandleMatchState(IMatchState matchState)
    {
        base.HandleMatchState(matchState);

        string messageJson = DecodeMatchState(matchState);

        switch (matchState.OpCode)
        {

            case NakamaOpCode.NEW_OFFER_TURN:
                var newOffers = JsonUtility.FromJson<OfferTurnStateData>(messageJson);
                var offerList = new List<Offer> { newOffers.offerOne, newOffers.offerTwo, newOffers.offerThree };

                PvEBattleManager battleManager = BattleManager as PvEBattleManager;
                battleManager.SetNewOffers(offerList);
                break;
        }
    }

    public async Task PlayerChooseOffer(int indexOffer)
    {
        try
        {
            await _socket.SendMatchStateAsync(_matchId, NakamaOpCode.PLAYER_CHOOSE_OFFER, indexOffer.ToJson(), null);

            BattleManager.WaitForOpponent();


        }
        catch (ApiResponseException e)
        {
            Debug.LogWarning("Error Player Choose Offer: " + e.Message);
        }
    }
}
