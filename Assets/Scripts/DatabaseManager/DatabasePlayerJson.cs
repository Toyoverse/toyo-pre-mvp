using System;
using System.Collections.Generic;

namespace Database
{

    [Serializable]
    public class DatabasePlayerJson
    {
        public Player player;
    }
    
    [Serializable]
    public class CallbackToyoList
    {
        public Toyo[] toyos;
    }
    
    [Serializable]
    public class CallbackToyoDetails
    {
        public Toyo toyo;
    }
    

    [Serializable]
    public class Player
    {
        public Box[] boxes;
        public Toyo[] toyos;
    }

    [Serializable]
    public class Box
    {
        public string objectId;
        public string isOpen;
        public string hash;
        public Toyo toyo;
        public string idOpenBox;
        public string idClosedBox;
        public int type;
        public string createdAt;
        public string updatedAt;
        public string tokenId;
        public string lastUnboxingStartedAt;
        public string typeId;
        public string[] specification;
        
    }

    [Serializable]
    public class Toyo
    {
        public string objectId;
        public string name;
        public bool hasTenParts;
        public bool isToyoSelected;
        public string createdAt;
        public string updatedAt;
        public string tokenId;
        public string transactionHash;
        
        public ToyoPersona toyoPersona; // Todo : remove, this is temporary only for the wrong JSON
        public ToyoPart[] parts;
    }

    [Serializable]
    public class ToyoPart
    {
        public string objectId;
        public TOYO_PIECE toyoPiece;
        public TOYO_TECHNOALLOY ToyoTechnoalloy;
        public string createdAt;
        public string updatedAt;
        public Card[] cards;

        public ToyoStats stats;
        public ToyoStats bonusStats;
        public ToyoPersona toyoPersona;
        public ToyoTheme toyoTheme;
        public int toyoLevel;
        public int hearthbound;
        public float toyoPartEXP;
        public bool isNFT;
    }

    [Serializable]
    public class ToyoStats
    {
        public float vitality;
        public float resistance;
        public float resilence;
        public float physicalStrength;
        public float cyberForce;
        public float technique;
        public float analysis;
        public float agility;
        public float speed;
        public float precision;
        public float stamina;
        public float luck;
        
    }

    [Serializable]
    public class Card
    {
        public string objectId;
        public string name;
        public CARD_TYPE cardType;
        public DEFENSE_TYPE defenseType;
        public int cost;
        public ATTACK_TYPE attackType;
        public ATTACK_SUB_TYPE attackSubType;
        public float duration;
        public string attackAnimation;
        public bool applyEffect;
        public string effectName;
        public string image_;
        public string createdAt;
        public string updatedAt;
    }

    [Serializable]
    public class ToyoPersona
    {
        public string objectId;
        public string createdAt;
        public string updatedAt;
        public string video;
        public string description;
        public string name;
        public int bodyType;
        public string thumbnail;

        //New
        public ToyoRegion region;
        public string rarity;
    }

    [Serializable]
    public class ToyoTheme
    {
        public string objectId;
        public string createdAt;
        public string updatedAt;
    }

    [Serializable]
    public class ToyoRegion
    {
        public string objectId;
    }
    
    [Serializable]
    public class LoginCallback
    {
        public string token;
        public string expiresAt;
    }


}