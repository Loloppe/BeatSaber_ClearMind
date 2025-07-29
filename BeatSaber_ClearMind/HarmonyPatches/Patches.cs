using CameraUtils.Core;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace ClearMind.HarmonyPatches
{
    internal class Patches
    {
        [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
        static class HideObstacle
        {
            static void Prefix(ref ObstacleController __instance, ref ObstacleData obstacleData)
            {
                if (Config.Instance.Enabled)
                {
                    // This game seems to reuse object during gameplay, so need to set back parameters just in case.
                    GameObject hideWrapper = null;
                    MeshRenderer renderer = null;
                    foreach (var wrapper in __instance._visualWrappers)
                    {
                        if (wrapper.name == "HideWrapper")
                        {
                            hideWrapper = wrapper;
                        }

                        if (wrapper.name == "ObstacleCore")
                        {
                            renderer = wrapper.GetComponent<MeshRenderer>();
                        }
                    }

                    if (renderer != null && hideWrapper != null)
                    {
                        VisibilityUtils.SetLayerRecursively(hideWrapper, VisibilityLayer.Obstacle);
                        renderer.forceRenderingOff = false;

                        // Option to make all walls transparent
                        if (Config.Instance.ForceTransparent) renderer.forceRenderingOff = true;

                        // Check to find non-gameplay wall.
                        if ((obstacleData.lineIndex > 3 && obstacleData.width >= 0) || (obstacleData.lineIndex < 0 && obstacleData.width + obstacleData.lineIndex <= 0))
                        {
                            if (Config.Instance.OutsideTransparent)
                            {
                                renderer.forceRenderingOff = true;
                            }
                            // This part hide the outline of the wall
                            if (Config.Instance.HideDesktopView)
                            {
                                VisibilityUtils.SetLayerRecursively(hideWrapper, VisibilityLayer.Event);
                            }
                            else if (Config.Instance.HideOutline)
                            {
                                // The only issue with this logic is that the desktop view will see outer wall as transparent, while the rest are default.
                                VisibilityUtils.SetLayerRecursively(hideWrapper, VisibilityLayer.DesktopOnly);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GameplayCoreInstaller), nameof(GameplayCoreInstaller.InstallBindings))]
        static class ArcIntensity
        {
            static void Postfix(ref GameplayCoreInstaller __instance)
            {
                var key = (BeatmapKey)__instance._sceneSetupData?.beatmapKey;
                var level = __instance._sceneSetupData?.beatmapLevel;
                if (key != null && level != null)
                {
                    var hasRequirement = SongCore.Collections.GetCustomLevelSongDifficultyData(key)?
                    .additionalDifficultyData?
                    ._requirements?.Any(x => x == "Noodle Extensions" || x == "Mapping Extensions") == true;
                    if (hasRequirement) Config.Instance.Enabled = false;
                    else Config.Instance.Enabled = true;
                }
            }
        }
    }
}
