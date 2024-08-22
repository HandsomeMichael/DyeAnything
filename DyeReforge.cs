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
    public class DyeReforge : GlobalItem
    {

        // as you now im a lazy and really complicated child , i want to make shit automatic

        internal struct DyeStatIncrease
        {

            public int maxLife;
            public float speed;
            public int regen;

            public int defense;

            public int manaCost;

            public float damage;

            public float wepSpeed;

            public float coinLuck;

            public float lifeSteal;


            public void ApplyStat(Player player)
            {
                player.lifeRegen += regen;
                player.moveSpeed += speed;
                player.statLifeMax2 += maxLife;
                player.statDefense += defense;
                player.manaCost += manaCost;
                player.coinLuck += coinLuck;
                player.lifeSteal += lifeSteal;
            }

            public void ApplyDamage(Item item,ref StatModifier statDamage)
            {
                if (damage != 0)
                {
                    statDamage *= 1f + damage;
                }
            }

            string FormatPercent(float perc)
            {
                return perc * 100f + " %";
            }

            public string GetStatText(Item item)
            {
                string finalText = "";

                if (maxLife != 0) finalText += maxLife > 0 ? "Increased Life By "+maxLife+"\n" : "Decreased Life By "+maxLife+"\n";
                if (speed != 0) finalText += speed > 0 ? "Increased Movement Speed By "+FormatPercent(speed)+"\n" : "Decreased Movement Speed By "+FormatPercent(speed)+"\n";
                if (regen != 0) finalText += regen > 0 ? "Increased Regen By "+regen+"\n" : "Decreased Regen By "+regen+"\n";
                if (defense != 0) finalText += defense > 0 ? "Increased Defense By "+defense+"\n" : "Decreased Defense By "+defense+"\n";
                if (damage != 0) finalText += damage > 0 ? "Increased Damage By "+FormatPercent(damage)+"\n" : "Decreased Damage By "+FormatPercent(damage)+"\n";
                if (coinLuck != 0) finalText += coinLuck > 0 ? "Increased Coin Luck By "+FormatPercent(coinLuck)+"\n" : "Decreased Coin Luck By "+FormatPercent(coinLuck)+"\n";
                if (lifeSteal != 0) finalText += "Can life leach by "+FormatPercent(lifeSteal);
                if (wepSpeed != 0) finalText += wepSpeed > 0 ? "Increased Attack Speed By "+FormatPercent(wepSpeed)+"\n" : "Decreased Attack Speed By "+FormatPercent(wepSpeed)+"\n";

                if (item.mana > 0)
                {
                    if (manaCost != 0) finalText += manaCost > 0 ? "Increased Mana Cost By "+manaCost+"\n" : "Decreased Mana Cost By "+manaCost+"\n";
                }

                return finalText == "" ? "No Extra Quality Found" : finalText ;
            }

        }

        internal static Dictionary<int,DyeStatIncrease> playerStats;
        static internal Random oneTimeUseRandomWow;

        public override void Unload()
        {
            playerStats = null;
        }

        public static void PreSetup(Mod Mod)
        {

            Mod.Logger.Info("Loading dye common reforges modules");

            playerStats = new Dictionary<int, DyeStatIncrease>();
            oneTimeUseRandomWow = new Random("The seed is fuck you".GetHashCode());

            Mod.Logger.Info("Setupping loop");
        }

        public static void PostSetup(Mod Mod)
        {
            oneTimeUseRandomWow = null;
        }
        
        public static void LoadItem(Mod Mod, Item item,int itemId)
        {
            var stat = new DyeStatIncrease();

            Mod.Logger.Info("Adding Good Prefixes for "+item.Name);

            var ran = oneTimeUseRandomWow;

            // Good
            switch (ran.Next(9))
            {
                case 0: stat.speed = (float)ran.Next(0,20) / 100f; break;
                case 1: stat.regen = ran.Next(1,10); break;
                case 2: stat.maxLife = ran.Next(10,50); break;
                case 3: stat.defense = ran.Next(1,8); break;
                case 4: stat.manaCost = ran.Next(5,20); break;
                case 5: stat.damage = (float)ran.Next(5,15) / 100f; break;
                case 6: stat.coinLuck = (float)ran.Next(1,10) / 100f; break;
                case 7: stat.lifeSteal = (float)ran.Next(1,5) / 100f; break;
                case 8: stat.wepSpeed = (float)ran.Next(1,10) / 100f; break;
                default:break;
            }

            Mod.Logger.Info("Adding Bad Prefixes for "+item.Name);

            // Bad
            switch (ran.Next(8))
            {
                case 0: stat.speed -= (float)ran.Next(0,10) / 100f; break;
                case 1: stat.regen -= ran.Next(1,6); break;
                case 2: stat.maxLife -= ran.Next(5,10); break;
                case 3: stat.defense -= ran.Next(1,4); break;
                case 4: stat.manaCost -= ran.Next(5,10); break;
                case 5: stat.damage -= (float)ran.Next(2,10) / 100f; break;
                case 6: stat.coinLuck -= (float)ran.Next(1,10) / 100f; break;
                case 7: stat.wepSpeed -= (float)ran.Next(1,15) / 100f; break;
                default:break;
            }

            Mod.Logger.Info("Adding Stat "+itemId+" // "+item.dye+ " // "+ item.Name);

            // playerStats.Add(item.dye,stat);
            playerStats[item.dye] = stat;
        }

        public override void HoldItem(Item item, Player player)
        {
            if (DyedItem.TryGetDye(item , out int dye))
            {
                playerStats[dye].ApplyStat(player);
            }   
        }

        public static string GetRegularPrefix(int itemID)
        {
            return "";
        }

        public static string GetPrefixString(int itemID) 
        {
            switch (itemID)
            {
                case ItemID.GelDye:return "Slimed in a non lewd way";
                case ItemID.ReflectiveGoldDye:return "Immersed in gold";
                // Hardmode i think ??
                case ItemID.GreenDye:return "Lets gamble.";
                case ItemID.IntenseRainbowDye:return "Gayass";
                case ItemID.TwilightDye:return "Burn within the twilight";
                case ItemID.PixieDye: return"Sparkled with fairy minds";
                case ItemID.ReflectiveDye:return"Immersed with illusion";
                case ItemID.IntenseBlueFlameDye:return "Pure rage.";
                // End Game
                case ItemID.VortexDye:return "Impower range";
                case ItemID.SolarDye:return"Melting Swords";
                case ItemID.NebulaDye:return "Magic duplicator";
                case ItemID.StardustDye:return "Star attractor";
                default:
                    return "Non Special Dye";
            }
        }

        public override bool IsLoadingEnabled(Mod mod)
        {
            return DyeServerConfig.Get.DyeReforges;
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.damage > 0 && entity.dye <= 0;

        public override float UseAnimationMultiplier(Item item, Player player)
        {
            float mult = base.UseAnimationMultiplier(item, player);
            if (DyedItem.TryGetDye(item , out int dye))
            {
                mult += playerStats[dye].wepSpeed;
            }
            return mult;
        }

        public override float UseSpeedMultiplier(Item item, Player player)
        {
            return UseAnimationMultiplier(item,player);
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (DyedItem.TryGetDye(item , out int dye))
            {
                if (DyeAnything.dyeToItemID[dye] == ItemID.VortexDye && Main.rand.NextBool(5,100))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        SoundEngine.PlaySound(SoundID.Item142);
                        Projectile.NewProjectile(source,position,velocity.RotatedByRandom(MathHelper.ToRadians(30)),type,damage/10,knockback,player.whoAmI,0f);
                    }
                    // if (item.useAmmo == AmmoID.Bullet)
                    // {
                    //     Projectile.NewProjectile(source,position,velocity*0.9f,ProjectileID.MoonlordBullet,damage/10,knockback,player.whoAmI,0f);   
                    // }
                    // else if (item.useAmmo == AmmoID.Arrow)
                    // {
                    //     Projectile.NewProjectile(source,position,velocity*0.9f,ProjectileID.MoonlordArrow,damage/10,knockback,player.whoAmI,0f);   
                    // }
                }
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            if (DyedItem.TryGetDye(item , out int dye))
            {
                playerStats[dye].ApplyDamage(item,ref damage);   
            }
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            
            if (DyedItem.TryGetDye(item , out int dye))
            {
                
                if (DyeAnything.dyeToItemID[dye] == ItemID.GreenDye)
                {
                    modifiers.FinalDamage *= Main.rand.NextFloat()*2f;
                }
            }
            base.ModifyHitNPC(item, player, target, ref modifiers);
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {

            if (DyedItem.TryGetDye(item , out int dye))
            {
                // 50% chance to summon a slashing sword

                if (DyeAnything.dyeToItemID[dye] == ItemID.SolarDye && Main.rand.NextBool(90,100))
                {

                    const float swordDistance = 100f;
                    Vector2 pos = target.Center + new Vector2(swordDistance,swordDistance).RotatedByRandom(MathHelper.ToRadians(360));

                    CombatText.NewText(new Rectangle((int)pos.X,(int)pos.Y,0,0),Color.White,"spawns");

                    Projectile.NewProjectile(item.GetSource_FromThis(),
                    pos,
                    pos.DirectionTo(target.Center) * 5f,
                    ModContent.ProjectileType<SolarSword>(),
                    10,
                    1f,
                    player.whoAmI,
                    0,
                    target.whoAmI);
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

                tooltips.Add(new TooltipLine(Mod,"dyeCommonPrefix",playerStats[dye].GetStatText(item)));
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


