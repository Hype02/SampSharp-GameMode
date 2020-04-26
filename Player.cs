using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode;
using SampSharp;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Definitions;
using System.Security.Cryptography;

namespace SAPC
{
    [PooledType]
    public class Player : BasePlayer
    {
        private string password;
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
            }
        }
        private byte adminLevel;
        public byte AdminLevel
        {
            get
            {
                return adminLevel;
            }
            set
            {
                adminLevel = value;
            }
        }
        private uint uid;
        public uint Uid
        {
            get
            {
                return uid;
            }
            set
            {
                uid = value;
            }
        }
        public bool IsRegistered
        {
            get
            {
                return uid > 0;
            }
        }
        private bool logged;
        public bool Logged
        {
            get
            {
                return logged;
            }
            set
            {
                logged = value;
            }
        }
        private long banExpiredate;
        public long BanExpireDate
        {
            get
            {
                return banExpiredate;

            }
            set
            {
                banExpiredate = value;
            }
        }
        private string banReason;
        public string BanReason
        {
            get
            {
                return banReason;
            }
            set
            {
                banReason = value;
            }
        }
        private string adminName;
        public string AdminName
        {
            get
            {
                return adminName;
            }
            set
            {
                adminName = value;
            }
        }
        private bool spectatingPlayer;
        public bool SpectatingPlayer
        {
            get
            {
                return spectatingPlayer;
            }
            set
            {
                spectatingPlayer = value;
            }
        }
        public string StringAdminLevel
        {
            get
            {
                if (this.adminLevel == Defines.AdminLevel.administrator)
                    return "Admin";
                if (this.adminLevel == Defines.AdminLevel.gamemaster)
                    return "GameMaster";
                if (this.adminLevel == Defines.AdminLevel.supporter)
                    return "Supporter";
                return "";
            }
        }
        private bool inPursuit;
        public bool InPursuit
        {
            get
            {
                return inPursuit;
            }
            set
            {
                inPursuit = value;
            }
        }
        private TextLabel nick;
        public TextLabel Nick
        {
            get
            {
                return nick;
            }
            set
            {
                nick = value;
            }
        }
        bool playingDeathMatch;
        public bool PlayingDeathMatch
        {
            get
            {
                return playingDeathMatch;
            }
            set
            {
                playingDeathMatch = value;
            }
        }
        public string AdminColorHex
        {
            get
            {
                if (this.adminLevel == Defines.AdminLevel.administrator)
                    return "{ff0000}";
                if (this.adminLevel == Defines.AdminLevel.gamemaster)
                    return "{6699ff}";
                if (this.adminLevel == Defines.AdminLevel.supporter)
                    return "{ff5c33}";
                return "{cccccc}";
            }
        }
        private float serverHealth;
        public float ServerHealth
        {
            get
            {
                return serverHealth;
            }
            set
            {
                serverHealth=value;
                base.Health = value;
            }
        }
        bool isDying;
        public bool IsDying
        {
            get
            {
                return isDying;
            }
            set
            {
                isDying = value;
            }
        }
        bool isSuspect;
        public bool IsSuspect
        {
            get
            {
                return isSuspect;
            }
            set
            {
                isSuspect = value;
            }
        }
        public void JoinDeathMatch()
        {
            this.PlayingDeathMatch = true;
            int randomNumber = new Random().Next(0, 8);
            Vector3[] spawnPoints = new Vector3[]
            {
                new Vector3(288.0302, 170.4629, 1007.1794),
                new Vector3(299.2405, 190.9509, 1007.1794),
                new Vector3(268.1711, 186.0975, 1008.1719),
                new Vector3(261.8717, 185.9882, 1008.1719),
                new Vector3(239.4634, 176.6972, 1003.0300),
                new Vector3(239.5177, 146.8661, 1003.0234),
                new Vector3(215.9001, 142.3453, 1003.0234),
                 new Vector3(288.745971, 169.350997, 1007.1718750)
            };
            this.Position = spawnPoints[randomNumber];
            this.GameText("~g~~h~~h~SPAWN", 3000, 4);
            this.serverHealth = 100.0f;
            this.GiveWeapon(Weapon.M4, 999999999);
            this.GiveWeapon(Weapon.Dildo, 999999999);
            this.GiveWeapon(Weapon.Deagle, 999999999);
            if(this.IsDying == true)
            this.IsDying = false;
        }
        public void LeaveDeathMatch()
        {
            this.playingDeathMatch = false;
            this.ClearAnimations();
            this.ResetWeapons();
            this.GameText("~r~~h~/DM to join back", 3000, 4);
            this.Position = new Vector3(-103.9699, -21.4087, 1000.7188);
            this.ServerHealth = 100.0f;
        }
        public void UpdateDatabaseData()
        {
            MySql.Query(@$"UPDATE accounts SET
            score ='{Score}',
            adminLevel = '{AdminLevel}',
            adminName='{AdminName}',
            banExpireDate='{BanExpireDate}',
            banReason='{BanReason}'
            WHERE uid = '{Uid}' LIMIT 1;");
        }
        public void ClearChat()
        {
            for (byte i = 1; i <= 20; i++)
                SendClientMessage("");
        }
        public void Register()
        {
            MySql.Query(@$"INSERT INTO accounts
            (name,
            adminLevel,
            score,
            password) VALUES
            ('{Name}',
            '{AdminLevel}',
            '{Score}', 
              '{Password}');");
        }
        public void LoadData()
        {
            var reader = MySql.Reader($"SELECT * FROM accounts WHERE name = '{Name}' LIMIT 1;");
            while (reader.Read())
            {
                this.password = (string)reader["password"];
                this.Score = (int)reader["score"];
                this.AdminLevel = (byte)reader["adminLevel"];
                this.Uid = (uint)reader["uid"];
                this.BanExpireDate = (long)reader["banExpireDate"];
                this.BanReason = (string)reader["banReason"];
                this.AdminName = (string)reader["adminName"];
            }
            reader.Close();
        }
        public void ShowBanInfo()
        {
            DateTimeOffset dateNow = DateTimeOffset.Now;
            DateTimeOffset expireDate = DateTimeOffset.FromUnixTimeMilliseconds(this.BanExpireDate);

            int days = expireDate.Day - dateNow.Day;
            MessageDialog message = null;
            if (days == 0)
            {
                message = new MessageDialog($"{Color.Red}You'v been banned.",
                   "Admin: " + this.Name + "\n" +
                   $"Ban expires today.\n" +
                   $"{Color.LightYellow}{this.BanReason}",
                   "Close");
            }
            else
            {
                message = new MessageDialog($"{Color.Red}You'v been banned.",
             "Admin: " + this.Name + "\n" +
             $"Ban for {days} days\n" +
             $"{Color.LightYellow}{this.BanReason}",
             "Close");
            }


            message.Show(this);
        }
        public void ShowLoginDialog()
        {
            InputDialog loginDialog = new InputDialog($"Hello, {this.Name}!",
                    "Your account exists.\n" +
                    "Please insert password to your account below:", true,
                    "Login", "Quit");
            loginDialog.Show(this);
            loginDialog.Response += (loginDialogSender, loginDialogArg) =>
            {
                if (loginDialogArg.DialogButton == DialogButton.Right)
                {
                    this.Kick();
                }
                else
                {
                    SHA256 sha256 = SHA256.Create();
                    byte[] inputPasswordBytes = Encoding.UTF8.GetBytes(loginDialogArg.InputText);
                    byte[] hashInputPasswordBytes = sha256.ComputeHash(inputPasswordBytes);
                    string hashedInputPassword = BitConverter.ToString(hashInputPasswordBytes).Replace("-", String.Empty);
                    if (hashedInputPassword == this.Password)
                    {
                        if (this.banExpiredate > DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                        {
                            this.ShowBanInfo();
                            this.Kick();
                            return;
                        }
                        this.ToggleSpectating(false); // will call "OnPlayerRequestClass" now   
                        foreach (Player player in Player.All)
                        {
                            if (player.inPursuit == false)
                            {
                                if (player.logged)
                                {
                                    player.SendClientMessage($"{Color.GreenYellow}[JOIN] {this.Name} logged to the game!");
                                }
                            }
                        }
                        this.logged = true;
                        this.Color = Defines.NickColor.gray;
                        this.Nick = new TextLabel($"{this.AdminColorHex}{this.StringAdminLevel} {this.Name} ({this.Id})", this.Color, new Vector3(0, 0, 0), 10f);
                        this.nick.AttachTo(this, new Vector3(0, 0, 0.2));
                        this.ClearChat();
                        this.SendClientMessage($"{Color.White}Hello, {Color.LightBlue}{this.Name}{Color.White}! Use {Color.GreenYellow}/dm{Color.White} if you're bored waiting for the game.");
                    }
                    else
                    {
                        loginDialog.Show(this);
                        this.SendClientMessage($"{Color.Red}Sorry, but your password is incorrect.");
                    }
                }
            };
        }
        public void BanPlayer(byte days, string reason, string adminName)
        {
            this.BanReason = reason;
            this.AdminName = adminName;
            DateTimeOffset banExpirationDate = DateTimeOffset.UtcNow;
            banExpirationDate = banExpirationDate.AddDays(days);
            this.BanExpireDate = banExpirationDate.ToUnixTimeMilliseconds();

            this.ShowBanInfo();
            Player.SendClientMessageToAll($"{Color.IndianRed}AdmCmd: {adminName} banned {this.Name} for {days} days. Reason: {reason}");
            this.Kick();
        }
        public override void Kick()
        {
            new Timer(1000, false).Tick += TimerTick;
            void TimerTick(object sender, EventArgs e)
            {
                if (this.IsDisposed == false)
                    base.Kick();
            }
        }
        public void Kick(string reason = "")
        {
            foreach (Player player in Player.All)
            {
                if (player.logged)
                {
                    player.SendClientMessage(reason);
                }
            }
            new Timer(1000, false).Tick += TimerTick;
            void TimerTick(object sender, EventArgs e)
            {
                if (this.IsDisposed == false)
                    base.Kick();
            }
        }
        public override void SpectatePlayer(BasePlayer targetPlayer)
        {
            this.ToggleSpectating(true);
            this.Interior = targetPlayer.Interior;
            this.VirtualWorld = targetPlayer.VirtualWorld;
            this.SpectatingPlayer = true;
            if (targetPlayer.InAnyVehicle)
                base.SpectateVehicle(targetPlayer.Vehicle);
            else
                base.SpectatePlayer(targetPlayer);
        }
        public override void ToggleSpectating(bool toggle)
        {
            if (toggle == false)
                if (this.spectatingPlayer == true)
                    this.spectatingPlayer = false;
            base.ToggleSpectating(toggle);

        }
        public static Player Find(string playerName)
        {
            bool isNumeric = int.TryParse(playerName, out _);
            if (isNumeric)
                return BasePlayer.Find(int.Parse(playerName)) as Player;
            byte GetSimiliarityCount(string targetName, ref string playerName)
            {
                byte count = 0;
                for (int i = 0; i < playerName.Length; i++)
                    if (targetName.ToLower()[i] == playerName.ToLower()[i])
                        count++;
                    else
                        break;
                return count;
            }
            Player bestSimiliarPlayer = null;
            byte bestSimiliarLetterCount = 0;
            foreach (Player player in Player.All)
            {
                if (GetSimiliarityCount(player.Name, ref playerName) > bestSimiliarLetterCount)
                {
                    bestSimiliarLetterCount = GetSimiliarityCount(player.Name, ref playerName);
                    bestSimiliarPlayer = player;
                }
            }
            return bestSimiliarPlayer;
        }
        public static void SendMessageToAdmins(string message)
        {
            foreach (Player player in Player.All)
            {
                if (player.Logged)
                {
                    if (player.AdminLevel > 0)
                    {
                        player.SendClientMessage($"{Color.IndianRed}AdmMsg: {message}");
                        player.PlaySound(5205);
                    }
                }
            }
        }
    }
}
