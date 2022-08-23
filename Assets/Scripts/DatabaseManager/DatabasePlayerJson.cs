using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

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
    public class BoxJSON
    {
        public string wallet;
        public Box[] boxes;
    }

    [Serializable]
    public class OpenBox
    {
        public string toyoHash;
        public string toyoSignature;
        public string tokenId;
        public string typeId;
        public string tokenIdClosedBox;
        public string tokenIdOpenBox;
        public Toyo toyo;
        public bool isOpen;
    }

    [Serializable]
    public class Box
    {
        public string objectId;
        public bool isOpen;
        public string hash;
        public Toyo toyo;
        public string createdAt;
        public string updatedAt;
        public string tokenId;
        public string idOpenBox;
        public string idClosedBox;
        public string type;
        public string region;
        public string currentOwner;
        public string lastUnboxingStartedAt;
        public string typeId;
        public Modifier[] modifiers;
    }

    [Serializable]
    public class BoxParent
    {
        public Box box;
    }
    
    [Serializable]
    public class Modifier
    {
        public string name;
        public int type;
        public string description;
        public string modification;
        public string restrictions;
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
        
        [FormerlySerializedAs("toyoPersona")] public ToyoPersona toyoPersonaOrigin; // Todo : remove, this is temporary only for the wrong JSON
        //public ToyoPersona toyoPersonaOrigin; 
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
        public string region;
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
        public string name;
    }
    
    [Serializable]
    public class LoginCallback
    {
        public string token;
        public string expiresAt;
    }

    [Serializable]
    public class TrainingConfigJSON
    {
        public string name;
        public int startAt;
        public int endAt;
        public string story;
        public bool isOngoing;
        public float bondReward;
        public float bonusBondReward;
        public string toyoTrainingConfirmationMessage;
        public string inTrainingMessage;
        public string losesMessage;
        public string rewardMessage;
        public string[] blows;
        public BlowConfig[] blowsConfig;
    }
}