
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
    status: STATUS;
    catchRate: number;
    rarity:RARITY;
}

interface ItemUseJSON {
    index_item: number
    index_blast: number
}

const healthPotionData: ItemData = { // healthPotionData
    id: 0,
    behaviour: ITEM_BEHAVIOUR.HEAL,
    gain_amount: 20,
    status: STATUS.NONE,
    catchRate: 0,
    rarity: RARITY.COMMON,
};

const superHealthPotionData: ItemData = { // superHealthPotionData
    id: 1,
    behaviour: ITEM_BEHAVIOUR.HEAL,
    gain_amount: 50,
    status: STATUS.NONE,
    catchRate: 0,
    rarity:RARITY.UNCOMMON,
};

const hyperHealthPotionData: ItemData = { // hyperHealthPotionData
    id: 2,
    behaviour: ITEM_BEHAVIOUR.HEAL,
    gain_amount: 200,
    status: STATUS.NONE,
    catchRate: 0,
    rarity:RARITY.RARE,
};

const manaPotionData: ItemData = { // manaPotionData
    id: 3,
    behaviour: ITEM_BEHAVIOUR.MANA,
    gain_amount: 10,
    status: STATUS.NONE,
    catchRate: 0,
    rarity:RARITY.COMMON,
};

const superManaPotionData: ItemData = { // superManaPotionData
    id: 4,
    behaviour: ITEM_BEHAVIOUR.MANA,
    gain_amount: 25,
    status: STATUS.NONE,
    catchRate: 0,
    rarity:RARITY.UNCOMMON,
};

const hyperManaPotionData: ItemData = { // hyperManaPotionData
    id: 5,
    behaviour: ITEM_BEHAVIOUR.MANA,
    gain_amount: 50,
    status: STATUS.NONE,
    catchRate: 0,
    rarity:RARITY.RARE,
};

const blastTrapData: ItemData = { // blastTrapData
    id: 6,
    behaviour: ITEM_BEHAVIOUR.CATCH,
    gain_amount: 0,
    status: STATUS.NONE,
    catchRate: 1,
    rarity:RARITY.COMMON,
};

const superBlastTrapData: ItemData = { // superBlastTrapData
    id: 7,
    behaviour: ITEM_BEHAVIOUR.CATCH,
    gain_amount: 0,
    status: STATUS.NONE,
    catchRate: 1.5,
    rarity:RARITY.COMMON,
};

const hyperBlastTrapData: ItemData = { // hyperBlastTrapData
    id: 8,
    behaviour: ITEM_BEHAVIOUR.CATCH,
    gain_amount: 0,
    status: STATUS.NONE,
    catchRate: 2,
    rarity:RARITY.COMMON,
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

function getDeckItem(nk: nkruntime.Nakama, logger: nkruntime.Logger, userId: string): Item[] {

    let userCards: ItemCollection;
    userCards = loadUserItems(nk, logger, userId);

    return userCards.deckItems;
}