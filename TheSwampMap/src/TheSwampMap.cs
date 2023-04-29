using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using SideLoader;

namespace MadHoekSwampMap
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class TheSwampMap : BaseUnityPlugin
    {
        internal const string GUID = "madhoek.TheSwampMap";
        internal const string NAME = "TheSwampMap";
        internal const string AUTHOR = "madhoek";
        internal const string VERSION = "1.0.1";

        internal static ManualLogSource Log;

        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"{NAME} {VERSION} loaded!");

            new Harmony(GUID).PatchAll();
        }

        [HarmonyPatch(typeof(Area), nameof(Area.GetMapScreen))]
        public class Area_GetMapScreen
        {
            [HarmonyPrefix]
            static bool GetMapscreen(Area __instance, ref Sprite __result)
            {
                var pack = SL.GetSLPack("madhoek-newswampmap");
                var bundle = pack.AssetBundles["swampmap"];
                string name = "tex_men_mapNewHallowedMarsh.png";

                if (SceneManagerHelper.ActiveSceneName == "HallowedMarshNewTerrain")
                {
                  __result = bundle.LoadAsset<Sprite>(name);
                    return false;
                }
                if (SceneManagerHelper.ActiveSceneName != "HallowedMarshNewTerrain")
                {
                   __result = ResourcesPrefabManager.Instance.GetAreaMap(__instance.m_mapScreenPath);
                }
                if (__result == null)
                {
                    __result = __instance.m_splashScreen_DEPRECATED;
                }
                return true;
            }

        }
    }
}
