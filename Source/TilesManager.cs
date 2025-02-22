using System;
using Monocle;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.DeathCounter
{
    public static class EntityExtensions
    {
        public static bool IsAroundPosition(this Entity entity, Vector2 position)
        {
            return (Math.Abs((int)entity.Position.X - position.X) <= 80 && Math.Abs((int)entity.Position.Y - position.Y) <= 80);
        }
        public static bool HaveComponent(this Entity entity, String component)
        {
            foreach (Component componentToCheck in entity.Components)
            {
                if (componentToCheck.ToString() == component) return true;
            }
            return false;
        }
        public static int Actions(this Entity entity)
        {
            string filePath = "Mia/EntitiesID.txt";
            if (!File.Exists(filePath)) using (File.Create(filePath)) { }
            var lines = File.ReadAllLines(filePath);
            int j = 0;
            if (new FileInfo(filePath).Length != 0 && !int.TryParse(lines[lines.Count() - 1], out _))
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine("");
                }
            }
            if (entity is Solid || entity.HaveComponent("Celeste.PlayerCollider"))
            {
                while (j < lines.Length)
                {
                    if (lines[j][0] == '#')
                    {

                        j += 1;
                        continue;
                    }
                    if (lines[j] == entity.ToString())
                    {
                        try { return int.Parse(lines[j + 1]); }
                        catch (FormatException) { Console.WriteLine($"{lines[j + 1]} could not be converted to an integer."); }
                    }
                    j += 2;
                }
                //                Console.WriteLine(entity.ToString() + " have no ID. Please insert it manually in" + filePath);
                using (StreamWriter sw = File.AppendText(filePath))
                {

                    sw.WriteLine(entity.ToString());
                    sw.WriteLine("0");
                }
            }
            return 0;
        }
    
    }
    public static class PlageExtensions
    {
        public static void FillPlage<T>(this T[,] originalArray, int x, int y, int width, int height, T value)
        {
            int maxWidth = originalArray.GetLength(1);
            int maxHeight = originalArray.GetLength(0);
            for (int j = y; j < Math.Min(y + height + 1, maxHeight); j++)
            {
                for (int i = x; i < Math.Min(x + width + 1, maxWidth); i++)
                {
                    originalArray[i, j] = value;
                }
            }
        }
    }
    public class TileManager
    {
        public static int[,] GetEntityAroundPlayerAsTiles(Level level, Player player)
        {
            int[,] tilesAroundPlayer = new int[20, 20];
            for (int i = 0; i < level.Entities.Count; i++)
            {

                Entity entity = level.Entities[i];
                if (entity.IsAroundPosition(player.Position) && (entity.HaveComponent("Celeste.PlayerCollider") || entity.HaveComponent("Monocle.Image") || entity.HaveComponent("Celeste.LightOcclude") || entity is Solid))
                {
                    int entityXTiled = 10 + (int)entity.X / 8 - (int)player.Position.X / 8;
                    int entityYTiled = 10 + (int)entity.Y / 8 - (int)player.Position.Y / 8;
                    int entityTileHeight = (int)entity.Height / 8;
                    int entityTileWidth = (int)entity.Width / 8;
                    int UUID = entity.Actions();
                    tilesAroundPlayer.FillPlage(entityXTiled, entityYTiled, entityTileWidth, entityTileHeight, UUID);
                }
            }
            return tilesAroundPlayer;
        }
        public static int[,] GetTilesAroundPlayer(Level level, char[,] array, Player player)
        {
            int[,] tilesAroundPlayer = new int[20, 20];
            int playerXTile = (int)player.Position.X / 8; //Coordinates on player in tiles
            int playerYTile = (int)player.Position.Y / 8;
            int playerX = Math.Abs(level.TileBounds.X - playerXTile);
            int playerY = Math.Abs(level.TileBounds.Y - playerYTile);

            for (int j = playerY - 10; j < playerY + 10; j++)
            {
                for (int i = playerX - 10; i < playerX + 10; i++)
                {
                    int incrI = i - (playerX - 10);
                    int incrJ = j - (playerY - 10);
                    try
                    {
                        if (array[i, j] != '0') tilesAroundPlayer[incrI, incrJ] = 1; //Due to binary representation : Something is 0000000, nothing is 1111111, and there is 125 entities that can be stored. For now, I think it's more than enough.
                        else tilesAroundPlayer[incrI, incrJ] = 0;
                    }
                    catch (IndexOutOfRangeException) { tilesAroundPlayer[incrI, incrJ] = 1; }
                }
            }
            return tilesAroundPlayer;
        }

        public static int[,] FusedArrays(Level level, char[,] array, Player player)
        {
            int[,] entityArray = GetEntityAroundPlayerAsTiles(level, player);
            int[,] tilesArray = GetTilesAroundPlayer(level, array, player);

            int[,] globalTiles = new int[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {

                    if (entityArray[j, i] == 0) //There is no entity in that tile
                    {
                        globalTiles[j, i] = tilesArray[i, j];
                    }
                    else globalTiles[j, i] = entityArray[i, j];
                }
            }
            return globalTiles;
        }
    }
}