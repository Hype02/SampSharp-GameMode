using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Display;
using MySql.Data.MySqlClient;
using SampSharp.GameMode.Definitions;

namespace SAPC
{
    public partial class GameMode
    {
        protected override void OnPlayerCommandText(BasePlayer sender, CommandTextEventArgs e)
        {
            base.OnPlayerCommandText(sender, e);
            e.Success = true;
            Player player = sender as Player;
            if (player.Logged == false)
                return;
            string[] args = e.Text.Split(' ');
            foreach (var arg in args)
                arg.ToLower();
            string command = args[0].ToLower().Replace("/", String.Empty);
            if (command == "ban")
            {
                if (player.AdminLevel < Defines.AdminLevel.gamemaster)
                {
                    player.SendClientMessage($"{Color.Red}No premissions.");
                    return;
                }
                try
                {
                    Player target = Player.Find(args[1]);
                    byte days = byte.Parse(args[2]);
                    string reason = args[3];
                    if (target == null)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player not found.");
                        return;
                    }
                    if (target.Logged == false)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player is unlogged.");
                        return;
                    }
                    if (days < 1 || days > 30)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Days must be between value 1-30.");
                        return;
                    }
                    for (int i = 4; i < args.Length; i++)
                        reason += " " + args[i];
                    target.BanPlayer(days, reason, player.Name);
                    return;
                }
                catch
                {
                    player.SendClientMessage($"{Color.Gray}Proper usage: /ban [ID/Part of player name] [days] [reason]");
                    return;
                }
            }
            if (command == "spec")
            {
                if (player.AdminLevel == 0)
                {
                    player.SendClientMessage($"{Color.Red}No premissions.");
                    return;
                }
                try
                {
                    Player target = Player.Find(args[1]);
                    if (target == null)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player not found.");
                        return;
                    }
                    if (target.Logged == false)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player is unlogged.");
                        return;
                    }
                    if (target == player)
                    {
                        player.SendClientMessage($"{Color.DarkGray}You can't spectate yourself.");
                        return;
                    }
                    if (player.SpectatingPlayer)
                    {
                        player.ToggleSpectating(false);
                        Player.SendMessageToAdmins($"{player.StringAdminLevel} {player.Name} is no longer spectating {target.Name}");
                        return;
                    }
                    player.SpectatePlayer(target);
                    Player.SendMessageToAdmins($"{player.StringAdminLevel} {player.Name} is now spectating {target.Name}");
                    return;
                }
                catch
                {
                    player.SendClientMessage($"{Color.Gray}Proper usage: /spec [ID/Part of player name]");
                    return;
                }
            }
            if (command == "pm")
            {
                try
                {
                    Player target = Player.Find(args[1]);
                    if (target == null)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player not found.");
                        return;
                    }
                    if (target.Logged == false)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player is unlogged.");
                        return;
                    }
                    if (target == player)
                    {
                        player.SendClientMessage($"{Color.DarkGray}You can't send message to yourself.");
                        return;
                    }
                    string message = String.Empty;
                    for (int i = 2; i < args.Length; i++)
                    {
                        message += " " + args[i];
                    }
                    target.SendClientMessage($"{Color.Yellow}(( {player.Name}.{player.Id}: {message} ))");
                    player.SendClientMessage($"{Color.LightYellow}(( > {target.Name}.{target.Id}: {message} ))");
                    return;
                }
                catch
                {
                    player.SendClientMessage($"{Color.LightGray}Proper usage: /pm [ID/Part of player name] [message]");
                    return;
                }
            }
            if (command == "lobby")
            {
                if (player.InPursuit)
                {
                    player.InPursuit = false;
                    player.Spawn();
                    foreach (Player pl in Player.All)
                    {
                        if (pl.InPursuit)
                        {
                            pl.SendClientMessage($"{Color.GreenYellow}[LOBBY] {player.Name} left pursuit.");
                        }
                    }
                }
                else
                {
                    player.SendClientMessage($"{Color.Green}You are already in Lobby!");
                }
                return;

            }
            if (command == "kick")
            {
                if (player.AdminLevel == 0)
                {
                    player.SendClientMessage($"{Color.Red}No premissions.");
                    return;
                }
                try
                {
                    Player target = Player.Find(args[1]);
                    string reason = args[2];
                    if (target == null)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player not found.");
                        return;
                    }
                    if (target.Logged == false)
                    {
                        player.SendClientMessage($"{Color.DarkGray}Player is unlogged.");
                        return;
                    }
                    for (int i = 3; i < args.Length; i++)
                        reason += " " + args[i];
                    target.Kick($"{Color.IndianRed}AdmCmd: {player.StringAdminLevel} {player.Name} kicked {target.Name}. Reason: {reason}");
                    return;
                }
                catch
                {
                    player.SendClientMessage($"{Color.Gray}Proper usage: /kick [ID/Part of player name] [reason]");
                    return;
                }


            }
            if (command == "oban")
            {
                if (player.AdminLevel != Defines.AdminLevel.administrator)
                {
                    player.SendClientMessage($"{Color.Red}No premissions.");
                    return;
                }
                try
                {
                    string accountName = args[1];
                    byte days = byte.Parse(args[2]);
                    string reason = String.Empty;
                    for (int i = 3; i < args.Length; i++)
                        reason += " " + args[i];
                    MySqlDataReader reader = MySql.Reader($"SELECT uid FROM accounts WHERE name = '{accountName}' LIMIT 1;");

                    if (reader.HasRows)
                    {
                        reader.Close();
                        DateTimeOffset expireDate = DateTimeOffset.UtcNow;
                        expireDate = expireDate.AddDays(days);
                        MySql.Query($"UPDATE accounts SET adminName = '{player.Name}', banExpireDate='{expireDate.ToUnixTimeMilliseconds()}', banReason = '{reason}' WHERE name = '{accountName}' LIMIT 1;");
                        foreach (Player pl in Player.All) // we are going now to notify all players about offline ban. Let them now that /q is not a solution to avoid ban.
                        {
                            if (pl.Name == accountName) // oops, player quit server but joined again but didn't login yet. Don't worry I thought about that!
                            {
                                pl.BanExpireDate = expireDate.ToUnixTimeMilliseconds();
                                pl.AdminName = player.Name;
                                pl.BanReason = reason;
                                // good luck with login bro.
                            }
                            pl.SendClientMessage($"{Color.IndianRed}AdmCmd: {player.StringAdminLevel} {player.Name} offline banned {accountName} for {days} days. Reason:{reason}");
                        }
                        return;
                    }
                    player.SendClientMessage($"{Color.Gray}Account {accountName} wasn't found.");
                    reader.Close();
                    return;
                }
                catch
                {
                    player.SendClientMessage($"{Color.Gray}Proper usage for offline ban: /oban [Account Name] [days] [reason]");
                }
                return;
            }
            if (command == "dm")
            {
                if (player.PlayingDeathMatch)
                    player.LeaveDeathMatch();
                else
                    player.JoinDeathMatch();
                return;
            }
            if (command == "ooc")
            {
                try
                {
                    string message = args[1];
                    for (byte i = 2; i < args.Length; i++)
                    {
                        message += " " + args[i];
                    }
                    foreach (Player pl in Player.All)
                    {
                        if (pl.Logged)
                        {
                            pl.SendClientMessage($"{Color.White}(( [OOC] {player.AdminColorHex}{player.Name}{Color.White}: {message} ))");
                        }
                    }
                }
                catch
                {
                    player.SendClientMessage($"{Color.Gray}Proper usage: /ooc [message tp all players]");
                }
                return;
            }
            if (command == "a")
            {
                MessageDialog onlineAdmins = new MessageDialog("Online Admins", "", "Ok");
                foreach (Player pl in Player.All)
                {
                    if (pl.Logged)
                    {
                        if (pl.AdminLevel != 0)
                        {
                            onlineAdmins.Message += $"{Color.White}ID {pl.Id}\t{pl.AdminColorHex}{pl.StringAdminLevel} {pl.Name}{Color.White}\n";
                        }
                    }
                }
                onlineAdmins.Message += "\nUse /pm [ID/Name] to contact.";
                onlineAdmins.Show(player);
                return;
            }
            if (command == "report")
            {
                try
                {
                    Player target = Player.Find(args[1]);
                    if (target == null)
                    {
                        player.SendClientMessage("Player not found");
                        return;
                    }
                    if (target.Logged == false)
                    {
                        player.SendClientMessage("Player is not logged");
                        return;
                    }
                    if(target == player)
                    {
                        player.SendClientMessage($"{Color.LightSlateGray}You can't really report yourself.");
                        return;
                    }
                    string message = String.Empty;
                    for (byte i = 2; i < args.Length; i++)
                    {
                        message += " " + args[i];
                    }
                    if (message.Length > 128)
                    {
                        player.SendClientMessage($"{Color.Gray}Sorry your report is too long.");
                        return;
                    }
                    MySql.Query($"INSERT INTO reports (reporterName, reportedName, reason, date) VALUES ('{player.Name}', '{target.Name}', '{message}', '{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}');");
                    Player.SendMessageToAdmins($"{Color.IndianRed}[PLAYER REPORT] You'v got new report. Check /reports to take action.");
                    MessageDialog dialog = new MessageDialog("Warning",
                        $"We don't tolerate abusing /report command.\n" +
                        $"Make sure you reported right player for a broken rule.\n" +
                        $"{Color.IndianRed}Your report will be reviewed as soon as online admin will receive it.\n" +
                        $"Don't worry, your report will be saved after server restart.",
                        "Okay");
                    dialog.Show(player);
                }
                catch (Exception ee)
                {
                    Console.WriteLine(ee.Message);
                    player.SendClientMessage($"{Color.LightGray}Proper usage: /report [ID/Part of player name] [reason]");
                }
                return;
            }
            if (command == "reports")
            {
                if (player.AdminLevel == 0)
                {
                    player.SendClientMessage($"{Color.Red}No premissions.");
                    return;
                }
                TablistDialog reports = new TablistDialog("Unreviewed reports", new string[] { "Date\tReporter\tReported\tReason\n" }, "Review", "Close");
                MySqlDataReader reader = MySql.Reader("SELECT * FROM reports");
                string reason;
                long reportDate;
                string reporterName;
                string reportedName;

                while (reader.Read())
                {
                    reporterName = (string)reader["reporterName"];
                    reportedName = (string)reader["reportedName"];
                    reason = (string)reader["reason"];
                    reportDate = (long)reader["date"];
                    DateTimeOffset dateTime = DateTimeOffset.FromUnixTimeMilliseconds(reportDate);
                    reports.Add($"{(uint)reader["uid"]} {dateTime.Year}.{dateTime.Month}.{dateTime.Day} {dateTime.Hour}:{dateTime.Minute}\t{reporterName}\t{reportedName}\t{reason}\n");
                }
          
                if(reader.HasRows == false)
                {
                    reader.Close();
                    MessageDialog dialog = new MessageDialog("Info", "No reports for now man.", "Nice");
                    dialog.Show(player);
                    return;
                }
                reader.Close();
                reports.Show(player);
                reports.Response += (reportsObj, reportsArg) =>
                  {
                      if (reportsArg.DialogButton == DialogButton.Left)
                      {
                          uint reportUID = uint.Parse(reportsArg.InputText.Split(" ")[0]);
                          ListDialog options = new ListDialog(
                              $"{Color.White}Manage report {Color.IndianRed}#{reportUID}",
                              "Select", "Close");

                          options.Items.Add("1\tReview\n2\tRemove\n");
                          options.Show(player);

                          options.Response+=(optionsObj, optionsArg)=>
                          {
                              if(optionsArg.DialogButton == DialogButton.Left)
                              {
                                  if(optionsArg.ListItem == 0)
                                  {
                                      string reportedName = String.Empty;
                                      string reporterName = String.Empty;
                                      MySqlDataReader reader = MySql.Reader($"SELECT reportedName, reporterName FROM reports WHERE uid = '{reportUID}' LIMIT 1");
                                      while(reader.Read())
                                      {
                                          reportedName = (string)reader["reportedName"];
                                          reporterName = (string)reader["reporterName"];
                                      }
                                      
                                      reader.Close();
                                      foreach(Player pl in Player.All)
                                      {
                                          if(pl.Name == reportedName)
                                          {
                                              player.SpectatePlayer(pl);
                                              Player.SendMessageToAdmins($"{player.Name} is reviewing reported player: {reportedName}");        
                                          }
                                          if(pl.Name == reporterName)
                                          {
                                              pl.SendClientMessage($"{player.AdminColorHex}{player.StringAdminLevel} {player.Name} {Color.Yellow}is reviewing your report.");
                                          }
                                      }
                                      player.SendClientMessage($"{Color.LightGray}If reported player leaves server or is offline you can use /ologs to browse his logs offline.");
                                  }
                                  else if(optionsArg.ListItem == 1)
                                  {
                                      MySql.Query($"DELETE FROM reports WHERE uid = '{reportUID}' LIMIT 1;");
                                      player.GameText("~y~REPORT REMOVED", 3000, 4);
                                  }
                              }
                          };
                      }
                  };
                return;
            }
            if(command == "cmds" || command == "commands")
            {
                MessageDialog dialog = new MessageDialog("Avaliable commands for you:", 
                    "/pm, /lobby, /dm, /ooc, /a, /report, /commands, /cmds, /cuff, /r\n" +
                    "/lock, /id", "Close");
                if(player.AdminLevel > 0)
                {
                    dialog.Message+="\nAdmin commands:\n" +
                        "/ban, /spec, /kick, /oban, /reports, /logs, /ologs";
                }
                dialog.Show(player);
                return;
            }
            if(command == "cuff")
            {
                if (player.InPursuit == false)
                {
                    player.SendClientMessage($"{Color.LightGray}You are not in a pursuit.");
                    return;
                }
                if (player.IsSuspect)
                {
                    player.SendClientMessage($"{Color.LightGray}Only LSPD can use that command.");
                    return;
                }

            }
            if(command == "lock")
            {
                
            }
            if(command == "v")
            {
                if(player.AdminLevel == 0)
                {
                    player.SendClientMessage($"{Color.IndianRed}No permissions");
                    return;
                }
                // this always returns NULL. This is where I ended the project due to unknown bug.
                BaseVehicle vehicle = BaseVehicle.Create(SampSharp.GameMode.Definitions.VehicleModelType.AT400, new SampSharp.GameMode.Vector3(1431.6393f, 1519.5398f, 10.5988f), 0.0f, 1, 1);
                vehicle.LinkToInterior(1); // <- vehicle is NULL
                // ----
                return;
            }
            if (command == "r")
            {
                if(player.InPursuit==false)
                {
                    player.SendClientMessage($"{Color.LightGray}You are not in a pursuit.");
                    return;
                }
                if (player.IsSuspect)
                {
                    player.SendClientMessage($"{Color.LightGray}Only LSPD can use that command.");
                    return;
                }
                string message = String.Empty;
                for(byte i=1; i<args.Length; i++)
                    message+=" " + args[i];
                foreach(Player pl in Player.All)
                {
                    if(pl.InPursuit)
                    {
                        if(pl.IsSuspect == false)
                        {
                            pl.SendClientMessage("{6489e5}" + $"TAC 1 ** LSPD {player.Name}:{message} **");
                        }
                    }
                }
                return;
            }
            if (command == "id")
            {
                if(player.InPursuit==false)
                {
                    player.SendClientMessage($"{Color.LightGray}You are not in pursuit.");
                    return;
                }
                if(player.IsSuspect == false)
                {
                    player.SendClientMessage($"{Color.LightGray}You are not suspect.");
                    return;
                }
                Player target = Player.Find(args[1]);
                if(target == null)
                {

                    player.SendClientMessage($"{Color.LightGray}Player not found.");
                    return;
                }
                if (target.Logged == false)
                {

                    player.SendClientMessage($"{Color.LightGray}Player is not logged yet.");
                    return;
                }
                if(target.IsInRangeOfPoint(5.0f, player.Position) ==false)
                {
                    player.SendClientMessage($"{Color.LightGray}Player is too far.");
                    return;
                }

                string[] wantedReason = new string[]
                {
                    "Public exposure",
                    "Escaped from hospital with coronavirus",
                    "Playing Fortnite",
                    "Playing on Horizon Gaming",
                    "Stole 11'yo v-bucks",
                    "Doesn't pay taxes",
                    "Stole pope's hat and ate it"
                };
              
                MessageDialog dialog = new MessageDialog($"{player.Name}'s ID",
                    $"Name: {player.Name}\n"+
                    $"Age: {new Random().Next(17, 80)}"+
                    $"{Color.IndianRed}\nTHIS PLAYER IS WANTED, REASON:\n" +
                    $"{wantedReason[new Random().Next(0, wantedReason.Length)]}", "Yeah");
                dialog.Show(target);
                player.SetChatBubble($"* {player.Name}'s showing his ID to {target.Name} *", Color.LightPink, 10f, 5000);
                return;
            }
            player.SendClientMessage($"{Color.White}Command wasn't found. Check out {Color.LightBlue}/cmds{Color.White} to see all commands.");
            return;
        }
    }
}
