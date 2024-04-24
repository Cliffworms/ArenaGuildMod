// Project:         Arena Guild for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2024 Hazelnut
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Hazelnut

using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.Guilds
{
    public class ArenaGuild : Guild
    {
        #region Constants

        private const int factionId = 1030;

        const int notEnoughGoldId = 454;
        const int recallEffectId = 94;

        #endregion

        #region Static Data

        protected static TextFile.Token newLine = TextFile.CreateFormatToken(TextFile.Formatting.JustifyCenter);

        // Guild messages - must clone any that contain macros before returning.

        protected static TextFile.Token[] welcomeTokens =
        {
            TextFile.CreateTextToken("Excellent, %pcn, welcome to the Arena! "), newLine, newLine,

            TextFile.CreateTextToken("We'll get you started on field work as soon as possible. "), newLine,
            TextFile.CreateTextToken("You'll begin your Arena career with the title of "), newLine,
            TextFile.CreateTextToken("%lev. However, with hard work and dedication, "), newLine,
            TextFile.CreateTextToken("you may be recognised for promotion soon enough. Please"), newLine,
            TextFile.CreateTextToken("do make use of our training facilities to study languages "), newLine,
            TextFile.CreateTextToken("or to improve your field work skills. "), newLine, newLine,

            TextFile.CreateTextToken("Here's a book containing guild ranks and locations of all guild "), newLine,
            TextFile.CreateTextToken("halls. Also here's a Mark of Recall, and a free locator device "), newLine,
            TextFile.CreateTextToken("to try out the next time you find yourself lost in a labyrinth "), newLine,
            TextFile.CreateTextToken("unable to find whatever it is you're seeking. Locators look like"), newLine,
            TextFile.CreateTextToken("Ankh symbols and will activate once you've explored enough. "), newLine, newLine,

            TextFile.CreateTextToken("Locators are provided free for guild quests involving dungeon "), newLine,
            TextFile.CreateTextToken("delving, but you can also purchase them from us for a price, "), newLine,
            TextFile.CreateTextToken("with discounts given for any relics you can find for the guild. "), newLine, newLine
        };

        protected static TextFile.Token[] eligibleTokens =
        {
            TextFile.CreateTextToken("Hmm, yes, you seem like a suitable candidate to join the Arena"), newLine,
            TextFile.CreateTextToken("our field work and finding the relics that we need for research. "), newLine, newLine,

            TextFile.CreateTextToken("We offer classes in all the various obscure languages "), newLine,
            TextFile.CreateTextToken("of Tamriel, as well as some of the more practical skills "), newLine,
            TextFile.CreateTextToken("required in the field while searching remote locations "), newLine,
            TextFile.CreateTextToken("for interesting antiquities and rare artifacts. "), newLine, newLine,

            TextFile.CreateTextToken("New members receive a free recall mark with 30 charges "), newLine,
            TextFile.CreateTextToken("to assist with transport without dabbling in magic arts. "), newLine,
            TextFile.CreateTextToken("Once you rise in our ranks to Field Officer level, then you'll "), newLine,
            TextFile.CreateTextToken("be able to get your recall mark repaired and recharged. "), newLine, newLine,

            TextFile.CreateTextToken("Beyond field work, our higher ranks are open to the more "), newLine,
            TextFile.CreateTextToken("accomplished scholars among us, and provide a large "), newLine,
            TextFile.CreateTextToken("reduction to the cost of locator devices. Note that "), newLine,
            TextFile.CreateTextToken("only those with sufficient intellect will be promoted. "), newLine, newLine,
        };

        protected static TextFile.Token[] ineligibleLowSkillTokens =
        {
            TextFile.CreateTextToken("I am sad to say that you are not eligible to join the Arena."), newLine,
            TextFile.CreateTextToken("We only accept members who have studied languages or the "), newLine,
            TextFile.CreateTextToken("other skills useful for field work; such as climbing, "), newLine,
            TextFile.CreateTextToken("lockpicking, or stealth. "), newLine,
        };

        protected static TextFile.Token[] ineligibleBadRepTokens =
        {
            TextFile.CreateTextToken("I am sad to say that you are ineligible to join our guild."), newLine,
            TextFile.CreateTextToken("Your reputation amongst scholars is such that we do not "), newLine,
            TextFile.CreateTextToken("wish to be associated with you, even for simple field work. "), newLine,
        };

        protected static TextFile.Token[] promotionTokens =
        {
            TextFile.CreateTextToken("Congratulations, %pcf. Because of your outstanding work for "), newLine,
            TextFile.CreateTextToken("the guild, we have promoted you to the rank of %lev. "), newLine,
            TextFile.CreateTextToken("Keep up the good work, and continue to study hard. "), newLine,
        };

        protected static string[] rankTitles = {
            "Fresh meat", "Punchbag", "Trainee", "Scrapper", "Brawler", "Bruiser", "Pugilist", "Duelist", "Gladiator", "Champion"
        };

        protected static List<DFCareer.Skills> guildSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Dodging,
                DFCareer.Skills.HandToHand,
            };

        protected static List<DFCareer.Skills> trainingSkills = new List<DFCareer.Skills>() {
                DFCareer.Skills.Dodging,
                DFCareer.Skills.HandToHand,
            };

        #endregion

        #region Properties

        public override string[] RankTitles { get { return rankTitles; } }

        public override List<DFCareer.Skills> GuildSkills { get { return guildSkills; } }

        public override List<DFCareer.Skills> TrainingSkills { get { return trainingSkills; } }

        #endregion

        #region Guild Membership and Faction

        public static int FactionId { get { return factionId; } }

        public override int GetFactionId()
        {
            return factionId;
        }

        #endregion

        #region Benefits

        public override bool CanRest()
        {
            return IsMember();
        }

        public override bool HallAccessAnytime()
        {
            return (rank >= 4);
        }

        #endregion

        #region Service Access:

        public override bool CanAccessLibrary()
        {
            return (rank >= 1);
        }

        public override bool CanAccessService(GuildServices service)
        {
            switch (service)
            {
                case GuildServices.Training:
                    return IsMember();
                case GuildServices.Quests:
                    return true;
                case GuildServices.Identify:
                    return true;
                case GuildServices.BuyPotions:
                    return (rank >= 1);
                case GuildServices.MakePotions:
                    return (rank >= 3);
                case GuildServices.DaedraSummoning:
                    return (rank >= 7);
                case GuildServices.MakeMagicItems:
                    return (rank >= 6);
            }
            return false;
        }

        #endregion

        #region Joining & messages

        override public void Join()
        {
            base.Join();

            // Fresh meat!
        }

        public override TextFile.Token[] TokensEligible(PlayerEntity playerEntity)
        {
            return eligibleTokens;
        }
        public override TextFile.Token[] TokensWelcome()
        {
            return (TextFile.Token[]) welcomeTokens.Clone();
        }
        public override TextFile.Token[] TokensPromotion(int newRank)
        {
            return (TextFile.Token[])promotionTokens.Clone();
        }
        public override TextFile.Token[] TokensIneligible(PlayerEntity playerEntity)
        {
            return (TextFile.Token[])ineligibleLowSkillTokens.Clone();
        }

        #endregion

    }

}
