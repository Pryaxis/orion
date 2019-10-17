﻿// Copyright (c) 2019 Pryaxis & Orion Contributors
// 
// This file is part of Orion.
// 
// Orion is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Orion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Orion.  If not, see <https://www.gnu.org/licenses/>.

namespace Orion.Entities {
    /// <summary>
    /// Specifies a buff type.
    /// </summary>
    public enum BuffType : byte {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        None = 0,
        ObsidianSkin = 1,
        Regeneration = 2,
        Swiftness = 3,
        Gills = 4,
        Ironskin = 5,
        ManaRegeneration = 6,
        MagicPower = 7,
        Featherfall = 8,
        Spelunker = 9,
        Invisibility = 10,
        Shine = 11,
        NightOwl = 12,
        Battle = 13,
        Thorns = 14,
        WaterWalking = 15,
        Archery = 16,
        Hunter = 17,
        Gravitation = 18,
        ShadowOrb = 19,
        Poisoned = 20,
        PotionSickness = 21,
        Darkness = 22,
        Cursed = 23,
        OnFire = 24,
        Tipsy = 25,
        WellFed = 26,
        Fairy = 27,
        Werewolf = 28,
        Clairvoyance = 29,
        Bleeding = 30,
        Confused = 31,
        Slow = 32,
        Weak = 33,
        Merfolk = 34,
        Silenced = 35,
        BrokenArmor = 36,
        Horrified = 37,
        TheTongue = 38,
        CursedInferno = 39,
        PetBunny = 40,
        BabyPenguin = 41,
        PetTurtle = 42,
        PaladinsShield = 43,
        Frostburn = 44,
        BabyEater = 45,
        Chilled = 46,
        Frozen = 47,
        Honey = 48,
        Pygmies = 49,
        BabySkeletronHead = 50,
        BabyHornet = 51,
        TikiSpirit = 52,
        PetLizard = 53,
        PetParrot = 54,
        BabyTruffle = 55,
        PetSapling = 56,
        Wisp = 57,
        RapidHealing = 58,
        ShadowDodge = 59,
        LeafCrystal = 60,
        BabyDinosaur = 61,
        IceBarrier = 62,
        Panic = 63,
        BabySlime = 64,
        EyeballSpring = 65,
        BabySnowman = 66,
        Burning = 67,
        Suffocation = 68,
        Ichor = 69,
        Venom = 70,
        WeaponImbueVenom = 71,
        Midas = 72,
        WeaponImbueCursedFlames = 73,
        WeaponImbueFire = 74,
        WeaponImbueGold = 75,
        WeaponImbueIchor = 76,
        WeaponImbueNanites = 77,
        WeaponImbueConfetti = 78,
        WeaponImbuePoison = 79,
        Blackout = 80,
        PetSpider = 81,
        Squashling = 82,
        Ravens = 83,
        BlackCat = 84,
        CursedSapling = 85,
        WaterCandle = 86,
        CozyFire = 87,
        ChaosState = 88,
        HeartLamp = 89,
        Rudolph = 90,
        Puppy = 91,
        BabyGrinch = 92,
        AmmoBox = 93,
        ManaSickness = 94,
        BeetleEndurance1 = 95,
        BeetleEndurance2 = 96,
        BeetleEndurance3 = 97,
        BeetleMight1 = 98,
        BeetleMight2 = 99,
        BeetleMight3 = 100,
        FairyRed = 101,
        FairyGreen = 102,
        Wet = 103,
        Mining = 104,
        Heartreach = 105,
        Calm = 106,
        Builder = 107,
        Titan = 108,
        Flipper = 109,
        Summoning = 110,
        Dangersense = 111,
        AmmoReservation = 112,
        Lifeforce = 113,
        Endurance = 114,
        Rage = 115,
        Inferno = 116,
        Wrath = 117,
        MinecartLeft = 118,
        Lovestruck = 119,
        Stinky = 120,
        Fishing = 121,
        Sonar = 122,
        Crate = 123,
        Warmth = 124,
        Hornet = 125,
        Imp = 126,
        ZephyrFish = 127,
        BunnyMount = 128,
        PigronMount = 129,
        SlimeMount = 130,
        TurtleMount = 131,
        BeeMount = 132,
        Spider = 133,
        Twins = 134,
        Pirate = 135,
        MiniMinotaur = 136,
        Slime = 137,
        MinecartRight = 138,
        Sharknado = 139,
        Ufo = 140,
        UfoMount = 141,
        DrillMount = 142,
        ScutlixMount = 143,
        Electrified = 144,
        MoonBite = 145,
        Happy = 146,
        Banner = 147,
        FeralBite = 148,
        Webbed = 149,
        Bewitched = 150,
        LifeDrain = 151,
        MagicLantern = 152,
        Shadowflame = 153,
        BabyFaceMonster = 154,
        CrimsonHeart = 155,
        Stoned = 156,
        PeaceCandle = 157,
        StarInABottle = 158,
        Sharpened = 159,
        Dazed = 160,
        DeadlySphere = 161,
        UnicornMount = 162,
        Obstructed = 163,
        Distorted = 164,
        DryadsBlessing = 165,
        MinecartRightMechanical = 166,
        MinecartLeftMechanical = 167,
        CuteFishronMount = 168,
        Penetrated = 169,
        SolarBlaze1 = 170,
        SolarBlaze2 = 171,
        SolarBlaze3 = 172,
        LifeNebula1 = 173,
        LifeNebula2 = 174,
        LifeNebula3 = 175,
        ManaNebula1 = 176,
        ManaNebula2 = 177,
        ManaNebula3 = 178,
        DamageNebula1 = 179,
        DamageNebula2 = 180,
        DamageNebula3 = 181,
        StardustCell = 182,
        Celled = 183,
        MinecartLeftWooden = 184,
        MinecartRightWooden = 185,
        DryadsBane = 186,
        StardustGuardian = 187,
        StardustDragon = 188,
        Daybroken = 189,
        SuspiciousLookingEye = 190,
        CompanionCube = 191,
        SugarRush = 192,
        BasiliskMount = 193,
        MightyWind = 194,
        WitheredArmor = 195,
        WitheredWeapon = 196,
        Oozed = 197,
        StrikingMoment = 198,
        CreativeShock = 199,
        PropellerGato = 200,
        Flickerwick = 201,
        Hoardagron = 202,
        BetsysCurse = 203,
        Oiled = 204,
        BallistaPanic = 205
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
