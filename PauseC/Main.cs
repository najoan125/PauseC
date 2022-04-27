using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace PauseC
{
    public static class Main
    {
        public static Setting setting;
        public static UnityModManager.ModEntry.ModLogger Logger;
        public static Harmony harmony;

        public static bool IsEnabled = false;
        public static bool CPressed = false;
        public static bool listening = false;
        public static bool nocalibration = false;

        public static void Setup(UnityModManager.ModEntry modEntry)
        {
            Logger = modEntry.Logger;
            modEntry.OnToggle = OnToggle;
            setting = new Setting();
            setting = UnityModManager.ModSettings.Load<Setting>(modEntry);
        }

        private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            IsEnabled = value;
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            if (value)
            {
                harmony = new Harmony(modEntry.Info.Id);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            else
            {
                harmony.UnpatchAll(modEntry.Info.Id);
            }
            return true;
        }





        private static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            bool togglecustom = GUILayout.Toggle(setting.togglecustom, RDString.language == SystemLanguage.Korean ? "사용자 정의 보정 키 켜기" : "Enable Custom Calibration Key");
            if (togglecustom)
            {
                setting.togglecustom = true;
                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                GUILayout.Label(RDString.language == SystemLanguage.Korean ? "사용자 정의 보정 키: " + setting.customkey.keyCode.ToString() : "Custom Calibraion Key: " + setting.customkey.keyCode.ToString());
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(30);
                if(GUILayout.Button(RDString.language == SystemLanguage.Korean ? "사용자 정의 보정 키 바꾸기" : "Change Custom Calibraion Key"))
                {
                    if (!listening)
                        listening = true;
                }
                GUILayout.EndHorizontal();
                if (listening)
                {
                    GUILayout.Label(RDString.language == SystemLanguage.Korean ? "입력 감지 중입니다... 사용자 정의 보정 키를 입력하세요(취소하시려면 마우스 좌클릭을 누르세요)" : "Detecting input... Enter a custom calibration key(To cancel, click the left mouse button)");
                    if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
                        listening = false;
                    else if (!UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Mouse0))
                    {
                        try
                        {
                            foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
                            {
                                if (Input.GetKey(kcode))
                                {
                                    setting.customkey.keyCode = kcode;
                                    nocalibration = true;
                                    listening = false;
                                }
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            else
            {
                setting.togglecustom = false;
            }
            if (UnityEngine.Input.GetKeyUp(setting.customkey.keyCode) && nocalibration)
            {
                nocalibration = false;
            }
        }
        private static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            setting.Save(modEntry);
        }
    }
}
