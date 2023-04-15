using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using System.Collections.Generic;

namespace EnableEmotes
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class EnableEmotesPlugin : BaseUnityPlugin
    {
        // The GUID of the project.
        internal const string GUID = "madhoek.EnableEmotes";
        // The NAME of the project.
        internal const string NAME = "EnableEmotes";
        // The name of the author.
        internal const string AUTHOR = "madhoek";
        // The VERSION of the project.
        internal const string VERSION = "1.0.0";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        internal static ManualLogSource Log;

        // Awake is called when your plugin is created. Use this to set up your mod.
        public void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"{NAME} {VERSION} loaded!");

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();

            CustomKeybindings.AddAction("Sit Emote", KeybindingsCategory.CustomKeybindings, ControlType.Keyboard);
            CustomKeybindings.AddAction("Arms Crossed Emote", KeybindingsCategory.CustomKeybindings, ControlType.Keyboard);
            CustomKeybindings.AddAction("Pick Berries Emote", KeybindingsCategory.CustomKeybindings, ControlType.Keyboard);
            CustomKeybindings.AddAction("Skin Animal Emote", KeybindingsCategory.CustomKeybindings, ControlType.Keyboard);

        }

        //this is where we listen for input then act on it
        [HarmonyPatch(typeof(LocalCharacterControl))]
        [HarmonyPatch("UpdateInteraction")]
        public static class Prefix_LocalCharacterControl_UpdateInteraction
        {
            static Dictionary<int, RewiredInputs> m_playerInputManager;
            static void Prepare()
            {
                m_playerInputManager = (Dictionary<int, RewiredInputs>)AccessTools.Field(typeof(ControlsInput), "m_playerInputManager").GetValue(null);
            }

            [HarmonyPrefix]
            public static bool PrefixUpdateInteraction(LocalCharacterControl __instance)
            {
                if (__instance.Character != null && __instance.Character.QuickSlotMngr != null)
                {
                    int playerID = __instance.Character.OwnerPlayerSys.PlayerID;

                    if (m_playerInputManager[playerID].GetButtonDown("Sit Emote"))
                    {
                        __instance.Character.CastSpell(Character.SpellCastType.Sit, __instance.Character.gameObject, Character.SpellCastModifier.Immobilized, 1, -1f);
                    }
                    else if (m_playerInputManager[playerID].GetButtonDown("Arms Crossed Emote"))
                    {
                        __instance.Character.CastSpell(Character.SpellCastType.IdleAlternate, __instance.Character.gameObject, Character.SpellCastModifier.Immobilized, 1, -1f);
                    }
                    else if (m_playerInputManager[playerID].GetButtonDown("Pick Berries Emote"))
                    {
                        __instance.Character.CastSpell(Character.SpellCastType.PickBerries, __instance.Character.gameObject, Character.SpellCastModifier.Immobilized, 1, -1f);
                    }
                    else if (m_playerInputManager[playerID].GetButtonDown("Skin Animal Emote"))
                    {
                        __instance.Character.CastSpell(Character.SpellCastType.SkinAnimal, __instance.Character.gameObject, Character.SpellCastModifier.Immobilized, 1, -1f);
                    }
                }
                return true;
            }
        }

    }
}
