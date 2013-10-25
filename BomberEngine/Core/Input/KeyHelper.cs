using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Core.Input
{
    public class KeyHelper
    {
        private static readonly KeyCode[] ConfirmKeys = 
        {
            KeyCode.Enter,
            KeyCode.GP_Start,
            KeyCode.GP_A
        };

        private static readonly KeyCode[] CancelKeys =
        {
            KeyCode.Escape,
            KeyCode.GP_Back,
            KeyCode.GP_B
        };

        public static bool IsConfirmKey(KeyCode code)
        {
            return ConfirmKeys.Contains(code);
        }

        public static bool IsCancelKey(KeyCode code)
        {
            return CancelKeys.Contains(code);
        }
    }
}
