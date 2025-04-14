enum BattleState {
    NONE,
    WAITING,
    READY,
    START,
    WAITFORPLAYERSWAP,
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


    meteo: Meteo

    TurnStateData: TurnStateData;
}

interface StartStateData {
    id: number;
    exp: number;
    iv: number;
    status: Status;
    activeMoveset: number[];
    meteo: Meteo;
}

interface TurnStateData {
    p_move_damage: number;
    p_move_effect: MoveEffect;

    wb_turn_type: TurnType;
    wb_move_index: number;
    wb_move_damage: number;
    wb_move_effect: MoveEffect;

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

        meteo: Meteo.None,

        TurnStateData: {
            p_move_damage: 0,
            p_move_effect: MoveEffect.None,

            wb_turn_type: TurnType.NONE,
            wb_move_index: 0,
            wb_move_damage: 0,
            wb_move_effect: MoveEffect.None,

            catched: false
        },
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

const matchLeave = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, presences: nkruntime.Presence[]): { state: WildBattleData } | null {
    for (let presence of presences) {
        logger.info("Player: %s left match: %s.", presence.userId, ctx.matchId);
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



const matchLoop = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, messages: nkruntime.MatchMessage[]): { state: WildBattleData } | null {
    // logger.info('Current state : %d', state.battle_state);

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


            var newBlast = getRandomBlastEntityInAllPlayerArea(state.player1_id, nk, logger);
            state.wild_blast = ConvertBlastToBlastEntity(newBlast);

            state.meteo = getRandomMeteo();

            const StartData: StartStateData = {
                id: state.wild_blast.data_id,
                exp: state.wild_blast.exp,
                iv: state.wild_blast.iv,
                status: Status.None,
                activeMoveset: state.wild_blast.activeMoveset,
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

                // faire que si c'est un autre OP code que ceux d'en dessous faire un errorfunc

                logger.debug('______________ LOOP BATTLE ______________');

                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));

                state.player1_state = PlayerState.BUSY;

                state.TurnStateData = {
                    p_move_damage: 0,
                    p_move_effect: MoveEffect.None,

                    wb_move_index: getRandomUsableMove(getMovesByIds(state.wild_blast!.activeMoveset!), state.wild_blast!.mana, state.wild_blast_platform),
                    wb_move_damage: 0,
                    wb_move_effect: MoveEffect.None,

                    wb_turn_type: TurnType.NONE,

                    catched: false
                }

                switch (message.opCode) {
                    case OpCodes.PLAYER_ATTACK:
                        let attackIndex = clamp(JSON.parse(nk.binaryToString(message.data)), 0, 3);
                        let move = getMoveById(state.p1_blasts[state.p1_index]!.activeMoveset![attackIndex]);

                        if (move == null) {
                            ({ state } = ErrorFunc(state, "Player 1 move null", dispatcher));
                            return;
                        }

                        state = performAttackSequence(state, move, dispatcher, nk, logger);
                        break;
                    case OpCodes.PLAYER_USE_ITEM:
                        let msgItem = {} as ItemUseJSON;

                        msgItem = JSON.parse(nk.binaryToString(message.data));

                        msgItem.index_item = clamp(msgItem.index_item, 0, state.player1_items.length - 1)

                        let item = state.player1_items[msgItem.index_item];

                        useItem(nk, logger, state.player1_id, item);

                        if (item == null) {
                            ({ state } = ErrorFunc(state, "Item to use is null", dispatcher));
                            return;
                        }

                        if (item.amount <= 0) {
                            ({ state } = ErrorFunc(state, "U don't have enough item", dispatcher));
                            return;
                        }

                        var itemData = getItemDataById(item.data_id);

                        switch (itemData.behaviour) {
                            case ITEM_BEHAVIOUR.HEAL:
                                state.p1_blasts[msgItem.index_blast] = healHealthBlast(state.p1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ITEM_BEHAVIOUR.MANA:
                                state.p1_blasts[msgItem.index_blast] = healManaBlast(state.p1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ITEM_BEHAVIOUR.STATUS:
                                break;
                            case ITEM_BEHAVIOUR.CATCH:
                                var wildBlastCaptured = false;

                                wildBlastCaptured = isBlastCaptured(state.wild_blast!.hp, state.wild_blast!.maxHp, getBlastDataById(state.wild_blast!.data_id).catchRate, itemData.catchRate!, 1)

                                if (wildBlastCaptured) {
                                    logger.debug('Wild blast Captured !', wildBlastCaptured);

                                    addBlast(nk, logger, state.player1_id, state.wild_blast!);

                                    state.battle_state = BattleState.END;

                                }

                                state.TurnStateData.catched = wildBlastCaptured;
                                break;
                            default:
                        }

                        ({ state } = executeWildBlastAttack(state, dispatcher));


                        break;
                    case OpCodes.PLAYER_CHANGE_BLAST:
                        var msgChangeBlast = clamp(JSON.parse(nk.binaryToString(message.data)), 0, state.p1_blasts.length - 1);

                        if (state.p1_index == msgChangeBlast) {
                            ErrorFunc(state, "Cannot change actual blast with actual blast", dispatcher);
                            return;
                        }

                        if (!isBlastAlive(state.p1_blasts[msgChangeBlast])) {
                            ({ state } = ErrorFunc(state, "Cannot change actual blast with dead blast in Ready", dispatcher));
                            return;
                        }

                        state.p1_index = msgChangeBlast;

                        ({ state } = executeWildBlastAttack(state, dispatcher));
                        break;
                    case OpCodes.PLAYER_WAIT:
                        state.player1_state = PlayerState.BUSY;

                        state.p1_blasts[state.p1_index]!.mana = calculateStaminaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, true);

                        ({ state } = executeWildBlastAttack(state, dispatcher));
                        break;
                }

                ({ blast: state.p1_blasts[state.p1_index]!, otherBlast: state.wild_blast! } = applyStatusEffectAtEndOfTurn(state.p1_blasts[state.p1_index]!, state.wild_blast!));
                ({ blast: state.wild_blast!, otherBlast: state.p1_blasts[state.p1_index]! } = applyStatusEffectAtEndOfTurn(state.wild_blast!,state.p1_blasts[state.p1_index]!));

                ({ state } = checkIfMatchContinue(nk, logger, state, dispatcher));

                if (state.battle_state == BattleState.WAITFORPLAYERSWAP) {

                    if (state.TurnStateData.wb_turn_type != TurnType.WAIT) state.wild_blast!.mana = calculateStaminaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);

                    logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);

                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));

                    return;
                } else if (state.battle_state == BattleState.END) {
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));

                    return;
                }
                else {

                    state.battle_state = BattleState.WAITING;

                    if (message.opCode != OpCodes.PLAYER_WAIT) state.p1_blasts[state.p1_index]!.mana = calculateStaminaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, false);
                    if (state.TurnStateData.wb_turn_type != TurnType.WAIT) state.wild_blast!.mana = calculateStaminaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);

                    //Send matchTurn
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));


                    logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);
                    logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp, state.p1_blasts[state.p1_index]?.mana);
                }

                logger.debug('______________ END LOOP BATTLE ______________');

            });
            break;
        case BattleState.WAITFORPLAYERSWAP:

            messages.forEach(function (message) {

                logger.debug('______________ PLAYER SWAP BLAST ______________');

                message.data == null ? logger.debug('Receive Op code : %d', message.opCode) : logger.debug('Receive Op code : %d, with data : %e', message.opCode, JSON.parse(nk.binaryToString(message.data)));

                if (message.opCode == OpCodes.PLAYER_CHANGE_BLAST) {

                    state.player1_state = PlayerState.BUSY;

                    var msgChangeBlast = clamp(JSON.parse(nk.binaryToString(message.data)), 0, state.p1_blasts.length - 1);

                    if (state.p1_index == msgChangeBlast) {
                        ErrorFunc(state, "Cannot change actual blast with actual blast", dispatcher);
                        return;
                    }

                    if (!isBlastAlive(state.p1_blasts[msgChangeBlast])) {
                        ({ state } = ErrorFunc(state, "Cannot change actual blast with dead blast", dispatcher));
                        return;
                    }

                    state.p1_index = msgChangeBlast;
                }

                logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);
                logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp,state.p1_blasts[state.p1_index]?.mana);

                state.battle_state = BattleState.WAITING;

                logger.debug('______________ END PLAYER SWAP BLAST ______________');
            });
            break;
        case BattleState.END:

            updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, 200)

            if (state.TurnStateData.catched) incrementMetadataStat(nk, state.player1_id, "blast_catched", 1);
            else incrementMetadataStat(nk, state.player1_id, "blast_defeated", 1);

            logger.debug("OwnerID: %s", ctx.userId);
            logger.debug("Username: %s", ctx.username);

            writeRecordLeaderboard(nk, logger, state.player1_id, LeaderboardBlastDefeatedId, 1);

            state.battle_state = BattleState.START;
            state.player1_state = PlayerState.BUSY;

            logger.debug('______________ END BATTLE ______________');

            return null;
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


function ErrorFunc(state: WildBattleData, error: string, dispatcher: nkruntime.MatchDispatcher) {
    state.battle_state = BattleState.READY;
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


function applyBlastAttack(attacker: BlastEntity, defender: BlastEntity, move: Move, state: WildBattleData): number {
    let damage = calculateDamage(
        calculateLevelFromExperience(attacker.exp),
        attacker.attack * getModifierMultiplier(Stats.Attack, attacker.modifiers),
        defender.defense * getModifierMultiplier(Stats.Defense, defender.modifiers),
        move.type,
        getBlastDataById(defender.data_id!).type,
        move.power,
        state.meteo,
    );

    defender.hp = clamp(defender.hp - damage, 0, Number.POSITIVE_INFINITY);
    attacker.mana = clamp(attacker.mana - move.cost, 0, Number.POSITIVE_INFINITY);

    return damage;
}

function executePlayerAttack(state: WildBattleData, move: Move, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    if (move.platform_cost > 0) {
        let amountType = getAmountOfPlatformTypeByType(state.player1_platform, move.type);

        if (amountType < move.platform_cost) {
            ({ state } = ErrorFunc(state, "Player not enough platform type", dispatcher));
            return { state };
        }

        if (move.effect != MoveEffect.None) state.wild_blast! = applyEffect(state.wild_blast!, move);

        state.TurnStateData.p_move_effect = move.effect;
        state.player1_platform = removePlatformTypeByType(state.player1_platform, move.type, move.platform_cost);
    } else {

        if (state.p1_blasts[state.p1_index]!.mana < move.cost) {
            return { state };
        }

        if (move.effect != MoveEffect.None) {
            const result = calculateEffectWithProbability(state.wild_blast!, move);
            state.wild_blast = result.blast;
            state.TurnStateData.p_move_effect = result.moveEffect;
        }

        state.player1_platform = addPlatformType(state.player1_platform, move.type);
        if (calculateWeatherModifier(state.meteo, move.type) > 1) state.player1_platform = addPlatformType(state.player1_platform, move.type);

    }

    const damage = applyBlastAttack(state.p1_blasts[state.p1_index]!, state.wild_blast!, move, state);
    state.TurnStateData.p_move_damage = damage;

    return { state };
}

function executeWildBlastAttack(state: WildBattleData, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    if (state.TurnStateData.wb_move_index == -1) {

        state.wild_blast!.mana = calculateStaminaRecovery(state.wild_blast!.mana, state.wild_blast!.mana, true);
        state.TurnStateData.wb_turn_type = TurnType.WAIT;

        return { state };
    }

    let wb_move = getMoveById(state.wild_blast!.activeMoveset![state.TurnStateData.wb_move_index]);

    if (wb_move.platform_cost > 0) {
        let amountType = getAmountOfPlatformTypeByType(state.wild_blast_platform, wb_move.type);

        if (amountType < wb_move.platform_cost) {
            ({ state } = ErrorFunc(state, "Wild blast not enough platform type", dispatcher));
            return { state };
        }

        if (wb_move.effect != MoveEffect.None) state.p1_blasts[state.p1_index] = applyEffect(state.p1_blasts[state.p1_index]!, wb_move);

        state.TurnStateData.wb_move_effect = wb_move.effect;
        state.wild_blast_platform = removePlatformTypeByType(state.wild_blast_platform, wb_move.type, wb_move.platform_cost);

    } else {

        if (wb_move.effect != MoveEffect.None) {
            const result = calculateEffectWithProbability(state.p1_blasts[state.p1_index]!, wb_move);
            state.p1_blasts[state.p1_index] = result.blast;
            state.TurnStateData.wb_move_effect = result.moveEffect;
        }


        state.wild_blast_platform = addPlatformType(state.wild_blast_platform, wb_move.type);
        if (calculateWeatherModifier(state.meteo, wb_move.type) > 1) state.wild_blast_platform = addPlatformType(state.wild_blast_platform, wb_move.type);
    }

    const damage = applyBlastAttack(state.wild_blast!, state.p1_blasts[state.p1_index]!, wb_move, state);

    state.TurnStateData.wb_move_damage = damage;
    state.TurnStateData.wb_turn_type = TurnType.ATTACK;

    return { state };
}

function handleAttackTurn(isPlayerFaster: boolean, state: WildBattleData, move: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): { state: WildBattleData } {
    ({ state } = isPlayerFaster ? executePlayerAttack(state, move, dispatcher) : executeWildBlastAttack(state, dispatcher));

    ({ state } = checkIfMatchContinue(nk, logger, state, dispatcher));

    return { state };
}

function performAttackSequence(state: WildBattleData, playerMove: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): WildBattleData {

    if (state.TurnStateData.wb_move_index >= 0) {

        var wb_move_priority = getMoveById(state.wild_blast!.activeMoveset![state.TurnStateData.wb_move_index]).priority;

        if (playerMove.priority > wb_move_priority) {
            ({ state } = handleAttackTurn(true, state, playerMove, dispatcher, nk, logger));

            if (state.battle_state == BattleState.READY) ({ state } = handleAttackTurn(false, state, playerMove, dispatcher, nk, logger));

            return state;

        } else if (playerMove.priority < wb_move_priority) {
            ({ state } = handleAttackTurn(false, state, playerMove, dispatcher, nk, logger));

            if (state.battle_state == BattleState.READY) ({ state } = handleAttackTurn(true, state, playerMove, dispatcher, nk, logger));

            return state;
        }
    }

    ({ state } = handleAttackTurn(getFasterBlast(state.p1_blasts[state.p1_index]!, state.wild_blast!), state, playerMove, dispatcher, nk, logger));

    if (state.battle_state == BattleState.READY) ({ state } = handleAttackTurn(!getFasterBlast(state.p1_blasts[state.p1_index]!, state.wild_blast!), state, playerMove, dispatcher, nk, logger));

    return state;
}

function checkIfMatchContinue(nk: nkruntime.Nakama, logger: nkruntime.Logger, state: WildBattleData, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    if (!isBlastAlive(state.wild_blast!)) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(true));

        addExpOnBlastInGame(nk, logger, state.player1_id, state.p1_blasts[state.p1_index]!, state.wild_blast!)

        state.battle_state = BattleState.END;
    }

    if (isAllBlastDead(state.p1_blasts)) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(false));

        state.battle_state = BattleState.END;

    }
    else if (!isBlastAlive(state.p1_blasts[state.p1_index]!)) {
        state.battle_state = BattleState.WAITFORPLAYERSWAP;
    }

    return { state };
}

//#endregion