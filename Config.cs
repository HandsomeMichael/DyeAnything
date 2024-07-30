using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json.Serialization;

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace DyeAnything
{
	public class DyeServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static DyeServerConfig Get => ModContent.GetInstance<DyeServerConfig>();

        // save the config , this requires reflection though.
        public static void SaveConfig() => typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[1] { Get });

        [DefaultValue(true)] 
		public bool SprayerCanDyeProjectile; 

		[DefaultValue(true)] 
		public bool WaterRemoveItemDye;

		[DefaultValue(true)] 
		public bool WaterRemoveNPCDye;

		[DefaultValue(true)] 
		public bool DyeReforges;
	}

	public class DyeClientConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;
		public static DyeClientConfig Get => ModContent.GetInstance<DyeClientConfig>();

        // save the config , this requires reflection though.
        public static void SaveConfig() => typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[1] { Get });

		[DefaultValue(true)] 
		public bool DyeItemDyeDye;

		[DefaultValue(true)] 
		public bool DyeItemPrefix;
	}
}