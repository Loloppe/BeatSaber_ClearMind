
using CameraUtils.Core;
using HarmonyLib;
using System.Linq;
using UnityEngine;

namespace ClearMind.HarmonyPatches
{
    internal class Patches
    {
        [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
        static class DetectExtensions
        {
            static void Postfix(StandardLevelDetailView __instance)
            {
                if (__instance.beatmapKey != null && __instance._beatmapLevel != null)
                {
                    var hasRequirement = SongCore.Collections.RetrieveDifficultyData(__instance._beatmapLevel, __instance.beatmapKey)?
                    .additionalDifficultyData?
                    ._requirements?.Any(x => x == "Noodle Extensions" || x == "Mapping Extensions") == true;
                    if (hasRequirement) Config.Instance.Enabled = false;
                    else Config.Instance.Enabled = true;
                }
            }
        }

        [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
        static class HideObstacle
        {
            static void Prefix(ref ObstacleController __instance, ref ObstacleData obstacleData)
            {
                if (Config.IsEnabled())
                {
                    // This game seems to recycle gameObject or something, so need to set back parameters just in case.
                    VisibilityUtils.SetLayerRecursively(__instance._stretchableObstacle.transform.Find("HideWrapper"), VisibilityLayer.Obstacle);
                    var renderer = __instance._stretchableObstacle.transform.Find("ObstacleCore").GetComponent<MeshRenderer>();
                    if (Config.Instance.ForceTransparent) renderer.forceRenderingOff = true;
                    else renderer.forceRenderingOff = false;

                    if ((obstacleData.lineIndex > 3 && obstacleData.width >= 0) || (obstacleData.lineIndex < 0 && obstacleData.width + obstacleData.lineIndex <= 0))
                    {
                        // Hide the obstacle core
                        renderer.forceRenderingOff = true;
                        if (!Config.Instance.Transparent)
                        {
                            if (!Config.Instance.HideDesktopView)
                            {
                                // The only issue with this is that the desktop view will see outside walls as invisible, and the rest as normal walls.
                                VisibilityUtils.SetLayerRecursively(__instance._stretchableObstacle.transform.Find("HideWrapper"), VisibilityLayer.DesktopOnly);
                            }
                            else
                            {
                                VisibilityUtils.SetLayerRecursively(__instance._stretchableObstacle.transform.Find("HideWrapper"), VisibilityLayer.Event);
                            }
                        }
                    }
                }
            }
        }
    }
}
