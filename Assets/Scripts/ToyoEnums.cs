public enum CARD_TYPE
{
    HEAVY = 0,
    FAST = 1,
    DEFENSE = 2,
    BOND = 3,
    SUPER = 4
}

public enum ATTACK_TYPE
{
    CYBER = 0,
    PHYSICAL = 1
}

public enum DEFENSE_TYPE
{
    BLOCK = 0,
    DODGE = 1
}

public enum ATTACK_SUB_TYPE
{
    NEUTRAL = 0,
    PIERCING = 1,
    SMASHING = 2,
    SLASHING = 3,
    MAGNETIC = 4,
    ELECTRIC = 5
}

public enum EFFECT_TYPE
{
    HP_MOD = 0,
    AP_MOD = 1,
    CHANGE_STAT = 2,
    CARD_MOD_DAMAGE = 3,
    CARD_MOD_COST = 4,
    CARD_MOD_TRUE_DAMAGE = 5,
    CARD_MOD_LIFE_STEAL = 6,
    RULE_MOD = 7,
    STUN = 8,
    DISCARD = 9
}

public enum TOYO_PIECE
{
    HEAD = 0,
    CHEST = 1,
    R_ARM = 2,
    L_ARM = 3,
    R_HAND = 4,
    L_HAND = 5,
    R_LEG = 6,
    L_LEG = 7,
    R_FOOT = 8,
    L_FOOT = 9
}

public enum TOYO_RARITY
{
    COMMON = 1,
    UNCOMMON = 2,
    RARE = 3,
    LIMITED = 4,
    COLLECTOR = 5,
    PROTOTYPE = 6
}

public enum TOYO_TECHNOALLOY
{
    SIDERITE = 0,
    MERCURY = 1,
    TITANIUM = 2,
    ALUMINUM = 3,
    CARBON = 4,
    SILICON = 5
}

public enum TOYO_STAT
{
    VITALITY = 0,
    RESISTANCE = 1,
    RESILIENCE = 2,
    PHYSICAL_STRENGTH = 3,
    CYBER_FORCE = 4,
    TECHNIQUE = 5,
    ANALYSIS = 6,
    AGILITY = 7,
    SPEED = 8,
    PRECISION = 9,
    STAMINA = 10,
    LUCK = 11
}

public enum RANKING_TYPE
{
    COOPER = 0,
    STEEL = 1,
    BRONZE = 2,
    SILVER = 3,
    GOLD = 4,
    PLATINUM = 5,
    SIDERITE = 6,
    MASTER = 7
}

public enum TrainingActionType
{
    None = 0,
    Punch = 1,
    Kick = 2,
    Move = 3
}

public enum TRAINING_RESULT
{
    NONE = 0,
    TOTALLY_WRONG = 1,
    WRONG_POSITION = 2,
    TOTALLY_CORRECT = 3
}

public enum BOX_TYPE
{
    None = 0,
    Regular = 1,
    Fortified = 2,
}

public enum BOX_REGION
{
    None = 0,
    Kytunt = 1,
    Jakana = 2,
    Xeon = 3
}

public enum TRAINING_STATUS
{
    NONE = 0,
    APPROVE_PENDING = 1,
    APPROVE_ERROR = 2,
    APPROVE_FINISHED = 3,
    STAKE_PENDING = 4,
    STAKE_ERROR = 5,
    IN_TRAINING = 6,
    CLAIM_PENDING = 7,
    CLAIM_ERROR = 8,
    FINISHED = 9,
    FINISHED_ERROR = 10,
    USER_CANCEL = 11
}

public enum TRANSACTION_TYPE
{
    NONE = 0,
    APPROVE = 1,
    STAKE = 2,
    CLAIM = 3
}

