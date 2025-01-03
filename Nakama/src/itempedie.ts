enum ItemBehaviour {
    NONE,
    HEAL,
    MANA,
    STATUS,
    CATCH,
};

enum Status {
    NONE,
    SLEEP,
    BURN,
    POISONOUS,
    WET,
};

enum Rarity {
    NONE,
    COMMON,
    UNCOMMON,
    RARE,
    EPIC,
    LEGENDARY,
    ULTIMATE,
    UNIQUE,
}

interface Item {
    data_id: number
    amount: number
}

interface ItemData {
    id: number;
    name: string;
    desc: string;
    behaviour: ItemBehaviour;
    gain_amount: number
    status: Status;
    catchRate: number;
    rarity:Rarity;
}

interface ItemUseJSON {
    index_item: number
    index_blast: number
}

const healthPotionData: ItemData = {
    id: 1,
    name: "Potion",
    desc: "Give 20 HP",
    behaviour: ItemBehaviour.HEAL,
    gain_amount: 20,
    status: Status.NONE,
    catchRate: 0,
    rarity:Rarity.COMMON,
};

const superHealthPotionData: ItemData = {
    id: 2,
    name: "Super Potion",
    desc: "Give 50 HP",
    behaviour: ItemBehaviour.HEAL,
    gain_amount: 50,
    status: Status.NONE,
    catchRate: 0,
    rarity:Rarity.COMMON,
};

const hyperHealthPotionData: ItemData = {
    id: 3,
    name: "Hyper Potion",
    desc: "Give 200 HP",
    behaviour: ItemBehaviour.HEAL,
    gain_amount: 200,
    status: Status.NONE,
    catchRate: 0,
    rarity:Rarity.COMMON,
};

const manaPotionData: ItemData = {
    id: 4,
    name: "Elixir",
    desc: "Give 10 Mana",
    behaviour: ItemBehaviour.MANA,
    gain_amount: 10,
    status: Status.NONE,
    catchRate: 0,
    rarity:Rarity.COMMON,
};

const superManaPotionData: ItemData = {
    id: 5,
    name: "Super Elixir",
    desc: "Give 25 Mana",
    behaviour: ItemBehaviour.MANA,
    gain_amount: 25,
    status: Status.NONE,
    catchRate: 0,
    rarity:Rarity.COMMON,
};

const hyperManaPotionData: ItemData = {
    id: 6,
    name: "Hyper Elixir",
    desc: "Give 100 Mana",
    behaviour: ItemBehaviour.MANA,
    gain_amount: 100,
    status: Status.NONE,
    catchRate: 0,
    rarity:Rarity.COMMON,
};

const blastTrapData: ItemData = {
    id: 7,
    name: "BlastTrap",
    desc: "Catch with bonus 1",
    behaviour: ItemBehaviour.CATCH,
    gain_amount: 0,
    status: Status.NONE,
    catchRate: 1,
    rarity:Rarity.COMMON,
};

const superBlastTrapData: ItemData = {
    id: 8,
    name: "Super BlastTrap",
    desc: "Catch with bonus 1.5",
    behaviour: ItemBehaviour.CATCH,
    gain_amount: 0,
    status: Status.NONE,
    catchRate: 1.5,
    rarity:Rarity.COMMON,
};

const hyperBlastTrapData: ItemData = {
    id: 9,
    name: "Hyper BlastTrap",
    desc: "Catch with bonus 2",
    behaviour: ItemBehaviour.CATCH,
    gain_amount: 0,
    status: Status.NONE,
    catchRate: 2,
    rarity:Rarity.COMMON,
};

const itemPedia: ItemData[] = [
    healthPotionData,
    superHealthPotionData,
    hyperHealthPotionData,
    manaPotionData,
    superManaPotionData,
    hyperManaPotionData,
    blastTrapData,
    superBlastTrapData,
    hyperBlastTrapData,
];

function getItemDataById(id: number): ItemData {
    const item = itemPedia.find((item) => item.id === id);
    if (!item) {
        throw new Error(`No Item found with ID: ${id}`);
    }
    return item;
}

const rpcLoadItemPedia: nkruntime.RpcFunction =
    function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
        return JSON.stringify(itemPedia);
    }


function getRandomItem(amount: number): Item {

    const randomIndex = Math.floor(Math.random() * itemPedia.length);

    let newItem: Item = {
        data_id: itemPedia[randomIndex].id,
        amount: amount,
    }

    return newItem;
}