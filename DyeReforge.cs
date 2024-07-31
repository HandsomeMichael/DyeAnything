using System.Collections.Generic;
using DyeAnything.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace DyeAnything
{
    public class DyeReforge : GlobalItem
    {
        public static string GetPrefixString(int itemID) 
        {
            switch (itemID)
            {
                case ItemID.GelDye:return "Slimed in a non lewd way";
                case ItemID.ReflectiveGoldDye:return "Immersed in gold";
                // Hardmode i think ??
                case ItemID.IntenseRainbowDye:return "Gayass";
                case ItemID.TwilightDye:return "Burn within the twilight";
                case ItemID.PixieDye: return"Sparkled with fairy minds";
                case ItemID.ReflectiveDye:return"Immersed with illusion";
                case ItemID.IntenseBlueFlameDye:return "Pure rage.";
                // End Game
                case ItemID.VortexDye:return "Impower guns and bows";
                case ItemID.SolarDye:return"Sword molted with solar";
                case ItemID.NebulaDye:return "Duplicates magic power";
                case ItemID.StardustDye:return "Attracts stars and below";
                default:
                    return "";
            }
        }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return DyeServerConfig.Get.DyeReforges;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.damage > 0 && entity.dye <= 0;

        public override void Load()
        {
            Mod.Logger.Info("Loaded dye reforges modules");
        }

        public override float UseAnimationMultiplier(Item item, Player player)
        {
            return base.UseAnimationMultiplier(item, player);
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return base.UseSpeedMultiplier(item, player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (DyedItem.TryGetDye(item , out int dye))
            {
                if (DyeAnything.dyeToItemID[dye] == ItemID.VortexDye && Main.rand.NextBool(1,10))
                {
                    if (item.useAmmo == AmmoID.Bullet)
                    {
                        Projectile.NewProjectile(source,position,velocity*0.9f,ProjectileID.MoonlordBullet,damage/10,knockback,player.whoAmI,0f);   
                    }
                    else if (item.useAmmo == AmmoID.Arrow)
                    {
                        Projectile.NewProjectile(source,position,velocity*0.9f,ProjectileID.MoonlordArrow,damage/10,knockback,player.whoAmI,0f);   
                    }
                }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (DyedItem.TryGetDye(item , out int dye))
            {
                if (DyeAnything.dyeToItemID[dye] == ItemID.SolarDye && Main.rand.NextBool(80,100))
                {
                    Projectile.NewProjectile(item.GetSource_FromThis(),target.Bottom,Vector2.Zero,ProjectileID.MolotovFire,10,0f,player.whoAmI,0f);
                }

                if (DyeAnything.dyeToItemID[dye] == ItemID.SolarDye && Main.rand.NextBool(80,100))
                {

                }
            }
            base.OnHitNPC(item, player, target, hit, damageDone);
        }


        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {

            if (DyedItem.TryGetDye(item , out int dye))
            {
                string text = GetPrefixString(DyeAnything.dyeToItemID[dye]) ;
                if (text != "") tooltips.Add(new TooltipLine(Mod,"dyeReforge",text));
            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Name == "dyeReforge")
            {
                if (DyedItem.TryGetDye(item , out int dye))
                {
                    Main.spriteBatch.BeginDyeShader(dye,item,true,true);

                    ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, line.Font, line.Text, new Vector2(line.X, line.Y), Color.White, line.Rotation, line.Origin, line.BaseScale, line.MaxWidth, line.Spread);
                    
                    Main.spriteBatch.BeginNormal(true,true);

                    return false;
                }
            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }
    }
}



























// Very WIP 

// using System.Collections.Generic;
// using DyeAnything.Utils;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria;
// using Terraria.Audio;
// using Terraria.DataStructures;
// using Terraria.Graphics.Shaders;
// using Terraria.ID;
// using Terraria.ModLoader;
// using System;
// using System.Linq;
// using ReLogic.Content;
// using Terraria.GameContent;
// using Terraria.GameContent.Events;
// using Terraria.GameContent.Liquid;
// using Terraria.Graphics;
// using Terraria.UI;
// using Ionic.Zip;

// namespace DyeAnything.Reforge
// {

// 	public class DyeReforges : ModSystem
// 	{
//     }

// 	class DyePrefixItem : GlobalItem
//     {
//         public DyePrefix GetCurrentPrefix(Item item) 
//         {
//             if (item == null ) return null;
//             if (item.TryGetGlobalItem<DyedItem>(out DyedItem dyedItem))
//             {
//                 if (dyedItem.dye > 0 && DyePrefixManager.Get().dyePrefixes.Length > dyedItem.dye) 
//                 {
//                     return DyePrefixManager.Get().dyePrefixes[dyedItem.dye];
//                 }
//             }
//             return null;
//         }
//         public override void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers)
//         {
//             GetCurrentPrefix(item)?.ModifyHitPvp(item,player,target,ref modifiers);
//         }

//         public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
//         {
//             GetCurrentPrefix(item)?.ModifyHitNPC(item,player,target,ref modifiers);
//         }

//         public override void ModifyItemScale(Item item, Player player, ref float scale)
//         {
//             GetCurrentPrefix(item)?.ModifyItemScale(item,player,ref scale);
//         }

//         public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
//         {
//             GetCurrentPrefix(item)?.ModifyManaCost(item,player,ref reduce,ref mult);
//         }

//         public override void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
//         {
//             GetCurrentPrefix(item)?.ModifyShootStats(item,player,ref position, ref velocity, ref type , ref damage ,ref knockback);
//         }

//         public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
//         {
//             GetCurrentPrefix(item)?.ModifyWeaponCrit(item,player,ref crit);
//         }

//         public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
//         {
//             GetCurrentPrefix(item)?.ModifyWeaponDamage(item,player,ref damage);
//         }

//         public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
//         {
//             GetCurrentPrefix(item)?.ModifyWeaponKnockback(item,player,ref knockback);
//         }

//         public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
//         {
//             GetCurrentPrefix(item)?.ModifyTooltips(item,tooltips,Mod);
//         }
//     }

//     public class DyePrefixManager : ModSystem
//     {
//         public static DyePrefixManager Get() => ModContent.GetInstance<DyePrefixManager>();
//         public DyePrefix[] dyePrefixes;

//         public override void PostSetupContent()
//         {
//             Random newRand = new Random(123);
//             List<DyePrefix> prefixes = new List<DyePrefix>(){
//                 new CritDyePrefix()
//             }; 
//             for (int i = 0; i < DyeAnything.dyeList.Count; i++)
//             {
                
//             }
            
            
//         }

//         public override void Unload()
//         {
//             dyePrefixes = null;
//         }

//     }

//     class CritDyePrefix : DyePrefix
//     {
//         public override void ModifyTooltips(Item item, List<TooltipLine> tooltips,Mod mod)
//         {
//             tooltips.Add(new TooltipLine(mod,"dyeprefix","+ Great crit lower damage"));
//         }
//         public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
//         {
//             crit += 0.05f; 
//         }

//         public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
//         {
//             damage -= 0.1f;
//         }
//     }
// 	public abstract class DyePrefix
// 	{
// 		public virtual void ModifyHitPvp(Item item, Player player, Player target, ref Player.HurtModifiers modifiers)
//         {
//         }

//         public virtual void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
//         {
//         }

//         public virtual void ModifyItemScale(Item item, Player player, ref float scale)
//         {
//         }

//         public virtual void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
//         {
//         }

//         public virtual void ModifyShootStats(Item item, Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
//         {
//         }

//         public virtual void ModifyWeaponCrit(Item item, Player player, ref float crit)
//         {
//         }

//         public virtual void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
//         {
//         }

//         public virtual void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
//         {
//         }

//         public virtual void ModifyTooltips(Item item, List<TooltipLine> tooltips,Mod mod)
//         {
//         }
// 	}
// }


