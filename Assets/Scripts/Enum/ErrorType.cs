public enum ErrorType
{
    NONE,
    // Shop error
    SHOP_ALREADY_BUYED,
    NOT_ENOUGH_COIN,
    NOT_ENOUGH_GEMS,

    // Move error
    NOT_UNLOCKED_MOVE,

    // Battle error
    NOT_ENOUGH_MANA,
    NOT_ENOUGH_PLATFORM_CHARGE,
    ALREADY_IN_BATTLE,
    IS_FULL_HP,
    IS_FULL_MANA,
    HAS_NO_STATUS,
    IS_FAINTED,

    SERVER_ERROR
}