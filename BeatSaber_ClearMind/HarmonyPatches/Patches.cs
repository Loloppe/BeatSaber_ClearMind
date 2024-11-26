
using CameraUtils.Core;
using HarmonyLib;
using System;
using System.Linq;

namespace ClearMind.HarmonyPatches
{
    internal class Patches
    {
        [HarmonyPatch(typeof(StandardLevelDetailView), nameof(StandardLevelDetailView.RefreshContent))]
        static class DetectExtensions
        {
            static void Postfix(StandardLevelDetailView __instance)
            {
                if(Config.Instance.Enabled == true && __instance.beatmapKey != null && __instance._beatmapLevel != null)
                {
                    var hasRequirement = SongCore.Collections.RetrieveDifficultyData(__instance._beatmapLevel, __instance.beatmapKey)?
                    .additionalDifficultyData?
                    ._requirements?.Any(x => x == "Noodle Extensions" || x == "Mapping Extensions") == true;
                    if (hasRequirement) Config.Instance.Enabled = false;
                }
            }
        }

        [HarmonyPatch(typeof(ObstacleController), nameof(ObstacleController.Init))]
        static class HideObstacle
        {
            static void Prefix(ref ObstacleData obstacleData, ref StretchableObstacle ____stretchableObstacle)
            {
                if (Config.Instance.Enabled)
                {
                    if ((obstacleData.lineIndex > 3 && obstacleData.width >= 0) || (obstacleData.lineIndex < 0 && obstacleData.width + obstacleData.lineIndex <= 0))
                    {
                        if (Config.Instance.HideDesktopView) VisibilityUtils.SetLayerRecursively(____stretchableObstacle.gameObject, VisibilityLayer.Event);
                        else VisibilityUtils.SetLayerRecursively(____stretchableObstacle.gameObject, VisibilityLayer.DesktopOnly);
                    }
                }
            }
        }

    }
}
