
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
    trophyRequired: 300,
    blastIds: [Balt.id, Stagpan.id, Botte.id, Booh.id, Ghoosto.id],
    blastLevels: [2, 6]
}

const theMiniHell: Area = {
    id: 2,
    trophyRequired: 600,
    blastIds: [Goblin.id, MiniDevil.id, DevilDare.id, Masks.id, Luckun.id, MiniHam.id, SadHam.id],
    blastLevels: [5, 9]
}

const theWildForest: Area = {
    id: 3,
    trophyRequired: 1000,
    blastIds: [Bearos.id, Treex.id, Moutmout.id, Piggy.id, Bleaub.id, Shroom.id],
    blastLevels: [8, 12]
}

const theWideOcean: Area = {
    id: 4,
    trophyRequired: 1300,
    blastIds: [Lantern.id, Droplet.id, Fireball.id, Mystical.id, Wormie.id, Smoky.id],
    blastLevels: [12, 15]
}

const theGloryCastle: Area = {
    id: 5,
    trophyRequired: 1600,
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

function getRandomBlastWithAreaId(newAreaId: number, nk: nkruntime.Nakama): Blast {

    let areaId = clamp(newAreaId, 0, allArea.length)

    let randomBlastId = getRandomBlastIdWithAreaId(areaId);
    let blastData = getBlastDataById(randomBlastId);
    let randomLevel = getRandomLevelInArea(areaId);

    let randomIv = getRandomNumber(MinIV, MaxIV);

    let newBlast: Blast = getNewBlast(nk, randomBlastId, randomIv, blastData, randomLevel)

    return newBlast;
}

function getRandomBlastEntityInAllPlayerArea(userId: string, nk: nkruntime.Nakama, logger: nkruntime.Logger): Blast {

    const account = nk.accountGetId(userId);
    const metadata = account.user.metadata as PlayerMetadata;

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