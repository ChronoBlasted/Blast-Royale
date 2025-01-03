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

function getRandomActiveMoveset(blastData: BlastData, exp: number): number[] {

    const availableMoves = blastData.movepool
        .filter(m => calculateLevelFromExperience(exp) >= m.levelMin)
        .map(m => m.move_id);

    const shuffledMoves = shuffleArray(availableMoves);
    const randomMoveset = shuffledMoves.slice(0, 4);

    return randomMoveset;
}




function clamp(value: number, min: number, max: number): number {
    return Math.min(Math.max(value, min), max);
}

function getRandomNumber(min: number, max: number): number {
    const minCeiled = Math.ceil(min);
    const maxFloored = Math.floor(max);
    return Math.floor(Math.random() * (maxFloored - minCeiled + 1)) + minCeiled;
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
