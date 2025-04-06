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

function addPlatformType(p_platform: Type[], newType: Type): Type[] {
    if (p_platform.length < 3) {
        p_platform.push(newType);
    } else {
        p_platform.shift();
        p_platform.push(newType);
    }

    return p_platform;
}
function getAmountOfPlatformTypeByType(p_platform: Type[], typeToCount: Type): number {
    return p_platform.filter(type => type === typeToCount).length;
}

function removePlatformTypeByType(p_platform: Type[], typeToRemove: Type, numberToRemove: number): Type[] {
    let removedCount = 0;

    for (let i = p_platform.length - 1; i >= 0 && removedCount < numberToRemove; i--) {
        if (p_platform[i] === typeToRemove) {
            p_platform.splice(i, 1);
            removedCount++;
        }
    }

    return p_platform;
}


function calculateDamage(
    attackerLevel: number,
    attackerAttack: number,
    defenderDefense: number,
    attackType: Type,
    defenderType: Type,
    movePower: number,
    meteo: Meteo,
): number {
    const weatherModifier = calculateWeatherModifier(meteo, attackType);

    const damage: number = ((2 * attackerLevel / 5 + 2) * movePower * getTypeMultiplier(attackType, defenderType) * (attackerAttack / defenderDefense) / 50) * weatherModifier;

    return Math.floor(damage);
}

function getTypeMultiplier(moveType: Type, defenderType: Type): number {
    switch (moveType) {
        case Type.FIRE:
            switch (defenderType) {
                case Type.GRASS:
                    return 2;
                case Type.WATER:
                    return 0.5;
                default:
                    return 1;
            }

        case Type.WATER:
            switch (defenderType) {
                case Type.FIRE:
                    return 2;
                case Type.GRASS:
                    return 0.5;
                default:
                    return 1;
            }

        case Type.GRASS:
            switch (defenderType) {
                case Type.WATER:
                    return 2;
                case Type.FIRE:
                    return 0.5;
                default:
                    return 1;
            }

        case Type.NORMAL:
            switch (defenderType) {
                case Type.LIGHT:
                    return 0.5;
                case Type.DARK:
                    return 0.5;
                default:
                    return 1;
            }

        case Type.GROUND:
            switch (defenderType) {
                case Type.ELECTRIC:
                    return 2;
                case Type.FLY:
                    return 0;
                default:
                    return 1;
            }

        case Type.FLY:
            switch (defenderType) {
                case Type.ELECTRIC:
                    return 0;
                case Type.GROUND:
                    return 2;
                default:
                    return 1;
            }

        case Type.ELECTRIC:
            switch (defenderType) {
                case Type.GROUND:
                    return 0;
                case Type.FLY:
                    return 2;
                default:
                    return 1;
            }

        case Type.LIGHT:
            switch (defenderType) {
                case Type.DARK:
                    return 2;
                case Type.NORMAL:
                    return 2;
                case Type.LIGHT:
                    return 0.5;
                default:
                    return 1;
            }

        case Type.DARK:
            switch (defenderType) {
                case Type.LIGHT:
                    return 2;
                case Type.NORMAL:
                    return 2;
                case Type.DARK:
                    return 0.5;
                default:
                    return 1;
            }

        default:
            return 1;
    }
}

function calculateWeatherModifier(weather: Meteo, moveType: Type): number {
    let modifier = 1.0;

    switch (weather) {
        case Meteo.Sun:
            if (moveType === Type.FIRE) {
                modifier = 1.5;
            }
            break;

        case Meteo.Rain:
            if (moveType === Type.WATER) {
                modifier = 1.5;
            }
            break;

        case Meteo.Leaves:
            if (moveType === Type.GRASS) {
                modifier = 1.5;
            }

            break;

        case Meteo.None:
            break;
    }

    return modifier;
}

function calculateEffectWithProbability(blast: Blast, move: Move): Blast {
    const statusEffectProbabilities: { [key in MoveEffect]?: number } = {
        [MoveEffect.Burn]: 0.1,
        [MoveEffect.Seeded]: 0.1,
        [MoveEffect.Wet]: 0.1,
        [MoveEffect.ManaExplosion]: 0.2,
        [MoveEffect.HpExplosion]: 0.2,
        [MoveEffect.ManaRestore]: 0.2,
        [MoveEffect.HpRestore]: 0.2,
        [MoveEffect.AttackBoost]: 0.5,
        [MoveEffect.DefenseBoost]: 0.5,
        [MoveEffect.SpeedBoost]: 0.5,
        [MoveEffect.AttackReduce]: 0.5,
        [MoveEffect.DefenseReduce]: 0.5,
        [MoveEffect.SpeedReduce]: 0.5,
        [MoveEffect.Cleanse]: 0.5,
    };

    // Ajuste la probabilité en fonction de l'entrée "probability"
    const effectProbability = statusEffectProbabilities[move.effect!];
    if (Math.random() < effectProbability!) {
        return applyEffect(blast, move);
    }

    return blast;
}


function applyEffect(blast: Blast, move: Move): Blast {
    switch (move.effect) {
        case MoveEffect.Burn:
            blast.status = Status.Burn;
            break;
        case MoveEffect.Seeded:
            blast.status = Status.Seeded;
            break;
        case MoveEffect.Wet:
            blast.status = Status.Wet;
            break;

        case MoveEffect.ManaExplosion:
            const manaDmg = Math.floor(blast.maxMana / 2);
            blast.hp = Math.max(0, blast.hp - manaDmg);
            blast.mana = Math.floor(blast.mana / 2);
            break;
        case MoveEffect.HpExplosion:
            const hpCost = Math.floor(blast.maxHp / 3);
            blast.hp = Math.max(0, blast.hp - hpCost);
            break;

        case MoveEffect.ManaRestore:
            blast.mana = Math.min(blast.maxMana, blast.mana + move.power);
            break;
        case MoveEffect.HpRestore:
            blast.hp = Math.min(blast.maxHp, blast.hp + move.power);
            break;

        case MoveEffect.AttackBoost:
            blast.attack = Math.floor(blast.attack * 1.5);
            blast.attack = Math.min(blast.attack, 500);
            break;
        case MoveEffect.DefenseBoost:
            blast.defense = Math.floor(blast.defense * 1.5);
            blast.defense = Math.min(blast.defense, 500);
            break;
        case MoveEffect.SpeedBoost:
            blast.speed = Math.floor(blast.speed * 1.5);
            blast.speed = Math.min(blast.speed, 500);
            break;

        case MoveEffect.AttackReduce:
            blast.attack = Math.max(1, Math.floor(blast.attack * 0.75));
            break;
        case MoveEffect.DefenseReduce:
            blast.defense = Math.max(1, Math.floor(blast.defense * 0.75));
            break;
        case MoveEffect.SpeedReduce:
            blast.speed = Math.max(1, Math.floor(blast.speed * 0.75));
            break;

        case MoveEffect.Cleanse:
            blast.status = Status.None;
            break;
    }

    return blast;
}


function applyStatusEffectAtStartOfTurn(blast: Blast, otherBlast: Blast, move: Move): { blast: Blast, otherBlast: Blast, move: Move } {

    switch (blast.status) {
        case Status.Wet:
            move.priority = Math.max(-2, move.priority - 1);
            break;

        default:
            break;
    }

    return { blast, otherBlast, move };
}

function applyStatusEffectAtEndOfTurn(blast: Blast, otherBlast: Blast): { blast: Blast, otherBlast: Blast} {
    switch (blast.status) {
        case Status.Burn:
            blast.hp = Math.max(0, blast.hp - Math.floor(blast.maxHp / 8));
            break;

        case Status.Seeded:
            const healAmount = Math.floor(blast.maxHp / 16);

            blast.hp = Math.max(0, blast.hp - healAmount);
            otherBlast.hp = Math.min(otherBlast.maxHp, otherBlast.hp + healAmount);
            break;

        default:
            break;
    }

    return { blast, otherBlast };
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

function getRandomMeteo(): Meteo {
    const values = Object.values(Meteo).filter(value => typeof value === "number") as Meteo[];
    return randomElement(values);
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
