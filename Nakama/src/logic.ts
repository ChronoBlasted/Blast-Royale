const healManaPerRound = 20;
const healManaPerWait = 50;

function calculateBlastStat(baseStat: number, iv: number, level: number): number {
    return Math.floor(((2 * baseStat + iv) * level) / 100 + 5);
}

function calculateBlastHp(baseHp: number, iv: number, level: number): number {
    return Math.floor(((2 * baseHp + iv) * level) / 100 + level + 10);
}

function calculateBlastMana(baseMana: number, iv: number, level: number): number {
    return Math.floor(((baseMana + iv) * (level / 100) + level / 2) + 10);
}

function calculateLevelFromExperience(experience: number): number {
    if (experience < 0) {
        throw new Error("L'expérience totale ne peut pas être négative.");
    }

    let niveau = 1;
    let experienceNiveau = 0;

    for (let i = 1; i <= 100; i++) {
        experienceNiveau = Math.floor((i ** 3) * 100 / 2);
        if (experience < experienceNiveau) {
            break;
        }
        niveau = i;
    }

    return niveau;
}

function calculateExperienceFromLevel(level: number): number {
    if (level < 1 || level > 100) {
        throw new Error("Le niveau doit être compris entre 1 et 100.");
    }

    let experienceNiveau = 0;

    for (let i = 1; i <= level; i++) {
        experienceNiveau = Math.floor((i ** 3) * 100 / 2);
    }

    return experienceNiveau;
}

function calculateExperienceGain(expYield: number, enemyLevel: number, yourLevel: number): number {
    const experience: number = Math.floor(((expYield * enemyLevel / 7) * ((2 * enemyLevel + 10) / (enemyLevel + yourLevel + 10)) + 1));
    return experience;
}

function getRandomActiveMoveset(blastData: BlastData, exp: number): number[] {

    const availableMoves = blastData.movepool
        .filter(m => calculateLevelFromExperience(exp) >= m.levelMin)
        .map(m => m.move_id);

    const shuffledMoves = shuffleArray(availableMoves);
    const randomMoveset = shuffledMoves.slice(0, 4);

    return randomMoveset;
}

//#region Battle

function calculateDamage(
    attackerLevel: number,
    attackerAttack: number,
    defenderDefense: number,
    attackerType: TYPE,
    defenderType: TYPE,
    movePower: number
): number {
    const damage: number = ((2 * attackerLevel / 5 + 2) * movePower * getTypeMultiplier(attackerType, defenderType) * (attackerAttack / defenderDefense) / 50) + 1;
    return Math.floor(damage);
}

function getTypeMultiplier(moveType: TYPE, defenderType: TYPE): number {
    switch (moveType) {
        case TYPE.FIRE:
            switch (defenderType) {
                case TYPE.GRASS:
                    return 2;
                case TYPE.WATER:
                    return 0.5;
                default:
                    return 1;
            }

        case TYPE.WATER:
            switch (defenderType) {
                case TYPE.FIRE:
                    return 2;
                case TYPE.GRASS:
                    return 0.5;
                default:
                    return 1;
            }

        case TYPE.GRASS:
            switch (defenderType) {
                case TYPE.WATER:
                    return 2;
                case TYPE.FIRE:
                    return 0.5;
                default:
                    return 1;
            }

        case TYPE.NORMAL:
            switch (defenderType) {
                case TYPE.LIGHT:
                    return 0.5;
                case TYPE.DARK:
                    return 0.5;
                default:
                    return 1;
            }

        case TYPE.GROUND:
            switch (defenderType) {
                case TYPE.ELECTRIC:
                    return 2;
                case TYPE.FLY:
                    return 0;
                default:
                    return 1;
            }

        case TYPE.FLY:
            switch (defenderType) {
                case TYPE.ELECTRIC:
                    return 0;
                case TYPE.GROUND:
                    return 2;
                default:
                    return 1;
            }

        case TYPE.ELECTRIC:
            switch (defenderType) {
                case TYPE.GROUND:
                    return 0;
                case TYPE.FLY:
                    return 2;
                default:
                    return 1;
            }

        case TYPE.LIGHT:
            switch (defenderType) {
                case TYPE.DARK:
                    return 2;
                case TYPE.NORMAL:
                    return 2;
                case TYPE.LIGHT:
                    return 0.5;
                default:
                    return 1;
            }

        case TYPE.DARK:
            switch (defenderType) {
                case TYPE.LIGHT:
                    return 2;
                case TYPE.NORMAL:
                    return 2;
                case TYPE.DARK:
                    return 0.5;
                default:
                    return 1;
            }

        default:
            return 1;
    }
}


function calculateStaminaRecovery(
    maxStamina: number,
    currentStamina: number,
    useWait: boolean = false
): number {
    const normalRecovery: number = maxStamina * 0.2;
    const waitRecovery: number = maxStamina * 0.5;

    let recoveredStamina: number = currentStamina + (useWait ? waitRecovery : normalRecovery);

    if (recoveredStamina > maxStamina) {
        recoveredStamina = maxStamina;
    }

    return Math.floor(recoveredStamina);
}

function getFasterBlast(blast1: Blast, blast2: Blast): boolean {
    if (blast1.speed > blast2.speed) {
        return true;
    } else {
        return false;
    }
}

function isAllBlastDead(allPlayerBlasts: Blast[]): boolean {
    return allPlayerBlasts.every((blast) => blast.hp === 0);
}

function isBlastAlive(blast: Blast): boolean {
    return blast.hp > 0;
}

function addExpOnBlastInGame(nk: nkruntime.Nakama, logger: nkruntime.Logger, playerId: string, currentPlayerBlast: Blast, enemyBlast: Blast) {
    let expToAdd = calculateExperienceGain(getBlastDataById(currentPlayerBlast.data_id).expYield, calculateLevelFromExperience(enemyBlast.exp), calculateLevelFromExperience(currentPlayerBlast.exp));
    addExpOnBlast(nk, logger, playerId, currentPlayerBlast.uuid, expToAdd);
}


function healHealthBlast(blast: Blast, amount: number): Blast {
    blast.hp += amount;

    if (blast.hp > blast.maxHp) blast.hp = blast.maxHp;

    return blast;
}
function healManaBlast(blast: Blast, amount: number): Blast {
    blast.mana += amount;

    if (blast.mana > blast.maxMana) blast.mana = blast.maxMana;

    return blast;
}

function calculateCaptureProbability(currentHP: number, maxHP: number, catchRate: number, temCardBonus: number, statusBonus: number): number {
    const hpFactor = (3 * maxHP - 2 * currentHP) / (3 * maxHP);
    const baseProbability = catchRate * hpFactor * temCardBonus * statusBonus;

    const captureProbability = Math.min(Math.max(baseProbability, 0), 1);

    return captureProbability;
}

function isBlastCaptured(
    currentHP: number,
    maxHP: number,
    catchRate: number,
    temCardBonus: number,
    statusBonus: number
): boolean {
    const captureProbability = calculateCaptureProbability(currentHP, maxHP, catchRate, temCardBonus, statusBonus) * 100;

    const randomValue = Math.random() * 100;

    return randomValue <= captureProbability;
}

//#endregion



function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}

function getRandomNumber(min: number, max: number): number {
    const minCeiled = Math.ceil(min);
    const maxFloored = Math.floor(max);
    return Math.floor(Math.random() * (maxFloored - minCeiled + 1)) + minCeiled;
}

function randomElement<T>(array: T[]): T {
    const randomIndex = Math.floor(Math.random() * array.length);
    return array[randomIndex];
}

function shuffleArray<T>(array: T[]): T[] {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}

function msecToSec(n: number): number {
    return Math.floor(n / 1000);
}
