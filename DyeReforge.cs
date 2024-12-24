using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using DyeAnything.Projectiles;
using DyeAnything.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DyeAnything
{

    // With this , item stat should be entirely very modular and easy to make with less bloats
    public struct ReforgeStat
    {
        public string name;
        public string description;
        public float statValue;
        public bool usePercentage;

        public ReforgeStat(string name, string description, float statValue,bool usePercentage)
        {
            this.name = name;
            this.description = description;
            this.statValue = statValue;
            this.usePercentage = usePercentage;
            
        }

        public void HoldUpdate(Player player)
        {
            if (usePercentage)
            {
                switch (name)
                {
                    case "moveSpeed":player.moveSpeed += statValue;break;
                    case "manaCost":player.manaCost += statValue;break;
                    case "coinLuck":player.coinLuck += statValue;break;
                    case "lifeSteal":player.lifeSteal += statValue;break;
                    default:break;
                }
            }
            else
            {
                switch (name)
                {
                    // Ints
                    case "lifeRegen":player.lifeRegen += (int)statValue;break;
                    case "statLifeMax2":player.statLifeMax2 += (int)statValue;break;
                    case "statDefense":player.statDefense += (int)statValue;break;
                    default:break;
                }
            }
        }

        // Strings
        public string GetDescription()
        {
            return description.Replace("{val}",GetStatText);
        }

        public string GetStatText => (statValue > 0 ? "+":"-") + " "+ (usePercentage ? FormatPercent(statValue) : statValue )+" ";

        public string FormatPercent(float perc)
        {
            return perc * 100f + " %";
        }
    }

    public class ReforgeApplyItem : GlobalItem
    {
        public static ReforgeApplyItem Get => ModContent.GetInstance<ReforgeApplyItem>();
        public Dictionary<int,ReforgeStat[]> reforgeStats;

        public void LoadItem(Random random, Item item)
        {
            Mod.Logger.Info("Adding Good Prefixes for "+item.Name);


        }

        public void PreSetup(Random random)
        {

            Mod.Logger.Info("Loading dye common reforges modules");

            // markiplier from the famous tv show among us
            random = new Random(DyeServerConfig.Get.DyeReforgeSeed.GetHashCode());
            reforgeStats = new Dictionary<int,ReforgeStat[]>();

            Mod.Logger.Info("Injecting dye loop");
        }

        public float GetStatValue(Item item , string name)
        {
            if (DyedItem.TryGetDye(item , out int dye))
            {
                // error checking if somehow shit happen
                if (reforgeStats == null || reforgeStats.Count <= 0)
                {
                    Main.NewText("Error : Reforge stat null or empty");
                    Mod.Logger.Error("Error : Reforge stat null or empty");
                    return 0f;
                }

                foreach (var stat in reforgeStats[dye])
                {
                    if (stat.name == name)
                    {
                        return stat.statValue;
                    }
                }
            }

            return 0f;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return base.AppliesToEntity(entity, lateInstantiation);
        }

        public override void HoldItem(Item item, Player player)
        {
            base.HoldItem(item, player);
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        {
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            float multiplier = base.UseSpeedMultiplier(item, player);
            multiplier += GetStatValue(item,"speed");   
            return multiplier;
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            damage += GetStatValue(item,"damage");   
        }
    }
}

