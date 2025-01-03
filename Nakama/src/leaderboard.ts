const LeaderboardTrophyId = "leaderboard_trophy";
const LeaderboardBlastDefeated = "leaderboard_blast_defeated";

function createTrophyLeaderboard(nk: nkruntime.Nakama, logger: nkruntime.Logger, ctx: nkruntime.Context) {

    let id = LeaderboardTrophyId;
    let authoritative = true;
    let sort = nkruntime.SortOrder.DESCENDING;
    let operator = nkruntime.Operator.BEST;
    let reset = "0 0 1 */2 *";

    try {
        nk.leaderboardCreate(id, authoritative, sort, operator, reset, undefined);
    } catch (error) {
        // Handle error
    }
}

function createBlastDefeatedLeaderboard(nk: nkruntime.Nakama, logger: nkruntime.Logger, ctx: nkruntime.Context) {

    let id = LeaderboardBlastDefeated;
    let authoritative = true;
    let sort = nkruntime.SortOrder.DESCENDING;
    let operator = nkruntime.Operator.INCREMENTAL;
    let reset = '0 0 1 */2 *';

    try {
        nk.leaderboardCreate(id, authoritative, sort, operator, reset, undefined);
    } catch (error) {
        // Handle error
    }
}

const rpcGetAroundTrophyLeaderboard: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {

        let result: nkruntime.LeaderboardRecordList;
        let id = LeaderboardTrophyId;
        let ownerIds: string[] = [ctx.userId];
        let limit = 100;
        let cursor = '';
        let overrideExpiry = 3600;

        try {
            result = nk.leaderboardRecordsList(id, ownerIds, limit, cursor, overrideExpiry);
        } catch (error) {
            // Handle error
        }
        return JSON.stringify(loadUserBlast(nk, logger, ctx.userId));
    }


let leaderboardReset: nkruntime.LeaderboardResetFunction = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, leaderboard: nkruntime.Leaderboard, reset: number) {

    if (leaderboard.id !== LeaderboardTrophyId) {
        return;
    }

    let result = nk.leaderboardRecordsList(leaderboard.id, undefined, undefined, undefined, reset);

    // If leaderboard is top tier and has 10 or more players, relegate bottom 3 players

    result.records!.forEach(function (r) {
        // Enlever /2 au dessus de 400 tr

        // nk.leaderboardRecordWrite(bottomTierId, r.ownerId, r.username, r.score, r.subscore, null, null);
        // nk.leaderboardRecordDelete(topTierId, r.ownerId);
    });
};

function writeRecordTrophyLeaderboard(nk: nkruntime.Nakama, logger: nkruntime.Logger, ctx: nkruntime.Context) {

    let id = LeaderboardTrophyId;
    let ownerID = ctx.userId;
    let username = ctx.username;
    let score = getCurrencyInWallet(nk,ctx.userId,Currency.Trophies);
    
    let result: nkruntime.LeaderboardRecord;
    
    try {
      result = nk.leaderboardRecordWrite(id, ownerID, username, score, undefined, undefined);
    } catch(error) {
      // Handle error
    }
}

function writeRecordBlastDefeatedLeaderboard(nk: nkruntime.Nakama, logger: nkruntime.Logger, ctx: nkruntime.Context,amount:number) {

    let id = LeaderboardBlastDefeated;
    let ownerID = ctx.userId;
    let username = ctx.username;
    let score = amount;
    
    let result: nkruntime.LeaderboardRecord;
    
    try {
      result = nk.leaderboardRecordWrite(id, ownerID, username, score, undefined, undefined);
    } catch(error) {
      // Handle error
    }
}

