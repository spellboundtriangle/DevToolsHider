using BoneLib.BoneMenu;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(DevToolsHider.DevToolsHiderMod), "DevToolsHider", "0.0.1", "triangle", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace DevToolsHider
{
    public class DevToolsHiderMod : MelonMod
    {
        public static MelonPreferences_Category DevToolsHider_Category;
        public static MelonPreferences_Entry<bool> DevToolsHidden;
        public const Int32 TargetLayer = 11; // Layer 11 is the target because it is unused in BONELAB as of Patch 6
        
        public override void OnInitializeMelon()
        {
            // Init MelonLoader preferences

            DevToolsHider_Category = MelonPreferences.CreateCategory("Dev Tools Hider");

            // Init BoneMenu elements

            BoneLib.BoneMenu.Page BoneMenu_MainPage = BoneLib.BoneMenu.Page.Root.CreatePage("Dev Tools Hider", Color.Lerp(Color.blue, Color.white, 0.7f)); // Light blue? maybe
            {
                BoneMenu_MainPage.CreateBoolPref("Enabled", Color.cyan, ref DevToolsHidden, UpdateSpectatorCameraSettings, prefDefaultValue: false);
            }
            LoggerInstance.Msg("Initialized.");
        }
        // BoneMenu methods
        public static void UpdateSpectatorCameraSettings(bool value)
        {
            var currentCamera = GameObject.Find("/GameplaySystems [0]/DisabledContainer/Spectator Camera/Spectator Camera").GetComponent<Camera>();
            if (value)
            {
                currentCamera.cullingMask = RemoveLayer(currentCamera.cullingMask, TargetLayer);
                return;
            }
            currentCamera.cullingMask = ShowLayer(currentCamera.cullingMask, TargetLayer);
        }
        public static int ShowLayer(int mask, int layer)
        {
            return mask |= (1 << layer);
        }
        public static int RemoveLayer(int mask, int layer)
        {
            return mask &= ~(1 << layer);
        }
        public static void SavePreferences()
        {
            DevToolsHider_Category.SaveToFile(false);
        }
    }

    [HarmonyLib.HarmonyPatch]
    public class Patches
    {
        // Switch layers
        [HarmonyLib.HarmonyPatch(typeof(SpawnGun), "Start")]
        [HarmonyLib.HarmonyPostfix]
        public static void SetSpawnGunLayers(SpawnGun __instance) 
        {
            SwitchRendererLayers(__instance);
        }

        [HarmonyLib.HarmonyPatch(typeof(FlyingGun), "Awake")]
        [HarmonyLib.HarmonyPostfix]
        public static void SetSpawnGunLayers(FlyingGun __instance)
        {
            SwitchRendererLayers(__instance);
        }

        public static void SwitchRendererLayers(Component __instance)
        {
            var Renderers = __instance.gameObject.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer element in Renderers)
            {
                element.gameObject.layer = DevToolsHiderMod.TargetLayer;
            }
        }

        // Update spectator camera setting on load
        [HarmonyLib.HarmonyPatch(typeof(RigScreenOptions), "Start")]
        [HarmonyLib.HarmonyPostfix]
        public static void OnLoadedGameplaySystems(RigScreenOptions __instance)
        {
            DevToolsHiderMod.UpdateSpectatorCameraSettings(DevToolsHiderMod.DevToolsHidden.Value);
        }


    }
}