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


