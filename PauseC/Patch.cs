using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PauseC
{
    public static class Patch
    {
        [HarmonyPatch(typeof(ADOBase), "GoToCalibration")]
        public static class CPressedEvent
        {
            public static bool Prefix()
            {
                if (Main.CPressed || Main.nocalibration || Main.listening)
                {
                    Main.CPressed = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(PauseMenu),"Update")]
        public static class UpdatePatch { 
            public static void Prefix()
            {
                if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.C))
                {
                    if (Main.setting.customkey.Down() && Main.setting.togglecustom)
                        return;
                    Main.CPressed = true;
                }
                if (Main.setting.customkey.Down() && Main.setting.togglecustom)
                {
                    ADOBase.GoToCalibration();
                }
            }
        }
    }
}
