
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
                if (Config.Instance.Enabled)
                {
                    // This game seems to reuse object during gameplay, so need to set back parameters just in case.
                    VisibilityUtils.SetLayerRecursively(__instance._stretchableObstacle.transform.Find("HideWrapper"), VisibilityLayer.Obstacle);
                    var renderer = __instance._stretchableObstacle.transform.Find("ObstacleCore").GetComponent<MeshRenderer>();
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
                            VisibilityUtils.SetLayerRecursively(__instance._stretchableObstacle.transform.Find("HideWrapper"), VisibilityLayer.Event);
                        }
                        else if (Config.Instance.HideOutline)
                        {
                            // The only issue with this logic is that the desktop view might see distant wall as transparent, while the rest are default.
                            VisibilityUtils.SetLayerRecursively(__instance._stretchableObstacle.transform.Find("HideWrapper"), VisibilityLayer.DesktopOnly);
                        }
                    }
                }
            }
        }
    }
}
