// region Setup 

enum BattleState {
    NONE,
    WAITING,
    READY,
    START,
    WAITFORPLAYERSWAP,
    WAITFORPLAYERCHOOSEOFFER,
    END,
}

enum PlayerState {
    NONE,
    BUSY,
    READY,
}

enum TurnType {
    NONE,
    ATTACK,
    ITEM,
    SWAP,
    WAIT
}

interface WildBattleData {
    emptyTicks: number

    presences: { [userId: string]: nkruntime.Presence | null }

    battle_state: BattleState;

    player1_state: PlayerState;
    player1_id: string;

    p1_index: number;
    p1_blasts: BlastEntity[];
    player1_items: Item[];
    player1_platform: Type[];

    wild_blast: BlastEntity | null;
    wild_blast_platform: Type[];

    index_progression: number;
    blast_defeated: number;
    blast_catched: number;

    meteo: Meteo

    turnStateData: TurnStateData;
    offerTurnStateData: OfferTurnStateData;
}

interface StartStateData {
    newBlastData: NewBlastData;
    meteo: Meteo;
}

interface NewBlastData {
    id: number;
    exp: number;
    iv: number;
    boss: boolean;
    shiny: boolean;
    status: Status;
    activeMoveset: number[];
}

interface OfferTurnStateData {
    offer_one: Offer;
    offer_two: Offer;
    offer_three: Offer;
}

interface TurnStateData {
    p_move_damage: number;
    p_move_effects: MoveEffectData[];

    wb_turn_type: TurnType;
    wb_move_index: number;
    wb_move_damage: number;
    wb_move_effects: MoveEffectData[];

    catched: boolean;
}

function rpcFindOrCreateWildBattle(context: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama): string {
    var matchId = nk.matchCreate('wildBattle', {});
    return JSON.stringify(matchId);
}

const matchInit = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, params: { [key: string]: string }): { state: WildBattleData, tickRate: number, label: string } {

    const wildBattleData: WildBattleData = {
        emptyTicks: 0,
        presences: {},
        battle_state: BattleState.START,

        player1_state: PlayerState.BUSY,
        player1_id: "",

        p1_index: 0,
        p1_blasts: [],
        player1_items: [],
        player1_platform: [],

        wild_blast: null,
        wild_blast_platform: [],

        index_progression: 1,
        blast_defeated: 0,
        blast_catched: 0,

        meteo: Meteo.None,

        turnStateData: {
            p_move_damage: 0,
            p_move_effects: [],

            wb_turn_type: TurnType.NONE,
            wb_move_index: 0,
            wb_move_damage: 0,
            wb_move_effects: [],

            catched: false
        },

        offerTurnStateData: {
            offer_one: {
                type: OfferType.NONE,
                coinsAmount: 0,
                gemsAmount: 0,
                blast: null,
                item: null,
            },
            offer_two: {
                type: OfferType.NONE,
                coinsAmount: 0,
                gemsAmount: 0,
                blast: null,
                item: null,
            },
            offer_three: {
                type: OfferType.NONE,
                coinsAmount: 0,
                gemsAmount: 0,
                blast: null,
                item: null,
            }
        }
    };

    return {
        state: wildBattleData,
        tickRate: 2, // 1 tick per second = 1 MatchLoop func invocations per second
        label: ''
    };
};


const matchJoinAttempt = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, presence: nkruntime.Presence, metadata: { [key: string]: any }): { state: WildBattleData, accept: boolean, rejectMessage?: string | undefined } | null {
    logger.debug('%q attempted to join Lobby match', ctx.userId);

    return {
        state,
        accept: true
    };
}

const matchJoin = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, presences: nkruntime.Presence[]): { state: WildBattleData } | null {
    for (const presence of presences) {
        state.emptyTicks = 0;
        state.presences[presence.userId] = presence;
    }

    return {
        state
    };
}

// region Leave 

const matchLeave = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, presences: nkruntime.Presence[]): { state: WildBattleData } | null {
    for (let presence of presences) {
        logger.info("Player: %s left match: %s.", presence.userId, ctx.matchId);

        if (state.player1_id == presence.userId) {

            PlayerLeave(nk, state, logger);
            dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(true));
        }

        state.presences[presence.userId] = null;
    }

    for (let userID in state.presences) {
        if (state.presences[userID] === null) {
            delete state.presences[userID];
        }
    }

    if (connectedPlayers(state) === 0) {
        return null;
    }

    return {
        state
    };
}


// region MatchLoop 

const matchLoop = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, messages: nkruntime.MatchMessage[]): { state: WildBattleData } | null {

    switch (state.battle_state) {
        case BattleState.START:

            logger.debug('______________ START BATTLE ______________');

            const keys = Object.keys(state.presences);
            const player1_presence = state.presences[keys[0]]!;

            state.player1_id = player1_presence.userId;

            var allPlayer1BlastInBattle = getDeckBlast(nk, logger, state.player1_id);
            state.p1_index = 0;
            state.p1_blasts = allPlayer1BlastInBattle;

            var allPlayer1Items = getDeckItem(nk, logger, state.player1_id);
            state.player1_items = allPlayer1Items;

            logger.debug('______________ CREAT NEW BLAST ______________');
            var newBlast = GetNewWildBlast(state, nk, logger);
            logger.debug('______________ FIND NEW BLAST ______________');

            state.wild_blast = ConvertBlastToBlastEntity(newBlast);

            const newWildBlast: NewBlastData = {
                id: state.wild_blast.data_id,
                exp: state.wild_blast.exp,
                iv: state.wild_blast.iv,
                boss: state.wild_blast.boss,
                shiny: state.wild_blast.shiny,
                activeMoveset: state.wild_blast.activeMoveset,
                status: Status.None
            }

            state.meteo = getRandomMeteo();

            const StartData: StartStateData = {
                newBlastData: newWildBlast,
                meteo: state.meteo,
            }

            logger.debug('Random blast with id: %d, lvl: %l appeared', state.wild_blast.data_id, calculateLevelFromExperience(state.wild_blast.exp));

            state.battle_state = BattleState.WAITING;

            dispatcher.broadcastMessage(OpCodes.MATCH_START, JSON.stringify(StartData));

            logger.debug('______________ END START BATTLE ______________');

            break;
        case BattleState.WAITING:

            messages.forEach(function (message) {
                switch (message.opCode) {
                    case OpCodes.PLAYER_READY:

                        state.player1_state = PlayerState.READY;

                        logger.debug('______________ PLAYER 1 READY ______________');
                        break;
                }
            });

            if (state.player1_state == PlayerState.READY) {

                dispatcher.broadcastMessage(OpCodes.ENEMY_READY);
                state.battle_state = BattleState.READY;

                logger.debug('______________ EVERYONE"S READY ______________');
            }

            break;
        case BattleState.READY:

            messages.forEach(function (message) {

                const validOpCodes = [
                    OpCodes.PLAYER_ATTACK,
                    OpCodes.PLAYER_USE_ITEM,
                    OpCodes.PLAYER_CHANGE_BLAST,
                    OpCodes.PLAYER_WAIT,
                    OpCodes.PLAYER_LEAVE,
                ];

                if (!validOpCodes.includes(message.opCode)) {
                    ({ state } = ErrorFunc(state, "OP CODE NOT VALID", dispatcher, BattleState.READY));
                    return;
                }

                logger.debug('______________ LOOP BATTLE ______________');

                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));

                state.player1_state = PlayerState.BUSY;

                state.turnStateData = {
                    p_move_damage: 0,
                    p_move_effects: [],

                    wb_move_index: getRandomUsableMove(getMovesByIds(state.wild_blast!.activeMoveset!), state.wild_blast!.mana, state.wild_blast_platform),
                    wb_move_damage: 0,
                    wb_move_effects: [],

                    wb_turn_type: TurnType.NONE,

                    catched: false
                }


                switch (message.opCode) {
                    // region Attack
                    case OpCodes.PLAYER_ATTACK:
                        let attackIndex = clamp(JSON.parse(nk.binaryToString(message.data)), 0, 3);
                        let move = getMoveById(state.p1_blasts[state.p1_index]!.activeMoveset![attackIndex]);

                        if (move == null) {
                            ({ state } = ErrorFunc(state, "Player 1 move null", dispatcher, BattleState.READY));
                            return;
                        }

                        state = performAttackSequence(state, move, dispatcher, nk, logger);
                        break;
                    // region Player Use Item
                    case OpCodes.PLAYER_USE_ITEM:
                        let msgItem = {} as ItemUseJSON;
                        msgItem = JSON.parse(nk.binaryToString(message.data));
                        msgItem.index_item = clamp(msgItem.index_item, 0, state.player1_items.length - 1)
                        let item = state.player1_items[msgItem.index_item];

                        if (item == null) {
                            ({ state } = ErrorFunc(state, "Item to use is null", dispatcher, BattleState.READY));
                            return;
                        }

                        if (item.amount <= 0) {
                            ({ state } = ErrorFunc(state, "U don't have enough item", dispatcher, BattleState.READY));
                            return;
                        }

                        useItem(nk, logger, state.player1_id, item);

                        var itemData = getItemDataById(item.data_id);

                        switch (itemData.behaviour) {
                            case ITEM_BEHAVIOUR.HEAL:
                                state.p1_blasts[msgItem.index_blast] = healHealthBlast(state.p1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ITEM_BEHAVIOUR.MANA:
                                state.p1_blasts[msgItem.index_blast] = healManaBlast(state.p1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ITEM_BEHAVIOUR.STATUS:
                                state.p1_blasts[msgItem.index_blast] = healStatusBlast(state.p1_blasts[msgItem.index_blast], itemData.status!);
                                break;
                            case ITEM_BEHAVIOUR.CATCH:
                                var wildBlastCaptured = false;

                                wildBlastCaptured = isBlastCaptured(state.wild_blast!.hp, state.wild_blast!.maxHp, getBlastDataById(state.wild_blast!.data_id).catchRate, itemData.catchRate!, 1) // TODO Get status bonus

                                if (wildBlastCaptured) {
                                    logger.debug('Wild blast Captured !', wildBlastCaptured);

                                    state.battle_state = BattleState.END;
                                }

                                state.turnStateData.catched = wildBlastCaptured;
                                break;
                            default:
                        }

                        ({ state } = executeWildBlastAttack(state, dispatcher, logger));
                        break;
                    // region Player Change
                    case OpCodes.PLAYER_CHANGE_BLAST:
                        var msgChangeBlast = clamp(JSON.parse(nk.binaryToString(message.data)), 0, state.p1_blasts.length - 1);

                        if (state.p1_index == msgChangeBlast) {
                            ErrorFunc(state, "Cannot change actual blast with actual blast", dispatcher, BattleState.READY);
                            return;
                        }

                        if (!isBlastAlive(state.p1_blasts[msgChangeBlast])) {
                            ({ state } = ErrorFunc(state, "Cannot change actual blast with dead blast in Ready", dispatcher, BattleState.READY));
                            return;
                        }

                        state.p1_index = msgChangeBlast;

                        ({ state } = executeWildBlastAttack(state, dispatcher, logger));
                        break;
                    // region Player Wait
                    case OpCodes.PLAYER_WAIT:
                        state.player1_state = PlayerState.BUSY;

                        state.p1_blasts[state.p1_index]!.mana = calculateManaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, true);

                        ({ state } = executeWildBlastAttack(state, dispatcher, logger));
                        break;

                    case OpCodes.PLAYER_LEAVE:
                        PlayerLeave(nk, state, logger);
                        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(true));

                        return null;
                }

                // region End turn Logic

                ({ blast: state.p1_blasts[state.p1_index]!, otherBlast: state.wild_blast! } = applyStatusEffectAtEndOfTurn(state.p1_blasts[state.p1_index]!, state.wild_blast!));
                ({ blast: state.wild_blast!, otherBlast: state.p1_blasts[state.p1_index]! } = applyStatusEffectAtEndOfTurn(state.wild_blast!, state.p1_blasts[state.p1_index]!));

                state = checkIfMatchContinue(state);

                if (state.battle_state == BattleState.END) {
                    if (isBlastAlive(state.wild_blast!)) state.wild_blast!.mana = calculateManaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);
                    if (isBlastAlive(state.p1_blasts[state.p1_index]!)) state.p1_blasts[state.p1_index]!.mana = calculateManaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, false);

                    dispatcher.broadcastMessage(OpCodes.NEW_BATTLE_TURN, JSON.stringify(state.turnStateData));

                    return;
                }
                else if (state.battle_state == BattleState.WAITFORPLAYERSWAP) {
                    state.wild_blast!.mana = calculateManaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);

                    dispatcher.broadcastMessage(OpCodes.NEW_BATTLE_TURN, JSON.stringify(state.turnStateData));

                    EndLoopDebug(logger, state);

                    return;
                }
                else {
                    state.battle_state = BattleState.WAITING;

                    state.p1_blasts[state.p1_index]!.mana = calculateManaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, false);
                    state.wild_blast!.mana = calculateManaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);

                    //Send matchTurn
                    dispatcher.broadcastMessage(OpCodes.NEW_BATTLE_TURN, JSON.stringify(state.turnStateData));
                }

                EndLoopDebug(logger, state);
            });
            break;
        // region WAITFORPLAYERSWAP
        case BattleState.WAITFORPLAYERSWAP:

            messages.forEach(function (message) {

                const validOpCodes = [
                    OpCodes.PLAYER_CHANGE_BLAST,
                ];

                if (!validOpCodes.includes(message.opCode)) {
                    ({ state } = ErrorFunc(state, "OP CODE NOT VALID", dispatcher, BattleState.WAITFORPLAYERSWAP));
                    return;
                }

                logger.debug('______________ PLAYER SWAP BLAST ______________');

                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));

                if (message.opCode == OpCodes.PLAYER_CHANGE_BLAST) {

                    state.player1_state = PlayerState.BUSY;

                    var msgChangeBlast = clamp(JSON.parse(nk.binaryToString(message.data)), 0, state.p1_blasts.length - 1);

                    if (state.p1_index == msgChangeBlast) {
                        ErrorFunc(state, "Cannot change actual blast with actual blast", dispatcher, BattleState.WAITFORPLAYERSWAP);
                        return;
                    }

                    if (!isBlastAlive(state.p1_blasts[msgChangeBlast])) {
                        ({ state } = ErrorFunc(state, "Cannot change actual blast with dead blast", dispatcher, BattleState.WAITFORPLAYERSWAP));
                        return;
                    }

                    state.p1_index = msgChangeBlast;
                }

                state.battle_state = BattleState.WAITING;

                logger.debug('______________ END PLAYER SWAP BLAST ______________');
                logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);
                logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp, state.p1_blasts[state.p1_index]?.mana);
            });
            break;
        //region CHOOSE OFFER
        case BattleState.WAITFORPLAYERCHOOSEOFFER:
            messages.forEach(function (message) {

                const validOpCodes = [
                    OpCodes.PLAYER_CHOOSE_OFFER,
                ];

                if (!validOpCodes.includes(message.opCode)) {
                    ({ state } = ErrorFunc(state, "OP CODE NOT VALID", dispatcher, BattleState.WAITFORPLAYERSWAP));
                    return;
                }

                logger.debug('______________ PLAYER CHOOSE OFFER ______________');

                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));

                if (message.opCode == OpCodes.PLAYER_CHOOSE_OFFER) {
                    state.index_progression++;

                    state.player1_state = PlayerState.BUSY;

                    var indexChooseOffer = clamp(JSON.parse(nk.binaryToString(message.data)), 0, 2);

                    let currentOffer: Offer = {
                        type: OfferType.NONE,
                        coinsAmount: 0,
                        gemsAmount: 0,
                        blast: null,
                        item: null,
                    };

                    switch (indexChooseOffer) {
                        case 0:
                            currentOffer = state.offerTurnStateData.offer_one;
                            break;
                        case 1:
                            currentOffer = state.offerTurnStateData.offer_two;
                            break;
                        case 2:
                            currentOffer = state.offerTurnStateData.offer_three;
                            break;
                    }

                    switch (currentOffer.type) {
                        case OfferType.BLAST:
                            addBlast(nk, logger, state.player1_id, currentOffer.blast!);
                            break;
                        case OfferType.ITEM:
                            addItem(nk, logger, state.player1_id, currentOffer.item!);
                            break;
                        case OfferType.COINS:
                            updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, currentOffer.coinsAmount);

                            if (getMetadataStat(nk, state.player1_id, "wildBattleButtonAds")) updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, currentOffer.coinsAmount / 2)
                            break;
                        case OfferType.GEMS:
                            updateWalletWithCurrency(nk, state.player1_id, Currency.Gems, currentOffer.gemsAmount);

                            if (getMetadataStat(nk, state.player1_id, "wildBattleButtonAds")) updateWalletWithCurrency(nk, state.player1_id, Currency.Gems, currentOffer.gemsAmount / 2)
                            break;
                        case OfferType.NONE:
                            break;
                    }

                    var newBlast = GetNewWildBlast(state, nk, logger);

                    state.wild_blast = ConvertBlastToBlastEntity(newBlast);

                    const newWildBlast: NewBlastData = {
                        id: state.wild_blast.data_id,
                        exp: state.wild_blast.exp,
                        iv: state.wild_blast.iv,
                        boss: state.wild_blast.boss,
                        shiny: state.wild_blast.shiny,
                        activeMoveset: state.wild_blast.activeMoveset,
                        status: Status.None
                    }

                    dispatcher.broadcastMessage(OpCodes.NEW_BLAST, JSON.stringify(newWildBlast));
                }

                state.battle_state = BattleState.WAITING;

                logger.debug('______________ END PLAYER CHOOSE OFFER ______________');
                logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp, state.p1_blasts[state.p1_index]?.mana);
            });
            break;
        // region END BATTLE
        case BattleState.END:

            const playerBlast = state.p1_blasts[state.p1_index]!;
            const wildBlast = state.wild_blast!;
            const allPlayerBlastFainted = isAllBlastDead(state.p1_blasts);

            const wildAlive = isBlastAlive(wildBlast);

            if (wildAlive == false || state.turnStateData.catched) {

                state.index_progression++;

                addExpOnBlastInGame(nk, logger, state.player1_id, playerBlast, wildBlast);

                if (state.turnStateData.catched) {
                    state.blast_catched++;

                    incrementQuest(state.player1_id, QuestIds.CATCH_BLAST, 1, nk, logger);

                    addBlast(nk, logger, state.player1_id, state.wild_blast!);
                }
                else {
                    state.blast_defeated++;

                    incrementQuest(state.player1_id, QuestIds.DEFEAT_BLAST, 1, nk, logger);
                }

                if (state.index_progression % 5 == 0 && state.index_progression % 10 != 0) {
                    let items: OfferTurnStateData = {
                        offer_one: getRandomOffer(nk, state, logger),
                        offer_two: getRandomOffer(nk, state, logger),
                        offer_three: getRandomOffer(nk, state, logger),
                    }

                    state.offerTurnStateData = items;

                    dispatcher.broadcastMessage(OpCodes.NEW_OFFER_TURN, JSON.stringify(items));

                    state.battle_state = BattleState.WAITFORPLAYERCHOOSEOFFER;

                } else {

                    var newBlast = GetNewWildBlast(state, nk, logger);


                    if (state.index_progression % 10 == 0) {
                        newBlast.exp = calculateExperienceFromLevel(state.index_progression / 2);
                        newBlast.iv = MaxIV;
                    };

                    state.wild_blast = ConvertBlastToBlastEntity(newBlast);

                    const newWildBlast: NewBlastData = {
                        id: state.wild_blast.data_id,
                        exp: state.wild_blast.exp,
                        iv: state.wild_blast.iv,
                        boss: state.wild_blast.boss,
                        shiny: state.wild_blast.shiny,
                        activeMoveset: state.wild_blast.activeMoveset,
                        status: Status.None
                    }

                    dispatcher.broadcastMessage(OpCodes.NEW_BLAST, JSON.stringify(newWildBlast));

                    state.battle_state = BattleState.WAITING;
                }

                EndLoopDebug(logger, state);

            }
            else if (allPlayerBlastFainted) {
                dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(false));
                PlayerLeave(nk, state, logger);

                return null;
            }

            logger.debug('______________ END BATTLE ______________');

            break;
    }


    if (connectedPlayers(state) === 0) {
        logger.debug('Running empty ticks: %d', state.emptyTicks);
        state.emptyTicks++;
    }

    if (state.emptyTicks > 100) {
        return null;
    }

    return {
        state
    };
}

const matchSignal = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, data: string): { state: WildBattleData, data?: string } | null {
    logger.debug('Lobby match signal received: ' + data);

    return {
        state,
        data: "Lobby match signal received: " + data
    };
}

const matchTerminate = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, graceSeconds: number): { state: WildBattleData } | null {
    logger.debug('Lobby match terminated');

    return {
        state
    };
}


function PlayerLeave(nk: nkruntime.Nakama, state: WildBattleData, logger: nkruntime.Logger) {
    let bonusAds = getMetadataStat(nk, state.player1_id, "wildBattleButtonAds");

    if (state.blast_catched > 0) {
        incrementMetadataStat(nk, state.player1_id, "blast_catched", state.blast_catched);
    }

    if (state.blast_defeated > 0) {

        incrementMetadataStat(nk, state.player1_id, "blast_defeated", state.blast_defeated);
        writeIncrementalRecordLeaderboard(nk, logger, state.player1_id, LeaderboardTotalBlastDefeatedId, state.blast_defeated);
        writeIncrementalRecordLeaderboard(nk, logger, state.player1_id, LeaderboardBlastDefeatedAreaId + getMetadataStat(nk, state.player1_id, "area"), state.blast_defeated);
    }

    if (state.index_progression > 1) {
        let totalCoins = 200 * (state.index_progression - 1);

        updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, totalCoins);

        if (bonusAds) updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, totalCoins / 2);

        writeBestRecordLeaderboard(nk, logger, state.player1_id, LeaderboardBestStageAreaId + getMetadataStat(nk, state.player1_id, "area"), state.index_progression - 1);
    }

    if (bonusAds) {
        setMetadataStat(nk, state.player1_id, "wildBattleButtonAds", false);
    }
}

function GetNewWildBlast(state: WildBattleData, nk: nkruntime.Nakama, logger: nkruntime.Logger): Blast {
    return getRandomBlastWithAreaId(state.player1_id, nk, Math.floor(state.index_progression / 5), state.index_progression % 10 == 0, logger);
}

function EndLoopDebug(logger: nkruntime.Logger, state: WildBattleData) {
    logger.debug('______________ END LOOP BATTLE ______________');
    logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);
    logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp, state.p1_blasts[state.p1_index]?.mana);
    logger.debug('Wild blast turn type : %h', state.turnStateData.wb_turn_type);
}

function ErrorFunc(state: WildBattleData, error: string, dispatcher: nkruntime.MatchDispatcher, currentBattleState: BattleState) {
    state.battle_state = currentBattleState;
    state.player1_state = PlayerState.READY;

    dispatcher.broadcastMessage(OpCodes.ERROR_SERV, JSON.stringify(error));

    return { state };
}

function connectedPlayers(s: WildBattleData): number {
    let count = 0;
    for (const p of Object.keys(s.presences)) {
        if (s.presences[p] !== null) {
            count++;
        }
    }
    return count;
}

//#region  Attack Logic
function applyBlastAttack(attacker: BlastEntity, defender: BlastEntity, move: Move, meteo: Meteo): number {
    let damage = calculateDamage(
        calculateLevelFromExperience(attacker.exp),
        attacker.attack * getModifierMultiplier(Stats.Attack, attacker.modifiers),
        defender.defense * getModifierMultiplier(Stats.Defense, defender.modifiers),
        move.type,
        getBlastDataById(defender.data_id!).type,
        move.power,
        meteo,
    );

    defender.hp = clamp(defender.hp - damage, 0, Number.POSITIVE_INFINITY);

    return damage;
}

interface AttackContext {
    move: Move;
    attacker: BlastEntity;
    defender: BlastEntity;
    attackerPlatforms: Type[];
    setAttacker: (b: BlastEntity) => void;
    setDefender: (b: BlastEntity) => void;
    getTurnStateData: () => TurnStateData;
    setMoveDamage: (dmg: number) => void;
    setMoveEffect: (effects: MoveEffectData[]) => void;
    meteo: Meteo;
    dispatcher: nkruntime.MatchDispatcher;
    isPlayer: boolean;
}

function executeAttack(ctx: AttackContext): void {
    // Gestion mana/plateforme
    switch (ctx.move.attackType) {
        case AttackType.Normal:
        case AttackType.Status:
            if (ctx.attacker.mana < ctx.move.cost) return;
            ctx.attacker.mana = clamp(ctx.attacker.mana - ctx.move.cost, 0, Number.POSITIVE_INFINITY);
            ctx.attackerPlatforms = addPlatformType(ctx.attackerPlatforms, ctx.move.type);
            if (calculateWeatherModifier(ctx.meteo, ctx.move.type) > 1) {
                ctx.attackerPlatforms = addPlatformType(ctx.attackerPlatforms, ctx.move.type);
            }
            break;
        case AttackType.Special: {
            const platformCount = getAmountOfPlatformTypeByType(ctx.attackerPlatforms, ctx.move.type);
            if (platformCount < ctx.move.cost) return;
            ctx.attackerPlatforms = removePlatformTypeByType(ctx.attackerPlatforms, ctx.move.type, ctx.move.cost);
            break;
        }
    }

    // Application des effets
    const effects = applyMoveEffects(
        ctx.move,
        () => ctx.defender,
        ctx.setDefender,
        () => ctx.attacker,
        ctx.setAttacker
    );
    ctx.setMoveEffect(effects);

    // Application des dégâts
    if (ctx.move.target === Target.Opponent) {
        const damage = applyBlastAttack(
            ctx.attacker,
            ctx.defender,
            ctx.move,
            ctx.meteo
        );
        ctx.setMoveDamage(damage);
    }
} function executePlayerAttack(
    state: WildBattleData,
    move: Move,
    dispatcher: nkruntime.MatchDispatcher
): { state: WildBattleData } {
    const playerIndex = state.p1_index;
    executeAttack({
        move,
        attacker: state.p1_blasts[playerIndex]!,
        defender: state.wild_blast!,
        attackerPlatforms: state.player1_platform,
        setAttacker: b => state.p1_blasts[playerIndex] = b,
        setDefender: b => state.wild_blast = b,
        getTurnStateData: () => state.turnStateData,
        setMoveDamage: dmg => state.turnStateData.p_move_damage = dmg,
        setMoveEffect: eff => state.turnStateData.p_move_effects = eff,
        meteo: state.meteo,
        dispatcher,
        isPlayer: true,
    });
    return { state };
}


function applyMoveEffects(
    move: Move,
    getTargetBlast: () => BlastEntity,
    setTargetBlast: (blast: BlastEntity) => void,
    getAttacker: () => BlastEntity,
    setAttacker: (blast: BlastEntity) => void
): MoveEffectData[] {
    if (!move.effects || move.effects.length === 0) return [];

    const effectsThisTurn: MoveEffectData[] = [];

    for (const effectData of move.effects) {
        if (effectData.effect === MoveEffect.None) continue;

        let getTarget: () => BlastEntity;
        let setTarget: (b: BlastEntity) => void;

        if (effectData.effectTarget === Target.Self) {
            getTarget = getAttacker;
            setTarget = setAttacker;
        } else {
            getTarget = getTargetBlast;
            setTarget = setTargetBlast;
        }

        if (move.attackType === AttackType.Special || move.attackType === AttackType.Status) {
            const updated = applyEffect(getTarget(), move, effectData);
            setTarget(updated);
            effectsThisTurn.push(effectData);
        } else {
            const result = calculateEffectWithProbability(getTarget(), move, effectData);
            setTarget(result.blast);
            effectsThisTurn.push(result.moveEffect);
        }
    }

    return effectsThisTurn;
}

function executeWildBlastAttack(
    state: WildBattleData,
    dispatcher: nkruntime.MatchDispatcher,
    logger: nkruntime.Logger
): { state: WildBattleData } {

    const moveIndex = state.turnStateData.wb_move_index;
    if (moveIndex === -1) {
        state.wild_blast!.mana = calculateManaRecovery(
            state.wild_blast!.maxMana,
            state.wild_blast!.mana,
            true
        );
        state.turnStateData.wb_turn_type = TurnType.WAIT;
        return { state };
    }

    const move = getMoveById(state.wild_blast!.activeMoveset![moveIndex]);

    executeAttack({
        move,
        attacker: state.wild_blast!,
        defender: state.p1_blasts[state.p1_index]!,
        attackerPlatforms: state.wild_blast_platform,
        setAttacker: b => state.wild_blast = b,
        setDefender: b => state.p1_blasts[state.p1_index] = b,
        getTurnStateData: () => state.turnStateData,
        setMoveDamage: dmg => state.turnStateData.wb_move_damage = dmg,
        setMoveEffect: eff => state.turnStateData.wb_move_effects = eff,
        meteo: state.meteo,
        dispatcher,
        isPlayer: false,
    });
    state.turnStateData.wb_turn_type = TurnType.ATTACK;
    return { state };
}

function handleAttackTurn(isPlayerFaster: boolean, state: WildBattleData, move: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): { state: WildBattleData } {
    ({ state } = isPlayerFaster ? executePlayerAttack(state, move, dispatcher) : executeWildBlastAttack(state, dispatcher, logger));

    state = checkIfMatchContinue(state);

    return { state };
}

function performAttackSequence(state: WildBattleData, playerMove: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): WildBattleData {

    let firstIsPlayer: boolean;

    if (state.turnStateData.wb_move_index >= 0) {
        const wildMoveId = state.wild_blast!.activeMoveset![state.turnStateData.wb_move_index];
        const wildMove = getMoveById(wildMoveId);

        if (playerMove.priority == wildMove.priority) {
            firstIsPlayer = getFasterBlast(state.p1_blasts[state.p1_index]!, state.wild_blast!);
        } else {
            firstIsPlayer = playerMove.priority > wildMove.priority;
        }

    } else {
        firstIsPlayer = true;
    }

    ({ state } = handleAttackTurn(firstIsPlayer, state, playerMove, dispatcher, nk, logger));

    if (state.battle_state === BattleState.READY) {
        ({ state } = handleAttackTurn(!firstIsPlayer, state, playerMove, dispatcher, nk, logger));
    }

    return state;
}

function checkIfMatchContinue(state: WildBattleData): WildBattleData {

    const playerBlast = state.p1_blasts[state.p1_index]!;
    const wildBlast = state.wild_blast!;

    const wildAlive = isBlastAlive(wildBlast);
    const playerAlive = isBlastAlive(playerBlast);
    const allPlayerDead = isAllBlastDead(state.p1_blasts);

    if (!wildAlive) {
        state.battle_state = BattleState.END;
    } else if (allPlayerDead) {
        state.battle_state = BattleState.END;
    } else if (!playerAlive) {
        state.battle_state = BattleState.WAITFORPLAYERSWAP;
    }

    return state;
}


//#endregion

// region Offer Turn Logic
function getRandomOffer(nk: nkruntime.Nakama, state: WildBattleData, logger: nkruntime.Logger): Offer {
    let offer: Offer = {
        type: OfferType.ITEM,
        coinsAmount: 0,
        gemsAmount: 0,
        blast: null,
        item: null,
    };

    const random = Math.floor(Math.random() * 4);
    switch (random) {
        case 0:
            offer.type = OfferType.BLAST;
            var newBlast = GetNewWildBlast(state, nk, logger);
            offer.blast = newBlast;
            break;
        case 1:
            offer.type = OfferType.ITEM;
            offer.item = getRandomItem(5);
            break;
        case 2:
            offer.type = OfferType.COINS;
            offer.coinsAmount = Math.floor(Math.random() * 1000) + 1;
            break;
        case 3:
            offer.type = OfferType.GEMS;
            offer.gemsAmount = Math.floor(Math.random() * 10) + 1;
            break;
    }

    return offer;
}

