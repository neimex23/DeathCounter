using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Celeste.Mod.DeathCounter
{
    internal class Utils
    {
        public static string GetInputs()
        {
            int[] inputs =
            {
                Input.MoveX.Value == -1 ? 1 : 0,
                Input.MoveX.Value == 1 ? 1 : 0,
                Input.MoveY.Value == 1 ? 1 : 0,
                Input.MoveY.Value == -1 ? 1 : 0,
                Input.Grab.Check ? 1 : 0,
                Input.DashPressed ? 1 : 0,
                Input.Jump.Check ? 1 : 0
            };
            return string.Join("", inputs);
        }

        public static string ConvertToString(int[,] array)
        {
            return string.Concat(array.Cast<int>().Select(o => o.ToString()));
        }
    }
}
