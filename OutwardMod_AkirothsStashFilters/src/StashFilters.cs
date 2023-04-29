using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using UnityEngine;

namespace StashFilters
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class StashFiltersPlugin : BaseUnityPlugin
    {
        internal const string GUID = "madhoek.akirothStashFilters";

        internal const string NAME = "Akiroth's StashFilters";

        internal const string AUTHOR = "madhoek";

        internal const string VERSION = "1.0.2";

        public static StashFiltersPlugin Instance;

        internal static ManualLogSource Log;

        internal void Awake()
        {
            StashFiltersPlugin.Instance = this;
            Log = this.Logger;
            Log.LogMessage($"{NAME} {VERSION} loaded!");

            new Harmony(GUID).PatchAll();
        }

        // Code from StashCraft by Akiroth
        // Fix ambiguous reference Error: Object -> GameObject
        public static void MakeStashFilter(Character Char)
        {
            try
            {
                if (Char.CharacterUI.StashPanel.m_sectionPanel == null)
                {
                    InventorySectionDisplay sectionPanel = Char.CharacterUI.InventoryPanel.m_sectionPanel;
                    InventorySectionButton[] componentsInChildren = sectionPanel.GetComponentsInChildren<InventorySectionButton>();
                    for (int i = 0; i < componentsInChildren.Length; i++)
                    {
                        componentsInChildren[i].StartInit();
                    }
                    InventorySectionDisplay sectionPanel2 = GameObject.Instantiate<InventorySectionDisplay>(sectionPanel, Char.CharacterUI.StashPanel.m_inventoryDisplay.transform, false);
                    Char.CharacterUI.StashPanel.m_sectionPanel = sectionPanel2;
                    Char.CharacterUI.StashPanel.m_stashInventory.SetFilter(Char.CharacterUI.StashPanel.m_inventoryDisplay.Filter);
                    Char.CharacterUI.StashPanel.m_stashInventory.SetExceptionFilter(Char.CharacterUI.StashPanel.m_inventoryDisplay.ExceptionFilter);
                }
                Char.CharacterUI.StashPanel.m_stashInventory.Refresh();
            }
            catch (Exception ex)
            {
                StashFiltersPlugin.Log.LogError("MakeStashFilter: " + ex.Message);
            }
        }

        // Code from StashCraft by Akiroth
        // removed because of bad performance: if (NetworkLevelLoader.Instance.AllPlayerReadyToContinue)
        [HarmonyPatch(typeof(StashPanel), "Show")]
        public class StashPanel_Show
        {
            [HarmonyPrefix]
            public static bool Show(StashPanel __instance)
            {
                StashFiltersPlugin.MakeStashFilter(__instance.LocalCharacter);
                return true;
            }
        }

        // Code from StashCraft by Akiroth
        [HarmonyPatch(typeof(InventorySectionButton), "OnToggleChanged")]
        public class InventorySectionButton_OnToggleChanged
        {
            [HarmonyPostfix]
            public static void OnToggleChanged(InventorySectionButton __instance, bool _checked)
            {
                try
                {
                    // [Error  :StashFiltersPlugin] InventorySectionButton_OnToggleChanged: Object reference not set to an instance of an object
                    // Fix NullReferenceException: StashPanel -> StashPanel?
                    __instance.CharacterUI.StashPanel?.m_stashInventory.Refresh();
                    
                }
                catch (Exception ex)
                {
                    StashFiltersPlugin.Log.LogError("InventorySectionButton_OnToggleChanged: " + ex.Message);
                }
            }
        }

    }
}