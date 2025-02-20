
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
    blastLevels: [2, 5]
}

const theDarkCaves: Area = {
    id: 1,
    trophyRequired: 300,
    blastIds: [Balt.id, Stagpan.id, Botte.id, Booh.id, Ghoosto.id],
    blastLevels: [4, 10]
}

const theMiniHell: Area = {
    id: 2,
    trophyRequired: 600,
    blastIds: [Goblin.id, MiniDevil.id, DevilDare.id,Masks.id,Luckun.id,MiniHam.id,SadHam.id],
    blastLevels: [8, 15]
}

const theWildForest: Area = {
    id: 3,
    trophyRequired: 1000,
    blastIds: [Bearos.id, Treex.id, Moutmout.id,Piggy.id,Bleaub.id,Shroom.id],
    blastLevels: [13, 20]
}

const theWideOcean: Area = {
    id: 4,
    trophyRequired: 1300,
    blastIds: [Lantern.id, Droplet.id, Fireball.id,Mystical.id,Wormie.id,Smoky.id],
    blastLevels: [17, 25]
}

const theGloryCastle: Area = {
    id: 5,
    trophyRequired: 1600,
    blastIds: [Clover.id, Scorlov.id, Skel.id,Frederic.id,Bud.id],
    blastLevels: [22, 30]
}

const theElusiveMount: Area = {
    id: 6,
    trophyRequired: 2000,
    blastIds: [Forty.id, Hiboo.id, Eggy.id,Dracoblast.id,Cerberus.id],
    blastLevels: [27, 40]
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

// function getRandomBlastInPlayerArea(userId: string, logger: nkruntime.Logger, nk: nkruntime.Nakama): Blast {

//     let metadata = nk.accountGetId(userId).user.metadata;

//     let randomBlastId = getRandomBlastIdInPlayerAreaIdInArea(metadata.area);
//     let blastData = getBlastDataById(randomBlastId);
//     let randomLevel = getRandomLevelInArea(metadata.area);

//     let randomIv = getRandomNumber(MinIV, MaxIV);

//     let newBlast: Blast = getNewBlast(nk, randomBlastId, randomIv, blastData, randomLevel)

//     logger.debug('user %s successfully get a random blast', userId);

//     return newBlast;
// }

function getRandomBlastInAllPlayerArea(userId: string, nk: nkruntime.Nakama): Blast {

    let metadata = nk.accountGetId(userId).user.metadata;

    let randomBlastId = getRandomBlastIdInPlayerAreaWithTrophy(getCurrencyInWallet(nk, userId, Currency.Trophies));
    let randomData = getBlastDataById(randomBlastId);
    let randomlevel = getRandomLevelInArea(metadata.area);

    let randomIv = getRandomNumber(MinIV, MaxIV);

    let newBlast: Blast = getNewBlast(nk, randomBlastId, randomIv, randomData, randomlevel)

    return newBlast;
}


function getNewBlast(nk: nkruntime.Nakama, randomBlastId: number, randomIv: number, randomData: BlastData, level: number): Blast {
    return {
        uuid: nk.uuidv4(),
        data_id: randomBlastId,
        exp: calculateExperienceFromLevel(level),
        iv: randomIv,
        hp: calculateBlastHp(randomData.hp, randomIv, level),
        maxHp: calculateBlastHp(randomData.hp, randomIv, level),
        mana: calculateBlastMana(randomData.mana, randomIv, level),
        maxMana: calculateBlastMana(randomData.mana, randomIv, level),
        attack: calculateBlastStat(randomData.attack, randomIv, level),
        defense: calculateBlastStat(randomData.defense, randomIv, level),
        speed: calculateBlastStat(randomData.speed, randomIv, level),
        status: STATUS.NONE,
        activeMoveset: getRandomActiveMoveset(randomData, calculateExperienceFromLevel(level))
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

function getRandomLevelInArea(id: number): number {
    const area = allArea.find((area) => area.id === id);

    if (area) {
        const [minLevel, maxLevel] = area.blastLevels;
        const randomLevel = getRandomNumber(minLevel, maxLevel);


        return randomLevel;
    }

    return 0;
}