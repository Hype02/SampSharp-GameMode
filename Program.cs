using SampSharp.Core;
using SampSharp.GameMode;
using SampSharp.GameMode.World;
using SampSharp.Core.Logging;
using System;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using System.Security.Cryptography;
using System.Text;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.SAMP;
using System.Web;

namespace SAPC
{
    public class Program
    {
        private static void Main()
        {
            new GameModeBuilder()
                .Use<GameMode>()
                .UseLogLevel(CoreLogLevel.Info)
                .UseEncoding("C:/sa-mp/serwer/env/codepages/cp1250.txt")
                .Run();
        }
    }
    public partial class GameMode : BaseMode
    {
        protected override void OnInitialized(EventArgs e)
        {
            MySql.Init();
            ServerInfo serverInfoInstance = new ServerInfo();
            serverInfoInstance.Init();
            new GameModeTimer().Init();
            base.OnInitialized(e);
        }
        protected override void OnPlayerRequestClass(BasePlayer sender, RequestClassEventArgs e)
        {
            base.OnPlayerRequestClass(sender, e);
            Player player = sender as Player;
            player.Interior = 3;
            player.SetSpawnInfo(0, 285, new Vector3(-103.9699, -21.4087, 1000.7188), 356.8667f);
            player.Team = 0;
            player.Spawn();
            player.ServerHealth = 100.0f;
        }
        protected override void OnPlayerConnected(BasePlayer sender, EventArgs e)
        {
            base.OnPlayerConnected(sender, e);
            sender.Color = Defines.NickColor.black;
            Player player = sender as Player;
            player.ToggleSpectating(true); // after setting to "false" will spawn player. We'll do it after we login succefully.
            player.ClearChat();
            player.LoadData();
            if(player.IsRegistered)
            {
                player.ShowLoginDialog();
            }
            else
            {
                InputDialog registerDialog = new InputDialog($"Hello, {sender.Name}!",
                   "Your account can be registered.\n" +
                   "Please insert password below to create new account.", true,
                   "Create", "Quit");
                registerDialog.Show(player);
                registerDialog.Response+=(registerDialogObject, registerDialogArg)=>
                {
                    if(registerDialogArg.DialogButton == DialogButton.Right)
                        player.Kick();
                    else
                    {
                        if(registerDialogArg.InputText.Length > 32 || registerDialogArg.InputText.Length < 8)
                        {
                            registerDialog.Show(player);
                            player.SendClientMessage($"{Color.Red}Password must be within 8-32 characters.");
                        }
                        else
                        {
                             char[] allowedCharacters = new char[]{
                            'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd',
                            'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x','c', 'v', 'b', 'n', 'm',
                             '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'};
                             bool IsAllowedCharacter(char character)
                            {
                                for (int j = 0; j < allowedCharacters.Length; j++)
                                {
                                    if (character == allowedCharacters[j])
                                    {
                                        return true;
                                    }
                                }
                                return false;
                            }
                             for(int i=0; i< registerDialogArg.InputText.Length; i++)
                            {
                                if(IsAllowedCharacter(registerDialogArg.InputText[i])==false)
                                {
                                    player.SendClientMessage($"{Color.Red}Your password can contain only characters A-Z and 0-9.");
                                    registerDialog.Show(player);
                                    return;
                                }
                            }
                             byte[] passwordBytes = Encoding.UTF8.GetBytes(registerDialogArg.InputText);
                             SHA256 sh256 = SHA256.Create();
                             byte[] hashBytes = sh256.ComputeHash(passwordBytes);
                             string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
                             player.Password = hashedPassword;
                             player.Register();
                             player.ShowLoginDialog();
                             player.SendClientMessage($"{Color.Yellow}Account registered! Now login with your password.");
                       
                        }
                    }
                };
            }
        }
        protected override void OnPlayerDisconnected(BasePlayer sender, DisconnectEventArgs e)
        {
            base.OnPlayerDisconnected(sender, e);
            Player player = sender as Player;
            if(player.Logged)
            {
                player.UpdateDatabaseData();
                player.Nick.Dispose();
            }
        }
        protected override void OnPlayerTakeDamage(BasePlayer sender, DamageEventArgs e)
        {
            base.OnPlayerTakeDamage(sender, e);
            Player player = sender as Player;
            if(player.Logged)
            {
                float damage = 0f;
                if(e.Weapon == Weapon.AK47)
                    damage = 10.0f;
                else if (e.Weapon == Weapon.Bat)
                    damage = 20.0f;
                else if (e.Weapon == Weapon.Colt45)
                    damage = 10.0f;
                else if (e.Weapon == Weapon.Deagle)
                    damage = 49.0f;
                else if (e.Weapon == Weapon.Dildo)
                    damage = 25.0f;
                else if (e.Weapon == Weapon.Explosion)
                    damage = 50.0f;
                else if (e.Weapon == Weapon.HelicopterBlades)
                    damage = 100.0f;
                else if (e.Weapon == Weapon.MP5)
                    damage = 5.0f;
                else if (e.Weapon == Weapon.M4)
                    damage = 10.0f;

                if(e.OtherPlayer != null)
                {
                    if (player.ServerHealth - damage <= 0)
                    {
                        Player otherPlayer = e.OtherPlayer as Player;
                        player.IsDying = true;
                        if (player.PlayingDeathMatch)
                        {
                            player.ApplyAnimation("ped", "KO_shot_face", 4.1f, false, true, true, true, 0, true);
                            new Timer(5000, false).Tick+=DeathMatchRespawn;
                            player.GameText($"~r~~h~~h~{otherPlayer.Name} has killed you", 3000, 4);
                            void DeathMatchRespawn(object sender, EventArgs args)
                            {
                                if(player.IsDisposed == false)
                                  player.JoinDeathMatch();
                            }
                        }
                        else if(player.InPursuit)
                        {

                        }
                    }
                    else
                    {
                        if(player.IsDying == false)
                          player.ServerHealth-=damage;
                    }
                }
                else
                {
                    player.ServerHealth -= damage;
                }
            }
        }
        protected override void OnPlayerDied(BasePlayer sender, DeathEventArgs e)
        {
            base.OnPlayerDied(sender, e);
            Player player = sender as Player;
            player.LeaveDeathMatch();
            player.Spawn();
        }
    }
}