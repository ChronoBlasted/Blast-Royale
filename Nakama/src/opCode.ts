enum OpCodes {
    MATCH_START = 10,

    PLAYER_WAIT = 15,
    PLAYER_ATTACK = 20,
    PLAYER_USE_ITEM = 30,
    PLAYER_CHANGE_BLAST = 40,

    PLAYER_READY = 50,
    ENEMY_READY = 55,

    MATCH_ROUND = 60,
    PLAYER_MUST_CHANGE_BLAST = 61,

    MATCH_END = 100,

    ERROR_SERV = 404,

    DEBUG = 500,

}

enum notificationOpCodes {
    CURENCY = 1000,
    BLAST = 1010,
    ITEM = 1020,
}