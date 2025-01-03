
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
