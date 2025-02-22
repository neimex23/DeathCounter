using System;
using Microsoft.Xna.Framework;
using Monocle;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.DeathCounter
{
    [Tracked]
    public class DeathDisplay : Entity
    {
        private readonly Level _level;
        private AreaModeStats _stats;
        private readonly MTexture _skullIcon;
        private readonly Color textColor = Color.White;
        private readonly Color outlineColor = Color.Black;

        public DeathDisplay(Level level)
        {
            _level = level;
            
            _skullIcon = GFX.Gui["collectables/skullBlue"];
            Depth = -101;
            Tag = Tags.HUD | Tags.Global | Tags.PauseUpdate | Tags.TransitionUpdate;
        }

        public override void Update()
        {
            base.Update();
        }

        public static int TotalDeaths = 0;
        public static int DeathsInCurrentChapter = 0;

        public override void Render()
        {
            Vector2 iconPosition = new Vector2(10, 9);
            _skullIcon.Draw(iconPosition, Vector2.Zero, Color.White, 0.5f);

            if (SaveData.Instance != null)
            {
                TotalDeaths = SaveData.Instance.TotalDeaths;
            }

            _stats = SaveData.Instance.Areas_Safe.First(a => a.ID_Safe == _level.Session.Area.ID).Modes[(int)_level.Session.Area.Mode];
            if (_stats != null)
            {
                DeathsInCurrentChapter = _stats.Deaths;
            }

            int display = Main.Settings.ShowChapterDeaths ? DeathsInCurrentChapter : TotalDeaths;
            DrawText($"x {display}", (int)(iconPosition.X + _skullIcon.Width * 0.5f + 5), (int)iconPosition.Y);
        }

        private void DrawText(string text, int x, int y)
        {
            ActiveFont.DrawOutline(text, new Vector2(x, y), Vector2.Zero, new Vector2(0.8f, 0.8f), textColor, 1.5f, outlineColor); 
        }
    }
}