using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using MelonLoader;
using UnityEngine;
using Il2CppSLZ.Marrow.Interaction;
using MenuHelper;

[assembly: MelonInfo(typeof(DevToolsHider.DevToolsHiderMod), name:"DevToolsHider", version:"1.0.0", author:"triangle", downloadLink:"https://thunderstore.io/c/bonelab/p/triangle/InteractableSettings/")]
[assembly: MelonGame(developer:"Stress Level Zero", name:"BONELAB")]

namespace DevToolsHider
{
    public class DevToolsHiderMod : MelonMod
    {
        public static MelonPreferences_Category DevToolsHider_Category;
        public static MelonPreferences_Entry<bool> DevToolsHidden;
        public static MelonPreferences_Entry<bool> EnableInFlatPlayer;
        public const Int32 FixtureLayer = 6;         // Layer 6, identified as Fixture, is the layer for fixtures (airlock doors, buttons, etc). Layer 11 does not collide with it by default.
        public const Int32 DynamicLayer = 10;        // Layer 10, identified as Dynamic, is the layer intended for dynamic object collision in BONELAB as of Patch 6. This isn't typically used for rendering, but it's possible that the user placed colliders on the renderers
        public const Int32 HideableLayer = 11;       // Layer 11 is the target for hiding objects
        public static bool IsFlatPlayer;
        public static Color SpawnGunBlue = new(0.06432372f, 0.7570404f, 0.856f, 1f);

        /*
         * By default, according to my findings, layer 11 does not collide with:
            1 - TransparentFX
            2 - IgnoreRaycast
            3 - ObserverTrigger
            4 - Water
            5 - UI
            6 - Fixture
            9 - NoCollide
            11 - 
            14 - 
            16 - Decaverse
            17 - Deciverse
            18 - Socket
            19 - Plug
            20 - 
            21 - PlayerAndNpc
            22 - 
            23 - FootballOnly
            25 - NoFootball
            26 - EntityTracker
            27 - BeingTracker
            28 - ObserverTracker
            29 - EntityTrigger
            30 - BeingTrigger
            31 - Background

         * Does collide with:
            0 - Default
            7 - 
            8 - Player
            10 - Dynamic
            12 - EnemyColliders
            13 - 
            15 - Interactable
            24 - Football

        * Layer 11 does collide with the default BONELAB projectile spawnable and laser pointers
        * Layer 11 can be world gripped

        * As of now, only 6 and 11 are added to collision with 11.
        */

        public override void OnInitializeMelon()
        {
            // Init MelonLoader preferences

            DevToolsHider_Category = MelonPreferences.CreateCategory("Dev Tools Hider");

            // Init BoneMenu elements

            BoneLib.BoneMenu.Page BoneMenu_MainPage = BoneLib.BoneMenu.Page.Root.CreatePage("Dev Tools Hider", SpawnGunBlue);
            {
                BoneMenu_MainPage.CreateBoolPref("Hide in Spectator", SpawnGunBlue, ref DevToolsHidden, UpdateSpectatorCameraSettings, prefDefaultValue: true);
                if (!BoneLib.HelperMethods.IsAndroid()) // This setting are useless on Quest as it doesn't have a spectator camera
                {
                    BoneMenu_MainPage.CreateBoolPref("Hide in FlatPlayer Spectator", SpawnGunBlue, ref EnableInFlatPlayer, UpdateSpectatorCameraSettings, prefDefaultValue: false, tooltip: "Hide in Spectator must be enabled for this to have any effect.");
                }
                BoneMenu_MainPage.CreateFunction("Toggle Held Items Visibility", SpawnGunBlue, ToggleHeldItemVisibility);
                BoneMenu_MainPage.CreateFunction("Toggle Held Camera Mask", SpawnGunBlue, ToggleHeldCameraMask);
            }
            Physics.IgnoreLayerCollision(HideableLayer, FixtureLayer, false);
            Physics.IgnoreLayerCollision(HideableLayer, HideableLayer, false);
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