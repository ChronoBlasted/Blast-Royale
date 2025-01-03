
interface Area {
    id: number
    name: string
    trophyRequired: number
    blastIds: number[];
    blastLevels: [number, number];
}

const thePlains: Area = {
    id: 0,
    name: "The Plains",
    trophyRequired: 0,
    blastIds: [Zephyrex.id, Gnawbit.id],
    blastLevels: [2, 5]
}

const theWildForests: Area = {
    id: 1,
    name: "The Wild Forests",
    trophyRequired: 300,
    blastIds: [Gnawbit.id, Zephyrwing.id, Electrix.id],
    blastLevels: [6, 10]
}

const theDarkCaves: Area = {
    id: 2,
    name: "The Dark Caves",
    trophyRequired: 600,
    blastIds: [Florax.id, Pyrex.id, Aquaflare.id],
    blastLevels: [11, 20]
}

const allArea: Area[] = [
    theDarkCaves,
    theWildForests,
    thePlains,
];

const rpcLoadAllArea: nkruntime.RpcFunction =
    function (): string {
        return JSON.stringify(allArea);
    }

    function getRandomBlastInPlayerArea(userId: string, logger: nkruntime.Logger, nk: nkruntime.Nakama): Blast {

        let metadata = nk.accountGetId(userId).user.metadata;
    
        let randomBlastId = getRandomBlastIdInPlayerAreaIdInArea(metadata.area);
        let blastData = getBlastDataById(randomBlastId);
        let randomblastLevels = calculateExperienceFromLevel(getRandomLevelInArea(metadata.area));
    
        let randomIv = getRandomNumber(MinIV, MaxIV);
    
        let newBlast: Blast = {
            uuid: nk.uuidv4(),
            data_id: randomBlastId,
            exp: randomblastLevels,
            iv: randomIv,
            hp: calculateBlastHp(blastData.hp, randomIv, calculateLevelFromExperience(randomblastLevels)),
            maxHp: calculateBlastHp(blastData.hp, randomIv, calculateLevelFromExperience(randomblastLevels)),
            mana: calculateBlastMana(blastData.mana, randomIv, calculateLevelFromExperience(randomblastLevels)),
            maxMana: calculateBlastMana(blastData.mana, randomIv, calculateLevelFromExperience(randomblastLevels)),
            attack: calculateBlastStat(blastData.attack, randomIv, calculateLevelFromExperience(randomblastLevels)),
            defense: calculateBlastStat(blastData.defense, randomIv, calculateLevelFromExperience(randomblastLevels)),
            speed: calculateBlastStat(blastData.speed, randomIv, calculateLevelFromExperience(randomblastLevels)),
            status: Status.NONE,
            activeMoveset: getRandomActiveMoveset(blastData, randomblastLevels)
        }
    
        logger.debug('user %s successfully get a random blast', userId);
    
        return newBlast;
    }
    
    function getRandomBlastInAllPlayerArea(userId: string, nk: nkruntime.Nakama): Blast {
    
        let metadata = nk.accountGetId(userId).user.metadata;
    
        let randomBlastId = getRandomBlastIdInPlayerAreaWithTrophy(getCurrencyInWallet(nk, userId, Currency.Trophies));
        let randomData = getBlastDataById(randomBlastId);
        let randomblastLevels = getRandomLevelInArea(metadata.area);
    
        let randomIv = getRandomNumber(MinIV, MaxIV);
    
        let newBlast: Blast = {
            uuid: nk.uuidv4(),
            data_id: randomBlastId,
            exp: calculateExperienceFromLevel(getRandomLevelInArea(metadata.area)),
            iv: randomIv,
            hp: calculateBlastHp(randomData.hp, randomIv, randomblastLevels),
            maxHp: calculateBlastHp(randomData.hp, randomIv, randomblastLevels),
            mana: calculateBlastMana(randomData.mana, randomIv, randomblastLevels),
            maxMana: calculateBlastMana(randomData.mana, randomIv, randomblastLevels),
            attack: calculateBlastStat(randomData.attack, randomIv, randomblastLevels),
            defense: calculateBlastStat(randomData.defense, randomIv, randomblastLevels),
            speed: calculateBlastStat(randomData.speed, randomIv, randomblastLevels),
            status: Status.NONE,
            activeMoveset: getRandomActiveMoveset(randomData, calculateExperienceFromLevel(getRandomLevelInArea(metadata.area)))
        }
    
        return newBlast;
    }
    
    
    function getRandomBlastIdInPlayerAreaIdInArea(id: number): number {
    
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
        const randomBlastId = getRandomBlastIdInPlayerAreaIdInArea(allAreaUnderTrophy[randomAreaIndex].id)
        return randomBlastId;
    }
    
    function getAllAreaUnderTrophy(amountOfTrophy: number): Area[] {
    
        const areaUnderTrophy: Area[] = [];
        for (const aire of allArea) {
            if (aire.trophyRequired <= amountOfTrophy) {
                areaUnderTrophy.push(aire);
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