
interface Area {
    id: number
    trophyRequired: number
    blastIds: number[];
    blastLevels: [number, number];
}

const thePlains: Area = {
    id: 0,
    trophyRequired: 0,
    blastIds: [Kitchi.id, Kenchi.id, Mousy.id, Clawball.id],
    blastLevels: [1, 3]
}

const theDarkCaves: Area = {
    id: 1,
    trophyRequired: 200,
    blastIds: [Balt.id, Stagpan.id, Botte.id, Booh.id, Ghoosto.id],
    blastLevels: [2, 6]
}

const theMiniHell: Area = {
    id: 2,
    trophyRequired: 500,
    blastIds: [Goblin.id, MiniDevil.id, DevilDare.id, Masks.id, Luckun.id, MiniHam.id, SadHam.id],
    blastLevels: [5, 9]
}

const theWildForest: Area = {
    id: 3,
    trophyRequired: 800,
    blastIds: [Bearos.id, Treex.id, Moutmout.id, Piggy.id, Bleaub.id, Shroom.id],
    blastLevels: [8, 12]
}

const theWideOcean: Area = {
    id: 4,
    trophyRequired: 1100,
    blastIds: [Lantern.id, Droplet.id, Fireball.id, Mystical.id, Wormie.id, Smoky.id],
    blastLevels: [12, 15]
}

const theGloryCastle: Area = {
    id: 5,
    trophyRequired: 1400,
    blastIds: [Clover.id, Scorlov.id, Skel.id, Frederic.id, Bud.id],
    blastLevels: [16, 20]
}

const theElusiveMount: Area = {
    id: 6,
    trophyRequired: 2000,
    blastIds: [Forty.id, Hiboo.id, Eggy.id, Dracoblast.id, Cerberus.id],
    blastLevels: [19, 30]
}

const allArea: Area[] = [
    thePlains,
    theDarkCaves,
    theMiniHell,
    theWildForest,
    theWideOcean,
    theGloryCastle,
    theElusiveMount
];

const rpcLoadAllArea: nkruntime.RpcFunction =
    function (): string {
        return JSON.stringify(allArea);
    }


const rpcSelectArea: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, payload: string) {

        const areaID: number = JSON.parse(payload);

        setMetadataStat(nk, ctx.userId, "area", areaID);

        // TODO Check si il peut

        logger.debug("user '%s' select area '%s'", ctx.userId, areaID);
    }


function getRandomBlastWithAreaId(userId: string, nk: nkruntime.Nakama, extraLevel: number, isBoss: boolean,logger:nkruntime.Logger): Blast {

    try {
        let areaId = clamp(getMetadataStat(nk, userId, "area"), 0, allArea.length)

        let randomBlastId = getRandomBlastIdWithAreaId(areaId);
        let blastData = getBlastDataById(randomBlastId);
        let randomLevel = getRandomLevelInArea(areaId) + extraLevel;

        let randomIv = getRandomNumber(MinIV, MaxIV);

        let newBlast: Blast = getNewBlast(nk, randomBlastId, randomIv, blastData, randomLevel, isBoss);

        return newBlast;
    } catch (error) {
        logger.error('storageRead error: %s', error);
        throw error;
    }

}

function getRandomBlastEntityInAllPlayerArea(userId: string, nk: nkruntime.Nakama, isBoss: boolean): Blast {


    let randomBlastId = getRandomBlastIdInPlayerAreaWithTrophy(getCurrencyInWallet(nk, userId, Currency.Trophies));
    let randomData = getBlastDataById(randomBlastId);
    let randomlevel = getRandomLevelInArea(getMetadataStat(nk, userId, "area")); // TODO Do get max area the player reaches



    let randomIv = getRandomNumber(MinIV, MaxIV);

    let newBlast: Blast = getNewBlast(nk, randomBlastId, randomIv, randomData, randomlevel, isBoss);

    return newBlast;
}

function getRandomBlastIdInPlayerAreaWithTrophy(amountOfTrophy: number): number {

    const allAreaUnderTrophy = getAllAreaUnderTrophy(amountOfTrophy);
    const randomAreaIndex = Math.floor(Math.random() * (allAreaUnderTrophy.length - 1));
    const randomBlastId = getRandomBlastIdWithAreaId(allAreaUnderTrophy[randomAreaIndex].id)
    return randomBlastId;
}

function getAllAreaUnderTrophy(amountOfTrophy: number): Area[] {

    const areaUnderTrophy: Area[] = [];
    for (const area of allArea) {
        if (area.trophyRequired <= amountOfTrophy) {
            areaUnderTrophy.push(area);
        }
    }
    return areaUnderTrophy;
}






function getNewBlast(nk: nkruntime.Nakama, randomBlastId: number, randomIv: number, randomData: BlastData, level: number, isBoss: boolean): Blast {
    return {
        uuid: nk.uuidv4(),
        data_id: randomBlastId,
        exp: calculateExperienceFromLevel(level),
        iv: randomIv,
        boss: isBoss,
        shiny: isShiny(),
        activeMoveset: getRandomActiveMoveset(randomData, calculateExperienceFromLevel(level)),
    };
}

function getRandomBlastIdWithAreaId(id: number): number {

    const area = allArea.find((area) => area.id === id);

    if (area && area.blastIds.length > 0) {
        const randomIndex = Math.floor(Math.random() * area.blastIds.length);
        return area.blastIds[randomIndex];
    }

    return 0;
}

function getRandomLevelInArea(id: number): number {
    const area = allArea.find((area) => area.id === id);

    if (area) {
        const [minLevel, maxLevel] = area.blastLevels;
        const randomLevel = getRandomNumber(minLevel, maxLevel);


        return randomLevel;
    }

    return 0;
}