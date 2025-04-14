
enum ITEM_BEHAVIOUR {
    NONE,
    HEAL,
    MANA,
    STATUS,
    CATCH,
};

interface Item {
    data_id: number
    amount: number
}

interface ItemData {
    id: number;
    behaviour: ITEM_BEHAVIOUR;
    gain_amount: number
    rarity:Rarity;
    status?: Status;
    catchRate?: number;
}

interface ItemUseJSON {
    index_item: number
    index_blast: number
}

const potionData: ItemData = { // healthPotionData
    id: 0,
    behaviour: ITEM_BEHAVIOUR.HEAL,
    gain_amount: 20,
    rarity: Rarity.COMMON,
};

const superPotionData: ItemData = { // superHealthPotionData
    id: 1,
    behaviour: ITEM_BEHAVIOUR.HEAL,
    gain_amount: 50,
    rarity:Rarity.UNCOMMON,
};

const hyperPotionData: ItemData = { // hyperHealthPotionData
    id: 2,
    behaviour: ITEM_BEHAVIOUR.HEAL,
    gain_amount: 200,
    rarity:Rarity.RARE,
};

const elixirData: ItemData = { // manaPotionData
    id: 3,
    behaviour: ITEM_BEHAVIOUR.MANA,
    gain_amount: 10,
    rarity:Rarity.COMMON,
};

const superElixirData: ItemData = { // superManaPotionData
    id: 4,
    behaviour: ITEM_BEHAVIOUR.MANA,
    gain_amount: 25,
    rarity:Rarity.UNCOMMON,
};

const hyperElixirData: ItemData = { // hyperManaPotionData
    id: 5,
    behaviour: ITEM_BEHAVIOUR.MANA,
    gain_amount: 50,
    rarity:Rarity.RARE,
};

const blastTrapData: ItemData = { // blastTrapData
    id: 6,
    behaviour: ITEM_BEHAVIOUR.CATCH,
    gain_amount: 0,
    catchRate: 1,
    rarity:Rarity.COMMON,
};

const superBlastTrapData: ItemData = { // superBlastTrapData
    id: 7,
    behaviour: ITEM_BEHAVIOUR.CATCH,
    gain_amount: 0,
    catchRate: 1.5,
    rarity:Rarity.COMMON,
};

const hyperBlastTrapData: ItemData = { // hyperBlastTrapData
    id: 8,
    behaviour: ITEM_BEHAVIOUR.CATCH,
    gain_amount: 0,
    catchRate: 2,
    rarity:Rarity.COMMON,
};

const AntiBurnData: ItemData = { // hyperBlastTrapData
    id: 9,
    behaviour: ITEM_BEHAVIOUR.STATUS,
    gain_amount: 0,
    status: Status.Burn,
    rarity:Rarity.COMMON,
};

const AntiSeededData: ItemData = { // hyperBlastTrapData
    id: 10,
    behaviour: ITEM_BEHAVIOUR.STATUS,
    gain_amount: 0,
    status: Status.Seeded,
    rarity:Rarity.COMMON,
};

const AntiWetData: ItemData = { // hyperBlastTrapData
    id: 11,
    behaviour: ITEM_BEHAVIOUR.STATUS,
    gain_amount: 0,
    status: Status.Wet,
    rarity:Rarity.COMMON,
};

const itemPedia: ItemData[] = [
    potionData,
    superPotionData,
    hyperPotionData,
    elixirData,
    superElixirData,
    hyperElixirData,
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

function getDeckItem(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): Item[] {

    let userCards: ItemCollection;
    userCards = loadUserItems(nk, logger, userId);

    return userCards.deckItems;
}