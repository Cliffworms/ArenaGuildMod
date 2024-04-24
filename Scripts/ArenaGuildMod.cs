// Project:         Arena Guild for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2024 Hazelnut
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Hazelnut

using System;
using System.Collections.Generic;
using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Guilds;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;

namespace ArenaGuildMod
{
    public class ArenaGuildMod : MonoBehaviour
    {
        static Mod mod;

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<ArenaGuildMod>();
        }

        void Awake()
        {
            InitMod();
            mod.IsReady = true;
        }

        public static void InitMod()
        {
            Debug.Log("Begin mod init: ArenaGuild");

            // Register the new faction id's
            if (RegisterFactionIds())
            {
                // Register the Guild class
                if (!GuildManager.RegisterCustomGuild(FactionFile.GuildGroups.GGroup1, typeof(ArenaGuild)))
                    throw new Exception("GuildGroup GGroup1 is already in use, unable to register ArenaGuild guild.");

                // Register the quest list
                //if (!QuestListsManager.RegisterQuestList("ArenaGuild"))
                //    throw new Exception("Quest list name is already in use, unable to register ArenaGuild quest list.");

                // Register the quest service id
                Services.RegisterGuildService(1030, GuildServices.Quests);
                // Register the training service id
                Services.RegisterGuildService(1031, GuildServices.Training);
            }
            else
                throw new Exception("Faction id's are already in use, unable to register factions for ArenaGuild guild.");

            Debug.Log("Finished mod init: ArenaGuild");
        }

        private static bool RegisterFactionIds()
        {
            bool success = FactionFile.RegisterCustomFaction(1030, new FactionFile.FactionData()
            {
                id = 1030,
                parent = 0,
                type = 2,
                name = "The Arena",
                summon = -1,
                region = -1,
                power = 40,
                //enemy1 = (int)FactionFile.FactionIDs.The_Mages_Guild,
                face = -1,
                race = -1,
                flat1 = 0x5B03, // Face from 182-3
                flat2 = 0x5C21, // Face from 184-33
                sgroup = 0,
                ggroup = 1,
                children = new List<int>() { 1031 }
            });
            success = FactionFile.RegisterCustomFaction(1031, new FactionFile.FactionData()
            {
                id = 1031,
                parent = 1030,
                type = 2,
                name = "Arena Trainers",
                summon = -1,
                region = -1,
                power = 25,
                face = -1,
                race = -1,
                flat2 = 0x5C0C, // Face from 184-12
                sgroup = 0,
                ggroup = 1,
                children = null
            }) && success;
            return success;
        }

    }
}