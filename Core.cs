using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;
using Il2CppSLZ.Marrow.Interaction;
using MenuHelper;

[assembly: MelonInfo(typeof(DevToolsHider.DevToolsHiderMod), "DevToolsHider", "0.0.1", "triangle", null)]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace DevToolsHider
{
    public class DevToolsHiderMod : MelonMod
    {
        public static MelonPreferences_Category DevToolsHider_Category;
        public static MelonPreferences_Entry<bool> DevToolsHidden;
        public static MelonPreferences_Entry<bool> EnableInFlatPlayer;
        public const Int32 DefaultLayer = 0;         // Layer 0, identified as Default, is the default layer and is generally used for level collision
        public const Int32 FixtureLayer = 6;         // Layer 6, identified as Fixture, is the layer for fixtures (airlock doors, buttons, etc)
        public const Int32 PlayerLayer = 8;          // Layer 8, identified as Player, is the layer for the player's collision
        public const Int32 DynamicLayer = 10;        // Layer 10, identified as Dynamic, is the layer intended for dynamic object collision in BONELAB as of Patch 6. This isn't typically used for rendering, but it's possible that the user placed colliders on the renderers
        public const Int32 HideableLayer = 11;       // Layer 11 is the target for hiding objects
        public const Int32 EnemyCollidersLayer = 12; // Layer 12, identified as EnemyColliders, is the layer used for NPC collision
        public const Int32 FootballLayer = 24;       // Layer 24, identified as Football, is the layer used for the player locosphere
        public static bool IsFlatPlayer;
        public static Color SpawnGunBlue = new(0.06432372f, 0.7570404f, 0.856f, 1f);
        

        public override void OnInitializeMelon()
        {
            // Init MelonLoader preferences

            DevToolsHider_Category = MelonPreferences.CreateCategory("Dev Tools Hider");

            // Init BoneMenu elements

            BoneLib.BoneMenu.Page BoneMenu_MainPage = BoneLib.BoneMenu.Page.Root.CreatePage("Dev Tools Hider", SpawnGunBlue);
            {
                BoneMenu_MainPage.CreateBoolPref("Hide in Spectator", SpawnGunBlue, ref DevToolsHidden, UpdateSpectatorCameraSettings, prefDefaultValue: true);
                if (!BoneLib.HelperMethods.IsAndroid()) // This settings are useless on Quest as it doesn't have a spectator camera
                {
                    BoneMenu_MainPage.CreateBoolPref("Hide in FlatPlayer Spectator", SpawnGunBlue, ref EnableInFlatPlayer, UpdateSpectatorCameraSettings, prefDefaultValue: false, tooltip: "Hide in Spectator must be enabled for this to have any effect.");
                }
                BoneMenu_MainPage.CreateFunction("Toggle Held Items Visibility", SpawnGunBlue, ToggleHeldItemVisibility);
                BoneMenu_MainPage.CreateFunction("Toggle Held Camera Mask", SpawnGunBlue, ToggleHeldCameraMask);
            }
            Physics.IgnoreLayerCollision(HideableLayer, DefaultLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, FixtureLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, PlayerLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, DynamicLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, HideableLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, EnemyCollidersLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, FootballLayer, false);
            LoggerInstance.Msg("Initialized.");
        }
        public override void OnLateInitializeMelon()
        {
            // Check for FlatPlayer
            if (FindMelon("FlatPlayer", "LlamasHere") != null)
            {
                IsFlatPlayer = true;
            }
            else
            {
                IsFlatPlayer = false;
            }
        }

        // BoneMenu methods
        public static void ToggleHeldItemVisibility()
        {
            int Target;
            GameObject[] HandItems = [BoneLib.Player.GetObjectInHand(BoneLib.Player.LeftHand), BoneLib.Player.GetObjectInHand(BoneLib.Player.RightHand)];
            foreach (var HandItemElement in HandItems)
            {
                if (HandItemElement != null && HandItemElement.GetComponentInParent<MarrowBody>() != null)
                {
                    var Renderers = HandItemElement.GetComponentInParent<MarrowEntity>(true).GetComponentsInChildren<Renderer>(true);
                    if (Renderers.Length != 0 && Renderers.First().gameObject.layer != HideableLayer)
                    {
                        Target = HideableLayer;
                    }
                    else
                    {
                        Target = DynamicLayer;
                    }
                    foreach (Renderer RendererElement in Renderers)
                    {
                        RendererElement.gameObject.layer = Target;
                    }
                }
            }
        }
        public static void ToggleHeldCameraMask() // Toggles the mask found under a held camera device
        {
            GameObject[] HandItems = [BoneLib.Player.GetObjectInHand(BoneLib.Player.LeftHand), BoneLib.Player.GetObjectInHand(BoneLib.Player.RightHand)];
            foreach (var HandItemElement in HandItems)
            {
                if (HandItemElement != null && HandItemElement.GetComponentInParent<MarrowBody>() != null)
                {
                    foreach (var ItemCamera in HandItemElement.GetComponentInParent<MarrowEntity>(true).GetComponentsInChildren<Camera>())
                    {
                        var CameraCullingMask = ItemCamera.cullingMask;
                        if (RemoveLayer(ItemCamera.cullingMask, HideableLayer) == ItemCamera.cullingMask)
                        {
                            ItemCamera.cullingMask = ShowLayer(ItemCamera.cullingMask, HideableLayer);
                        }
                        else
                        {
                            ItemCamera.cullingMask = RemoveLayer(ItemCamera.cullingMask, HideableLayer);
                        }
                    }
                }
            }
        }

        public static void UpdateSpectatorCameraSettings(bool value)
        {
            var currentCamera = GameObject.Find("/GameplaySystems [0]/DisabledContainer/Spectator Camera/Spectator Camera").GetComponent<Camera>();
            if (DevToolsHidden.Value && (!IsFlatPlayer || EnableInFlatPlayer.Value))
            {
                currentCamera.cullingMask = RemoveLayer(currentCamera.cullingMask, HideableLayer);
                return;
            }
            currentCamera.cullingMask = ShowLayer(currentCamera.cullingMask, HideableLayer);
        }

        public static int ShowLayer(int mask, int layer)
        {
            return mask |= (1 << layer);
        }
        public static int RemoveLayer(int mask, int layer)
        {
            return mask &= ~(1 << layer);
        }
    }

    [HarmonyLib.HarmonyPatch]
    public class Patches
    {
        // Switch layers of Spawn Gun and Nimbus Gun renderers
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
                element.gameObject.layer = DevToolsHiderMod.HideableLayer;
            }
        }

        // Update spectator camera setting on load. This is done instead of hooking level load because this caused an error, possibly being called before GameplaySystems being spawned?
        [HarmonyLib.HarmonyPatch(typeof(RigScreenOptions), "Start")]
        [HarmonyLib.HarmonyPostfix]
        public static void OnLoadedGameplaySystems(RigScreenOptions __instance)
        {
            if (!DevToolsHiderMod.IsFlatPlayer || DevToolsHiderMod.EnableInFlatPlayer.Value)
            {
                DevToolsHiderMod.UpdateSpectatorCameraSettings(DevToolsHiderMod.DevToolsHidden.Value);
            }
        }
    }
}