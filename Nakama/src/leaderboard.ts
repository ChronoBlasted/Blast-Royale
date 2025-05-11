const LeaderboardTrophyId = "leaderboard_trophy";
const LeaderboardBlastDefeatedId = "leaderboard_blast_defeated";

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

    let id = LeaderboardBlastDefeatedId;
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

function writeRecordLeaderboard(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string, leaderboardId: string, score: number) {

    const incrementType: nkruntime.OverrideOperator =
        score > 0 ? nkruntime.OverrideOperator.INCREMENTAL : nkruntime.OverrideOperator.DECREMENTAL;

    try {
        nk.leaderboardsGetId([leaderboardId]);
    } catch (error: any) {
        logger.error("Leaderboard dont exist error: %s", JSON.stringify(error));
    }

    var username = nk.accountGetId(userId).user.username

    try {
        nk.leaderboardRecordWrite(leaderboardId, userId, username, score, 0, undefined, incrementType);
    } catch (error: any) {
        logger.error("Leaderboard write error: %s", JSON.stringify(error));
    }
}