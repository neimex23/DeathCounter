using System;
using System.Data;
using System.Net.NetworkInformation;
using System.Linq;
using Monocle;
using System.Reflection;

namespace Celeste.Mod.DeathCounter
{
    public class Main : EverestModule
    {
        public static Main Instance { get; private set; }
        public static DeathCounterSettings Settings => (DeathCounterSettings)Instance._Settings;
        public override Type SettingsType => typeof(DeathCounterSettings);


        public Main()
        {
            Instance = this;
        }

        public override void Load()
        {
            if (_Settings == null)
            {
                _Settings = new DeathCounterSettings();
            }
            On.Celeste.Level.LoadLevel += LoadLevel;
            On.Celeste.Player.Update += PlayerUpdate;
        }

        public override void Unload()
        {
            On.Celeste.Level.LoadLevel -= LoadLevel;
            On.Celeste.Player.Update -= PlayerUpdate;
        }

        private void LoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            if (isFromLoader)
            {
                self.Add(new DeathDisplay(self));
            }
        }

        private void PlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            if (Engine.Scene is Level level)
            {
                var position = $"{self.Position.X}, {self.Position.Y}";
                Console.WriteLine($"Player position: {position}");

                string inputs = Utils.GetInputs();
                Console.WriteLine($"Player inputs: {inputs}");
            }
        }

        public static int GetTotalPlayerDeaths()
        {
            return SaveData.Instance.TotalDeaths;
        }

    }
}
