using SampSharp.GameMode;
namespace SAPC
{
    public class ServerInfo : BaseMode
    {
        public void Init()
        {
            SetGameModeText("Police Chase");
            SendRconCommand("hostname [0.3-DL] ls-pursuits | Cop Chase Server");
            SendRconCommand("weburl https://discord.gg/pqQ8cbw");
            SendRconCommand("language English");
            EnableStuntBonusForAll(false);
            DisableInteriorEnterExits();
            ShowNameTags(false);
        }
    }
}
