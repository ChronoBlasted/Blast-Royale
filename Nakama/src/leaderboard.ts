const LeaderboardTrophyId = "leaderboard_trophy";
const LeaderboardTotalBlastDefeatedId = "leaderboard_blast_defeated";
const LeaderboardBlastDefeatedAreaId = "leaderboard_blast_defeated_area_";
const LeaderboardBestStageAreaId = "leaderboard_best_stage_area_";



function createAreaLeaderboards(nk: nkruntime.Nakama, logger: nkruntime.Logger, ctx: nkruntime.Context) {
    for (const area of allArea) {

        // Leaderboard pour les meilleurs stages dans la zone
        const bestStageLeaderboardId = `${LeaderboardBestStageAreaId}${area.id}`;
        createLeaderboard(nk, logger, bestStageLeaderboardId, nkruntime.Operator.BEST);

        // Leaderboard pour les blasts vaincus dans la zone
        const blastDefeatedLeaderboardId = `${LeaderboardBlastDefeatedAreaId}${area.id}`;
        createLeaderboard(nk, logger, blastDefeatedLeaderboardId, nkruntime.Operator.INCREMENTAL);
    }
}


function createLeaderboard(
    nk: nkruntime.Nakama,
    logger: nkruntime.Logger,
    id: string,
    operator: nkruntime.Operator
): void {
    const authoritative = true;
    const sort = nkruntime.SortOrder.DESCENDING;
    const reset = '0 0 1 * *';

    try {
        nk.leaderboardCreate(id, authoritative, sort, operator, reset, undefined);
        logger.info(`Leaderboard '${id}' created successfully.`);
    } catch (error) {
        logger.error(`Failed to create leaderboard '${id}': ${error}`);
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

function writeRecordLeaderboard(
    nk: nkruntime.Nakama,
    logger: nkruntime.Logger,
    userId: string,
    leaderboardId: string,
    score: number,
    operator: nkruntime.OverrideOperator
) {
    try {
        nk.leaderboardsGetId([leaderboardId]);
    } catch (error: any) {
        logger.error("Leaderboard dont exist error: %s", JSON.stringify(error));
    }

    const username = nk.accountGetId(userId).user.username;

    try {
        nk.leaderboardRecordWrite(leaderboardId, userId, username, score, 0, undefined, operator);
    } catch (error: any) {
        logger.error("Leaderboard write error: %s", JSON.stringify(error));
    }
}

function writeIncrementalRecordLeaderboard(
    nk: nkruntime.Nakama,
    logger: nkruntime.Logger,
    userId: string,
    leaderboardId: string,
    score: number
) {
    const incrementType: nkruntime.OverrideOperator =
        score > 0 ? nkruntime.OverrideOperator.INCREMENTAL : nkruntime.OverrideOperator.DECREMENTAL;
    writeRecordLeaderboard(nk, logger, userId, leaderboardId, score, incrementType);
}

function writeBestRecordLeaderboard(
    nk: nkruntime.Nakama,
    logger: nkruntime.Logger,
    userId: string,
    leaderboardId: string,
    score: number
) {
    writeRecordLeaderboard(nk, logger, userId, leaderboardId, score, nkruntime.OverrideOperator.BEST);
}