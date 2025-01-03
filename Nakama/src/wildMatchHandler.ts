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

    player1_current_blast: Blast | null;
    player1_blasts: Blast[];
    player1_items: Item[];

    wild_blast: Blast | null;

    TurnStateData: TurnStateData;
}

interface StartStateData {
    id: number;
    exp: number;
    iv: number;
    status: Status;
    activeMoveset: number[];
}

interface TurnStateData {
    p_move_damage: number;
    p_move_status: Status;

    wb_turn_type: TurnType;
    wb_move_index: number;
    wb_move_damage: number;
    wb_move_status: Status;

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

        player1_current_blast: null,
        player1_blasts: [],
        player1_items: [],
        wild_blast: null,

        TurnStateData: {
            p_move_damage: 0,
            p_move_status: Status.NONE,
            wb_turn_type: TurnType.NONE,
            wb_move_index: 0,
            wb_move_damage: 0,
            wb_move_status: Status.NONE,
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
                    p_move_status: Status.NONE,

                    wb_move_index: getRandomNumber(0, state.wild_blast!.activeMoveset!.length - 1),
                    wb_move_damage: 0,
                    wb_move_status: Status.NONE,

                    wb_turn_type: TurnType.NONE,

                    catched: false
                }

                switch (message.opCode) {
                    case OpCodes.PLAYER_ATTACK:
                        let attackIndex = clamp(JSON.parse(nk.binaryToString(message.data)), 0, 3);
                        let move = getMoveById(state.player1_current_blast!.activeMoveset![attackIndex]);

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
                            case ItemBehaviour.HEAL:
                                state.player1_blasts[msgItem.index_blast] = healHealthBlast(state.player1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ItemBehaviour.MANA:
                                state.player1_blasts[msgItem.index_blast] = healManaBlast(state.player1_blasts[msgItem.index_blast], itemData.gain_amount);
                                break;
                            case ItemBehaviour.STATUS:
                                break;
                            case ItemBehaviour.CATCH:
                                var wildBlastCaptured = false;

                                wildBlastCaptured = isBlastCaptured(state.wild_blast!.hp, state.wild_blast!.maxHp, getBlastDataById(state.wild_blast!.data_id).catchRate, itemData.catchRate, 1)

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
                       
                        break;
                    case OpCodes.PLAYER_WAIT:
                      
                        break;
                }

                logger.debug('______________ END LOOP BATTLE ______________');

            });
            break;
        case BattleState.WAITFORPLAYERSWAP:

            messages.forEach(function (message) {

                logger.debug('______________ PLAYER SWAP BLAST ______________');

                logger.debug('______________ END PLAYER SWAP BLAST ______________');
            });
            break;
        case BattleState.END:

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
function applyBlastAttack(attacker: Blast, defender: Blast, move: Move, state: WildBattleData): number {
    let damage = calculateDamage(
        calculateLevelFromExperience(attacker.exp),
        attacker.attack,
        defender.defense,
        move.type,
        getBlastDataById(defender.data_id!).type,
        move.power
    );

    defender.hp = clamp(defender.hp - damage, 0, Number.POSITIVE_INFINITY);
    attacker.mana = clamp(attacker.mana - move.cost, 0, Number.POSITIVE_INFINITY);

    return damage;
}

function executePlayerAttack(state: WildBattleData, move: Move, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    if (state.player1_current_blast!.mana < move.cost) {
        return { state };
    }

    const damage = applyBlastAttack(state.player1_current_blast!, state.wild_blast!, move, state);
    state.TurnStateData.p_move_damage = damage;
    state.TurnStateData.p_move_status = Status.NONE;

    return { state };
}

function executeWildBlastAttack(state: WildBattleData, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    let wb_move = getMoveById(state.wild_blast!.activeMoveset![state.TurnStateData.wb_move_index]);

    if (state.wild_blast!.mana < wb_move.cost) {
        state.TurnStateData.wb_move_index = -1;
    }

    if (state.TurnStateData.wb_move_index < 0) {
        state.wild_blast!.mana = calculateStaminaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, true);
        state.TurnStateData.wb_turn_type = TurnType.WAIT;
    } else {
        const damage = applyBlastAttack(state.wild_blast!, state.player1_current_blast!, wb_move, state);

        state.TurnStateData.wb_move_damage = damage;
        state.TurnStateData.wb_move_status = Status.NONE;
        state.TurnStateData.wb_turn_type = TurnType.ATTACK;
    }

    return { state };
}

function handleAttackTurn(isPlayerFaster: boolean, state: WildBattleData, move: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): { state: WildBattleData } {
    ({ state } = isPlayerFaster ? executePlayerAttack(state, move, dispatcher) : executeWildBlastAttack(state, dispatcher));

    ({ state } = checkIfMatchContinue(nk, logger, state, dispatcher));

    return { state };
}

function performAttackSequence(state: WildBattleData, playerMove: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): WildBattleData {

    ({ state } = handleAttackTurn(getFasterBlast(state.player1_current_blast!, state.wild_blast!), state, playerMove, dispatcher, nk, logger));

    if (state.battle_state == BattleState.READY) ({ state } = handleAttackTurn(!getFasterBlast(state.player1_current_blast!, state.wild_blast!), state, playerMove, dispatcher, nk, logger));

    return state;
}

function checkIfMatchContinue(nk: nkruntime.Nakama, logger: nkruntime.Logger, state: WildBattleData, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    if (!isBlastAlive(state.wild_blast!)) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(true));

        addExpOnBlastInGame(nk, logger, state.player1_id, state.player1_current_blast!, state.wild_blast!)

        state.battle_state = BattleState.END;
    }

    if (isAllBlastDead(state.player1_blasts)) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(false));

        state.battle_state = BattleState.END;

    }
    else if (!isBlastAlive(state.player1_current_blast!)) {
        state.battle_state = BattleState.WAITFORPLAYERSWAP;
    }

    return { state };
}

//#endregion