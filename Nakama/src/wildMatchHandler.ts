// region Setup 

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

// region MatchHandler 

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


// region MatchLoop 

const matchLoop = function (ctx: nkruntime.Context, logger: nkruntime.Logger, nk: nkruntime.Nakama, dispatcher: nkruntime.MatchDispatcher, tick: number, state: WildBattleData, messages: nkruntime.MatchMessage[]): { state: WildBattleData } | null {

    switch (state.battle_state) {
        case BattleState.START:

            logger.debug('1 ______________ START BATTLE ______________');

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

            logger.debug('1 ______________ END START BATTLE ______________');

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
                    // region Attack
                    case OpCodes.PLAYER_ATTACK:
                        let attackIndex = clamp(JSON.parse(nk.binaryToString(message.data)), 0, 3);
                        let move = getMoveById(state.p1_blasts[state.p1_index]!.activeMoveset![attackIndex]);

                        if (move == null) {
                            ({ state } = ErrorFunc(state, "Player 1 move null", dispatcher));
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
                            ({ state } = ErrorFunc(state, "Item to use is null", dispatcher));
                            return;
                        }

                        if (item.amount <= 0) {
                            ({ state } = ErrorFunc(state, "U don't have enough item", dispatcher));
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

                                    addBlast(nk, logger, state.player1_id, state.wild_blast!);

                                    state.battle_state = BattleState.END;

                                }

                                state.TurnStateData.catched = wildBlastCaptured;
                                break;
                            default:
                        }

                        ({ state } = executeWildBlastAttack(state, dispatcher,logger));
                        break;
                    // region Player Change
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

                        ({ state } = executeWildBlastAttack(state, dispatcher,logger));
                        break;
                    // region Player Wait
                    case OpCodes.PLAYER_WAIT:
                        state.player1_state = PlayerState.BUSY;

                        state.p1_blasts[state.p1_index]!.mana = calculateManaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, true);

                        ({ state } = executeWildBlastAttack(state, dispatcher,logger));
                        break;
                }

                // region End turn Logic

                ({ blast: state.p1_blasts[state.p1_index]!, otherBlast: state.wild_blast! } = applyStatusEffectAtEndOfTurn(state.p1_blasts[state.p1_index]!, state.wild_blast!));
                ({ blast: state.wild_blast!, otherBlast: state.p1_blasts[state.p1_index]! } = applyStatusEffectAtEndOfTurn(state.wild_blast!, state.p1_blasts[state.p1_index]!));

                ({ state } = checkIfMatchContinue(nk, logger, state, dispatcher));

                if (state.battle_state == BattleState.END) {
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));
                    return;
                }
                else if (state.battle_state == BattleState.WAITFORPLAYERSWAP) {
                    state.wild_blast!.mana = calculateManaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);

                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));
                    return;
                }
                else {
                    state.battle_state = BattleState.WAITING;

                    state.p1_blasts[state.p1_index]!.mana = calculateManaRecovery(state.p1_blasts[state.p1_index]!.maxMana, state.p1_blasts[state.p1_index]!.mana, false);
                    state.wild_blast!.mana = calculateManaRecovery(state.wild_blast!.maxMana, state.wild_blast!.mana, false);

                    //Send matchTurn
                    dispatcher.broadcastMessage(OpCodes.MATCH_ROUND, JSON.stringify(state.TurnStateData));
                }

                logger.debug('______________ END LOOP BATTLE ______________');
                logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);
                logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp, state.p1_blasts[state.p1_index]?.mana);
                logger.debug('Wild blast turn type : %h', state.TurnStateData.wb_turn_type);

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

                state.battle_state = BattleState.WAITING;

                logger.debug('______________ END PLAYER SWAP BLAST ______________');
                logger.debug('Wild blast HP : %h, Mana : %m', state.wild_blast?.hp, state.wild_blast?.mana);
                logger.debug('Player blast HP : %h, Mana : %m', state.p1_blasts[state.p1_index]?.hp, state.p1_blasts[state.p1_index]?.mana);
            });
            break;
        case BattleState.END:

            updateWalletWithCurrency(nk, state.player1_id, Currency.Coins, 200)

            if (state.TurnStateData.catched) incrementMetadataStat(nk, state.player1_id, "blast_catched", 1);
            else incrementMetadataStat(nk, state.player1_id, "blast_defeated", 1);

            writeRecordLeaderboard(nk, logger, state.player1_id, LeaderboardBlastDefeatedId, 1);

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

function executePlayerAttack(
    state: WildBattleData,
    move: Move,
    dispatcher: nkruntime.MatchDispatcher
): { state: WildBattleData } {

    const playerIndex = state.p1_index;

    const getTargetBlast = (): BlastEntity => {
        return move.target === Target.Opponent
            ? state.wild_blast!
            : state.p1_blasts[playerIndex]!;
    };

    const setTargetBlast = (blast: BlastEntity) => {
        if (move.target === Target.Opponent) {
            state.wild_blast = blast;
        } else {
            state.p1_blasts[playerIndex] = blast;
        }
    };

    const applyEffectIfNeeded = () => {
        if (move.effect === MoveEffect.None) return;

        if (move.attackType === AttackType.Special) {
            const updated = applyEffect(getTargetBlast(), move);
            setTargetBlast(updated);
            state.TurnStateData.p_move_effect = move.effect;
        } else {
            const result = calculateEffectWithProbability(getTargetBlast(), move);
            setTargetBlast(result.blast);
            state.TurnStateData.p_move_effect = result.moveEffect;
        }
    };

    const hasEnoughMana = () => state.p1_blasts[playerIndex]!.mana >= move.cost;

    const reduceMana = () => {
        state.p1_blasts[playerIndex]!.mana = clamp(
            state.p1_blasts[playerIndex]!.mana - move.cost,
            0,
            Number.POSITIVE_INFINITY
        );
    };

    const handlePlatformBoost = () => {
        const boosted = calculateWeatherModifier(state.meteo, move.type) > 1;
        state.player1_platform = addPlatformType(state.player1_platform, move.type);
        if (boosted) {
            state.player1_platform = addPlatformType(state.player1_platform, move.type);
        }
    };

    switch (move.attackType) {
        case AttackType.Normal:
        case AttackType.Status:
            if (!hasEnoughMana()) return { state };
            reduceMana();
            handlePlatformBoost();
            break;

        case AttackType.Special: {
            const platformCount = getAmountOfPlatformTypeByType(state.player1_platform, move.type);
            if (platformCount < move.cost) {
                ({ state } = ErrorFunc(state, "Player not enough platform type", dispatcher));
                return { state };
            }
            state.player1_platform = removePlatformTypeByType(state.player1_platform, move.type, move.cost);
            break;
        }
    }

    applyEffectIfNeeded();

    if (move.target === Target.Opponent) {
        const damage = applyBlastAttack(
            state.p1_blasts[playerIndex]!,
            state.wild_blast!,
            move,
            state.meteo
        );

        state.TurnStateData.p_move_damage = damage;
    }

    return { state };
}

function executeWildBlastAttack(
    state: WildBattleData,
    dispatcher: nkruntime.MatchDispatcher,
    logger: nkruntime.Logger
): { state: WildBattleData } {

    const moveIndex = state.TurnStateData.wb_move_index;

    if (moveIndex === -1) {
        state.wild_blast!.mana = calculateManaRecovery(
            state.wild_blast!.maxMana,
            state.wild_blast!.mana,
            true
        );
        state.TurnStateData.wb_turn_type = TurnType.WAIT;
        return { state };
    }

    const move = getMoveById(state.wild_blast!.activeMoveset![moveIndex]);

    const getTargetBlast = (): BlastEntity => {
        return move.target === Target.Opponent
            ? state.p1_blasts[state.p1_index]! : state.wild_blast!;
    };


    const setTargetBlast = (blast: BlastEntity) => {
        switch (move.target) {
            case Target.Opponent:
                state.p1_blasts[state.p1_index] = blast;
                break;
            case Target.Self:
                state.wild_blast = blast;
                break;
        }
    };

    const reduceMana = () => {
        state.wild_blast!.mana = clamp(
            state.wild_blast!.mana - move.cost,
            0,
            Number.POSITIVE_INFINITY
        );
    };

    const applyEffectIfNeeded = () => {
        if (move.effect === MoveEffect.None) return;

        if (move.attackType === AttackType.Special) {
            const updated = applyEffect(getTargetBlast(), move);
            setTargetBlast(updated);
            state.TurnStateData.wb_move_effect = move.effect;
        } else {
            const result = calculateEffectWithProbability(getTargetBlast(), move);
            setTargetBlast(result.blast);
            state.TurnStateData.wb_move_effect = result.moveEffect;
        }
    };

    const handlePlatformBonus = () => {
        state.wild_blast_platform = addPlatformType(state.wild_blast_platform, move.type);
        if (calculateWeatherModifier(state.meteo, move.type) > 1) {
            state.wild_blast_platform = addPlatformType(state.wild_blast_platform, move.type);
        }
    };

    logger.debug('Wild blast attack with move');

    switch (move.attackType) {
        case AttackType.Special: {
            const platformCount = getAmountOfPlatformTypeByType(state.wild_blast_platform, move.type);
            if (platformCount < move.cost) {
                ({ state } = ErrorFunc(state, "Wild blast not enough platform type", dispatcher));
                return { state };
            }

            state.wild_blast_platform = removePlatformTypeByType(state.wild_blast_platform, move.type, move.cost);
            break;
        }

        case AttackType.Normal:
        case AttackType.Status:
            reduceMana();
            handlePlatformBonus();
            break;
    }
    logger.debug('Wild blast 2');


    applyEffectIfNeeded();
    logger.debug('Wild 3');

    if (move.target === Target.Opponent) {
        const damage = applyBlastAttack(
            state.wild_blast!,
            state.p1_blasts[state.p1_index]!,
            move,
            state.meteo
        );
        state.TurnStateData.wb_move_damage = damage;
    }
    logger.debug('Wild 4');

    state.TurnStateData.wb_turn_type = TurnType.ATTACK;

    return { state };
}


function handleAttackTurn(isPlayerFaster: boolean, state: WildBattleData, move: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): { state: WildBattleData } {
    ({ state } = isPlayerFaster ? executePlayerAttack(state, move, dispatcher) : executeWildBlastAttack(state, dispatcher,logger));

    ({ state } = checkIfMatchContinue(nk, logger, state, dispatcher));

    return { state };
}

function performAttackSequence(state: WildBattleData, playerMove: Move, dispatcher: nkruntime.MatchDispatcher, nk: nkruntime.Nakama, logger: nkruntime.Logger): WildBattleData {

    let firstIsPlayer: boolean;

    if (state.TurnStateData.wb_move_index >= 0) {
        const wildMoveId = state.wild_blast!.activeMoveset![state.TurnStateData.wb_move_index];
        const wildMove = getMoveById(wildMoveId);

        if (playerMove.priority !== wildMove.priority) {
            firstIsPlayer = playerMove.priority > wildMove.priority;
        } else {
            firstIsPlayer = getFasterBlast(state.p1_blasts[state.p1_index]!, state.wild_blast!);
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

function checkIfMatchContinue(nk: nkruntime.Nakama, logger: nkruntime.Logger, state: WildBattleData, dispatcher: nkruntime.MatchDispatcher): { state: WildBattleData } {

    const playerBlast = state.p1_blasts[state.p1_index]!;
    const wildBlast = state.wild_blast!;

    const wildAlive = isBlastAlive(wildBlast);
    const playerAlive = isBlastAlive(playerBlast);
    const allPlayerDead = isAllBlastDead(state.p1_blasts);

    if (!wildAlive) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(true));
        addExpOnBlastInGame(nk, logger, state.player1_id, playerBlast, wildBlast);
        state.battle_state = BattleState.END;
    } else if (allPlayerDead) {
        dispatcher.broadcastMessage(OpCodes.MATCH_END, JSON.stringify(false));
        state.battle_state = BattleState.END;
    } else if (!playerAlive) {
        state.battle_state = BattleState.WAITFORPLAYERSWAP;
    }

    return { state };
}


//#endregion