﻿using BeatSaberMarkupLanguage.Attributes;
using CameraUtils.Core;
using IPA.Config.Stores;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace ClearMind
{
    public class Config
    {
        public static Config Instance;
        public virtual bool Enabled { get; set; } = false;
        public virtual bool OutsideTransparent { get; set; } = false;
        public virtual bool ForceTransparent { get; set; } = false;
        public virtual bool HideOutline { get; set; } = false;
        public virtual bool HideDesktopView { get; set; } = false;

        /// <summary>
		/// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
		/// </summary>
		public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(Config other)
        {
            // This instance's members populated from other
        }
    }
}
