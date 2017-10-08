using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalemServer.Game
{
    public static class Messages
    {
        public static String CHAT_LIVING { get; } = Convert("[b]{0}:[/b] {1}");
        public static String CHAT_DEAD { get; } = Convert("[color=red][i]{0}:[/i][/color] [color=gray]{1}[/color]");
        public static String CHAT_JAILOR { get; } = Convert("[color=gray][b]Jailor:[/b][/color] {1}");

        public static String ABILITY_DIDNT_USE_NIGHT { get; } = Convert("[bcolor=red][color=white]You did not use your night ability[/bcolor][/color]");
        public static String ABILITY_DIDNT_USE_DAY { get; } = Convert("[bcolor=red][color=white]You did not use your day ability[/bcolor][/color]");
        public static String ABILITY_ESCORT_ROLEBLOCKED { get; } = Convert("[bcolor=red][color=white]You were roleblocked![/bcolor][/color]");
        public static String ABILITY_SHERIFF_NOT_SUS { get; } = Convert("Your target was not suspicious");
        public static String ABILITY_SHERIFF_MAFIA { get; } = Convert("Your target is a member of the Mafia!");
        public static String ABILITY_SHERIFF_SERIAL_KILLER { get; } = Convert("Your target is a Werewolf!");
        public static String ABILITY_SHERIFF_WEREWOLF { get; } = Convert("Your target is a Serial Killer!");

        public static String GAME_JOIN { get; } = Convert("[b]{0} has joined the game[/b]");
        public static String GAME_PICKED_NAME { get; } = Convert("[b]{0} has chosen their name![/b]");
        public static String GAME_PICKED_RENAME { get; } = Convert("[b]{0} has renamed themselves to {1}![/b]");
        public static String GAME_QUIT { get; } = Convert("[b]{0} has left the game[/b]");

        // Information

        public static String INFO_INVALID_NAME { get; } = Convert("[color=green]That name is not valid[/color]");
        public static String INFO_NAME_EXISTS { get; } = Convert("[color=green]That name already exists[/color]");
        public static String INFO_STOP_SPAM { get; } = Convert("[b]Cease your futile spamming or you will be yelled at[/b]");
        public static String INFO_RECEIVED_ROLE { get; } = Convert("[color=blue]You have the role {0}[/color]");

        // Announcements that appear as an overlay

        public static String ANNOUNCE_DEATH_NAME { get; } = Convert("{0}'s body was found last night");
        public static String ANNOUNCE_DEATH_WILL_FOUND { get; } = Convert("We found a will next to their body");
        public static String ANNOUNCE_DEATH_WILL_NOT_FOUND { get; } = Convert("We could not find a last will.");
        public static String ANNOUNCE_DEATH_DEATHNOTE { get; } = Convert("A deathnote was found next to the body");
        public static String ANNOUNCE_DEATH_ROLE { get; } = Convert("They were {0}.");

        public static String VOTING_VOTED_PLAYER { get; } = Convert("[b]{0}[/b] [color=green]has voted against[/color] [b]{1}[/b]");
        public static String VOTING_UNVOTED_PLAYER { get; } = Convert("[b]{0}[/b] [color=green]cancelled their vote[/color]");
        public static String VOTING_VOTED_ANOTHER_PLAYER { get; } = Convert("[b]{0}[/b] [color=green]changed their vote to[/color] [b]{1}[/b]");
        public static String VOTING_JUDGEMENT_CAST { get; } = Convert("[b]{0}[/b] has voted");
        public static String VOTING_JUDGEMENT_UNCAST { get; } = Convert("[b]{0}[/b] cancelled their vote");
        public static String VOTING_JUDGEMENT_SWITCH { get; } = Convert("[b]{0}[/b] switched votes");
        public static String VOTING_VOTE_RESULTS_GUILTY { get; } = Convert("[b]{0}[/b] voted [color=red]guilty[/color]");
        public static String VOTING_VOTE_RESULTS_INNOCENT { get; } = Convert("[b]{0}[/b] voted [color=green]innocent.[/color]");
        public static String VOTING_VOTE_RESULTS_ABSTAINED { get; } = Convert("[b]{0}[/b] [color=aqua]abstained.[/color]");

        // Deaths

        public static String DEATH_SUICIDE { get; } = Convert("{0} {1}commited suicide");
        public static String DEATH_MAFIA { get; } = Convert("{0} was {1}murdered by the mafia");

        /// <summary>
        /// A method to convert basic BBCode to HTML A method to convert BBCode to HTML
        /// Author: Danny Battison
        /// Contact: gabehabe @googlemail.com
        /// </summary>
        /// <param name="str">A string formatted in BBCode</param>
        /// <returns>The HTML representation of the BBCode string</returns>
        private static string Convert(string str)
        {
            Regex exp;
            // format the bold tags: [b][/b] becomes:
            // <strong></strong>
            exp = new Regex(@"[b](.+?)[/b]");
            str = exp.Replace(str, "<strong>$1</strong>");

            // format the italic tags: [i][/i] becomes:
            // <em></em>
            exp = new Regex(@"[i](.+?)[/i]");
            str = exp.Replace(str, "<em>$1</em>");

            // format the underline tags: [u][/u] becomes:
            // <u></u>
            exp = new Regex(@"[u](.+?)[/u]");
            str = exp.Replace(str, "<u>$1</u>");

            // format the strike tags: [s][/s] becomes:
            // <strike></strike>
            exp = new Regex(@"[s](.+?)[/s]");
            str = exp.Replace(str, "<strike>$1</strike>");

            // format the url tags: [url=www.website.com]my site[/url] becomes:
            // <a href="www.website.com">my site</a>
            exp = new Regex(@"[url=([^]]+)]([^]]+)[/url]");
            str = exp.Replace(str, "<a href=\"$1\">$2</a>");

            // format the img tags:
            // becomes: <img src="www.website.com/img/image.jpeg">
            exp = new Regex(@"[img]([^]]+)[/img]");
            str = exp.Replace(str, "<img src=\"$1\">");

            // format img tags with alt: [img=www.website.com/img/image.jpeg]this is the alt text[/img]
            // becomes: <img src="www.website.com/img/image.jpeg" alt="this is the alt text">
            exp = new Regex(@"[img=([^]]+)]([^]]+)[/img]");
            str = exp.Replace(str, "<img src=\"$1\" alt=\"$2\">");

            //format the colour tags: [color=red][/color]
            // becomes: <font color="red"></font>
            // supports UK English and US English spelling of colour/color
            exp = new Regex(@"[color=([^]]+)]([^]]+)[/color]");
            str = exp.Replace(str, "<span color=\"$1\">$2</font>");
            exp = new Regex(@"[bcolor=([^]]+)]([^]]+)[/bcolor]");
            str = exp.Replace(str, "<span background-color=\"$1\">$2</font>");

            // format the size tags: [size=3][/size] becomes:
            // <font size="+3"></font>
            exp = new Regex(@"[size=([^]]+)]([^]]+)[/size]");
            str = exp.Replace(str, "<span size=\" +$1\">$2</font>");

            // lastly, replace any new line characters with

            str = str.Replace("rn", "\nrn");

            return str;
        }

        public static String PickRandomName()
        {
            return PickRandomFirstName() + " " + PickRandomLastName();
        }

        private static String PickRandomFirstName()
        {
            switch (new Random().Next(0, 15))
            {
                case 0:
                    return "Cotton";

                case 1:
                    return "Deodat";

                case 2:
                    return "Edward";

                case 3:
                    return "Giles";

                case 4:
                    return "James";

                case 5:
                    return "James";

                case 6:
                    return "John";

                case 7:
                    return "John";

                case 8:
                    return "John";

                case 9:
                    return "Jonathan";

                case 10:
                    return "Samuel";

                case 11:
                    return "Samuel";

                case 12:
                    return "Thomas";

                case 13:
                    return "William";

                case 14:
                    return "William";

                default:
                    return "Player " + new Random().Next(0, 150);
            }
        }

        private static String PickRandomLastName()
        {
            switch (new Random().Next(0, 15))
            {
                case 0:
                    return "Mather";

                case 1:
                    return "Lawson";

                case 2:
                    return "Bishop";

                case 3:
                    return "Corey";

                case 4:
                    return "Bayley";

                case 5:
                    return "Russel";

                case 6:
                    return "Hathorne";

                case 7:
                    return "Proctor";

                case 8:
                    return "Willard";

                case 9:
                    return "Corwin";

                case 10:
                    return "Parris";

                case 11:
                    return "Sewall";

                case 12:
                    return "Danforth";

                case 13:
                    return "Hobbs";

                case 14:
                    return "Phips";

                default:
                    return "Player " + new Random().Next(0, 150);
            }
        }
    }
}