using System;

namespace SalemServer.Game
{
    public enum GamePhase
    {
        /// <summary>
        /// Before the game starts, just does nothing until enough players
        /// </summary>
        PREGAME,

        /// <summary>
        /// The game has started, pick your name
        /// </summary>
        PICK_NAME,

        /// <summary>
        /// Names have been picked, find out what role you are
        /// </summary>
        ASSIGNING_ROLE,

        /// <summary>
        /// The beginning of the night phase, players can now talk at night and use their night actions
        /// </summary>
        NIGHT_BEGIN,

        /// <summary>
        /// The ending of the night phase, death messages and results pop up, then after a few
        /// seconds it moves on to day
        /// </summary>
        NIGHT_END,

        /// <summary>
        /// Players walk out of their houses
        /// </summary>
        DAY_BEGIN,

        /// <summary>
        /// Each death starts displaying. First the player name and the cause of death
        /// </summary>
        DEATH_NAME,

        /// <summary>
        /// Announce if a will was or wasn't found
        /// </summary>
        DEATH_WILL_EXISTS,

        /// <summary>
        /// Display the death will
        /// </summary>
        DEATH_WILL_DISPLAY,

        /// <summary>
        /// Announce a deathnote was found
        /// </summary>
        DEATH_DEATHNOTE_FOUND,

        /// <summary>
        /// Show their deathnote if it exists, otherwise skip this
        /// </summary>
        DEATH_DEATHNOTE_DISPLAY,

        /// <summary>
        /// Now show their role
        /// </summary>
        DEATH_ROLE,

        /// <summary>
        /// Announce their role was [Cleaned]
        /// </summary>
        DEATH_ROLE_CLEANED,

        /// <summary>
        /// The deaths finally finish displaying and players can now talk about their leads
        /// </summary>
        DISCUSSION,

        /// <summary>
        /// The discussion should be done, you can now vote people up
        /// </summary>
        VOTING,

        /// <summary>
        /// There was enough votes to vote someone up, they're now walking to the stand.
        /// </summary>
        VOTING_UP,

        /// <summary>
        /// Only the defendant can talk now, they must explain why they are innocent without anyone
        /// able to outtalk them
        /// </summary>
        VOTING_DEFENSE,

        /// <summary>
        /// The town must vote innocent or guilty on the player
        /// </summary>
        VOTING_DECISION,

        /// <summary>
        /// They were found guilty, lynch animation
        /// </summary>
        VOTING_GUILTY,

        /// <summary>
        /// Announce if their will existed or not
        /// </summary>
        VOTING_DEATH_WILL_EXISTS,

        /// <summary>
        /// They just died, will shows up
        /// </summary>
        VOTING_DEATH_WILL_DISPLAY,

        /// <summary>
        /// Their role now shows up
        /// </summary>
        VOTING_DEATH_ROLE,

        /// <summary>
        /// They were found innocent and walk down from the stand
        /// </summary>
        VOTING_INNO,

        /// <summary>
        /// Everyone walks inside, goto night_begins
        /// </summary>
        DAY_ENDS
    }

    public class GameTime
    {
        /// <summary>
        /// Returns the seconds this phase lasts for, 0 seconds if it doesn't expire naturally
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        public static int GetTime(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.NIGHT_BEGIN: return 40;
                case GamePhase.NIGHT_END: return 3; // Allow players a moment to see the results of the night
                case GamePhase.DAY_BEGIN: return 5; // Players walk out to see the town, then deaths begin.
                case GamePhase.DEATH_NAME: return 10; // We spend this long looking at their name and weeping
                case GamePhase.DEATH_WILL_EXISTS: return 5; // Announce a will was found or wasn't
                case GamePhase.DEATH_WILL_DISPLAY: return 10; // Display their will if it existed
                case GamePhase.DEATH_DEATHNOTE_FOUND: return 10; // Announce there was a deathnote if it existed
                case GamePhase.DEATH_DEATHNOTE_DISPLAY: return 10; // Display the deathnote
                case GamePhase.DEATH_ROLE: return 10; // Their role was this
                case GamePhase.DISCUSSION: return 40; // Players have 40 seconds to talk about the events of the nights and their suspicions
                case GamePhase.VOTING: return 20; // Players have 20 seconds to vote up someone to be lynched
                case GamePhase.VOTING_DEFENSE: return 20; // The victim has 20 seconds to plead their case
                case GamePhase.VOTING_DECISION: return 20; // 20 seconds for players to vote guilty or innocent
                case GamePhase.VOTING_GUILTY: return 10; // The lynched player knows he's going to die, the votes are shown.
                case GamePhase.VOTING_DEATH_WILL_EXISTS: return 5; // Announce a will was found, or wasn't
                case GamePhase.VOTING_DEATH_WILL_DISPLAY: return 10; // The lynched player has his will shown.
                case GamePhase.VOTING_DEATH_ROLE: return 10; // The lynched player has just died, his will is shown.
                case GamePhase.VOTING_INNO: return 5; // Walk off the stand
                case GamePhase.DAY_ENDS: return 5; // Walk inside house
                default:
                    throw new InvalidOperationException("GamePhase " + phase + " does not have an associated time");
            }
        }

        /// <summary>
        /// What phase comes next if they expire naturally?
        /// </summary>
        /// <param name="GamePhase"></param>
        /// <returns></returns>
        public static GamePhase GetNextPhase(GamePhase phase)
        {
            switch (phase)
            {
                case GamePhase.NIGHT_BEGIN: return GamePhase.NIGHT_END;
                case GamePhase.NIGHT_END: return GamePhase.DAY_BEGIN;
                case GamePhase.DEATH_WILL_DOESNT_EXIST: return GamePhase.DEATH_ROLE;
                case GamePhase.DISCUSSION: return GamePhase.VOTING;
                case GamePhase.VOTING: return GamePhase.DAY_ENDS;
                case GamePhase.VOTING_DEFENSE: return GamePhase.VOTING_DECISION;
                case GamePhase.VOTING_INNO: return GamePhase.VOTING;
                case GamePhase.VOTING_GUILTY: return GamePhase.VOTING_DEATH_WILL_EXISTS;
                case GamePhase.VOTING_DEATH_WILL_DISPLAY: return GamePhase.VOTING_DEATH_ROLE;
                case GamePhase.VOTING_DEATH_ROLE: return GamePhase.DAY_ENDS;
                case GamePhase.DAY_ENDS: return GamePhase.NIGHT_BEGIN;
                case GamePhase.DAY_BEGIN:
                case GamePhase.VOTING_DEATH_WILL_EXISTS:
                case GamePhase.DEATH_NAME:
                case GamePhase.DEATH_ROLE:
                case GamePhase.VOTING_DECISION:
                case GamePhase.DEATH_WILL_EXISTS:
                    throw new InvalidOperationException("GamePhase " + phase + " requires the next phase to be handled manually");
                default: throw new System.Collections.Generic.KeyNotFoundException("GamePhase " + phase + " does not have a GamePhase to follow it");
            }
        }
    }
}