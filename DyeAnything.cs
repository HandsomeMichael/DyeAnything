 using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

using DyeAnything.Utils;
using Terraria.ModLoader.IO;
using Terraria.UI.Chat;
using Terraria.GameContent;
using System;
using Terraria.DataStructures;
using System.IO;
using Terraria.Audio;
using Terraria.ID;
using DyeAnything.Items;
using Terraria.Graphics.Shaders;


namespace DyeAnything
{
    // In preparation of shader hack rewrite

    // most code in here are adapted
    // many many appreciation for "gamrguy"to make his 1.3 shaderLib opensource
    // https://github.com/gamrguy/ShaderLib/tree/master 

	public class DyeAnything : Mod
	{
		// public static Dictionary<int,int> dyeList;
		public static List<int> dyeList;
        public static Dictionary<int,int> dyeToItemID;
        public static Dictionary<int,int> dyeToDyeName;

        // public static bool currentlyDoingAReallySketchyHackToPatchShit;

        public override void Load()
        {
			// wtf
            dyeList = new List<int>();
            dyeToItemID = new Dictionary<int, int>();
            Terraria.On_Main.GetProjectileDesiredShader += ShaderPatch;
            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float += ProjPatch;
            // Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_27_HeldItem += HeldItemPatch;
            // Terraria.DataStructures.On_DrawData.ad
        }

        public override void Unload()
        {
            dyeList = null;
            dyeToItemID = null;
            dyeToDyeName = null;
            Terraria.On_Main.GetProjectileDesiredShader -= ShaderPatch;
            Terraria.On_Projectile.NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float -= ProjPatch;
            // Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_27_HeldItem -= HeldItemPatch;
        }

        // private void HeldItemPatch(On_PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo)
        // {
        //     currentlyDoingAReallySketchyHackToPatchShit = true;
        //     orig(ref drawinfo);
        //     currentlyDoingAReallySketchyHackToPatchShit = false;
        // }

        private int ProjPatch(On_Projectile.orig_NewProjectile_IEntitySource_float_float_float_float_int_int_float_int_float_float_float orig, 
        IEntitySource spawnSource, float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner, float ai0, float ai1, float ai2)
        {
            int hasil = orig(spawnSource,X,Y,SpeedX,SpeedY,Type,Damage,KnockBack,Owner,ai0,ai1,ai2);

            if (!DyeServerConfig.Get.ProjectileFollowParentDye) return hasil;

            // get from item
            if (hasil >= 0 && spawnSource is IEntitySource_WithStatsFromItem itemSource)
            {
                // Main.NewText("from item");
                if (itemSource.Item != null && itemSource.Item.TryGetGlobalItem<DyedItem>(out DyedItem dyedItem)) 
                {
                    // Main.NewText("got moditem");
                    if (dyedItem.dye > 0 && Main.projectile[hasil].TryGetGlobalProjectile<DyedProjectile>(out DyedProjectile dyedProjectile))
                    {
                        // Main.NewText("success applied " + dyedItem.dye);
                        dyedProjectile.dye = dyedItem.dye;
                    }
                }
            }

            // get from parents
            if (hasil >= 0 && spawnSource is EntitySource_Parent entitySource ) 
            {
                if (entitySource != null && entitySource.Entity != null)
                {
                    // projectile
                    if (entitySource.Entity is Projectile projSource) 
                    {
                        if (projSource.TryGetGlobalProjectile<DyedProjectile>(out DyedProjectile parentDyedProjectile))
                        {
                            if (parentDyedProjectile.dye > 0 && Main.projectile[hasil].TryGetGlobalProjectile<DyedProjectile>(out DyedProjectile dyedProjectile))
                            {
                                dyedProjectile.dye = parentDyedProjectile.dye;
                            }

                        }
                    } // npc
                    else if (entitySource.Entity is NPC npcSource) 
                    {
                        if (npcSource.TryGetGlobalNPC<DyedNPC>(out DyedNPC dyedNPC))
                        {
                            if (dyedNPC.dye > 0 && Main.projectile[hasil].TryGetGlobalProjectile<DyedProjectile>(out DyedProjectile dyedProjectile)) 
                            {
                                dyedProjectile.dye = dyedNPC.dye;
                            }
                        }
                    }
                }
            }
            return hasil;
        }

        public override void PostSetupContent() 
		{

            if (DyeServerConfig.Get.DyeReforges)
            {
                DyeReforge.PreSetup(this);
            }

			for (int i = 0; i < ItemLoader.ItemCount; i++)
			{
				Item item = new Item();
				item.SetDefaults(i);

                if (item.type == ItemID.UnicornWispDye)
                {

                }
				if (item.dye > 0)
				{
					// dyeList.Add(i,item.dye);
					dyeList.Add(item.dye);
                    dyeToItemID[item.dye] = i;

                    if (DyeServerConfig.Get.DyeReforges)
                    {

                        // very very very very weird bug

                        DyeReforge.LoadItem(this,item,i);
                    }
				}
			}

            if (DyeServerConfig.Get.DyeReforges)
            {
                DyeReforge.PostSetup(this);
            }
		}

        private int ShaderPatch(On_Main.orig_GetProjectileDesiredShader orig, Projectile proj)
        {
            // if (proj != null && proj.active && proj.owner != 255)
            // {
            //     if (proj.TryGetOwner(out Player player)) 
            //     {
            //         if (player.TryGetModPlayer<DyePlayer>(out DyePlayer dyePlayer)) 
            //         {
            //             return dyePlayer.dye;
            //         }
            //     }
            // }
            if (proj.active && proj.TryGetGlobalProjectile<DyedProjectile>(out DyedProjectile dyedProjectile)) 
            {
                if (dyedProjectile.dye > 0)
                {
                    return dyedProjectile.dye;
                }
            }
            return orig(proj);
        }
	}

	public class DyePlayer : ModPlayer
	{
		public int dye;

        public override void ResetEffects()
        {
            dye = 0;

            // update dye in dye slot
            foreach (var item in Player.dye)
            {
                if (item != null && item.type == ModContent.ItemType<RandomDye>() || item.type == ModContent.ItemType<IntenseRandomDye>() || item.type == ModContent.ItemType<SuperIntenseRandomDye>()) 
                {
                    RandomDye ranDye = item.ModItem as RandomDye;
                    ranDye.UpdateInventory(Player);
                }
            }
        }
        // public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        // {
        // }
    }

	public class DyedNPC : GlobalNPC
	{
		public int dye;

        public override bool InstancePerEntity => true;
        // protected override bool CloneNewInstances => true;


		// save dye for town npcs
        // do we need to sync this on server ?
        public override void SaveData(NPC npc, TagCompound tag)
        {

            // Wow slight performance wow

            if (!DyeServerConfig.Get.FailSaveLoad)
            {
                tag.Add("dye",dye);
                return;
            }

            // Only save saved dye

            if (DyeAnything.dyeList.Contains(dye)) 
            {
                tag.Add("dye",dye);
            }
            else 
            {
                Mod.Logger.Warn("Missing dye mod , refusing to save");
            }
        }

        public override void LoadData(NPC npc, TagCompound tag)
        {

            dye = tag.GetAsInt("dye");
            if (!DyeServerConfig.Get.FailSaveLoad) return;

            if (!DyeAnything.dyeList.Contains(dye))
            {
                // use the first dye
                dye = DyeAnything.dyeList[1];
                Mod.Logger.Warn("Missing dye mod , altering dye");
            }
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			if (dye > 0) {spriteBatch.BeginDyeShader(dye,npc,true);}
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
			if (dye > 0) {spriteBatch.BeginNormal(true);}
            // ChatManager.DrawColorCodedString(spriteBatch,FontAssets.DeathText.Value,"dye : "+dye,npc.Center - Main.screenPosition,Color.White,0f,Vector2.One,Vector2.One);
        }
    }

	public class DyedProjectile : GlobalProjectile
	{
		public int dye;
        public override bool InstancePerEntity => true;

        public override void PostAI(Projectile projectile)
        {
			// very safe shit
			if (projectile.owner != 255 && projectile.TryGetOwner(out Player owner)) 
			{
				if (owner.TryGetModPlayer<DyePlayer>(out DyePlayer dyePlayer)) 
				{
                    if (dyePlayer.dye != 0) {dye = dyePlayer.dye;}
				}
			}
        }

        public static bool TryGetDye(Projectile projectile,out int dyeValue)
		{
            dyeValue = 0;

			if (projectile.TryGetGlobalProjectile<DyedProjectile>(out DyedProjectile dyedProjectile))
			{
                dyeValue = dyedProjectile.dye;
                return dyedProjectile.dye > 0;
			}

            return false;
		}

        // public override bool PreDraw(Projectile projectile, ref Color lightColor)
        // {
		// 	if (dye > 0) {Main.spriteBatch.BeginDyeShader(dye,projectile,true);}
        //     return base.PreDraw(projectile, ref lightColor);
        // }

        // public override void PostDraw(Projectile projectile, Color lightColor)
        // {
        //     // if (dye > 0) {Main.spriteBatch.BeginNormal(true);}
        //     ChatManager.DrawColorCodedString(Main.spriteBatch,FontAssets.DeathText.Value,"dye : "+dye,projectile.Center - Main.screenPosition,Color.White,0f,Vector2.One,Vector2.One);

        // }
    }

	public class DyedItem : GlobalItem
	{
		public int dye;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public static bool TryGetDye(Item item,out int dyeValue)
		{
            dyeValue = 0;

			if (item.TryGetGlobalItem<DyedItem>(out DyedItem dyedItem))
			{
                dyeValue = dyedItem.dye;
                return dyedItem.dye > 0;
			}

            return false;
		}


        public override void Update(Item item, ref float gravity, ref float maxFallSpeed)
        {
            if (!DyeServerConfig.Get.WaterRemoveItemDye) return;
            
            // wet item dont deserve love
            if (item.wet) 
            {
                if (dye != 0)
                {
                    for (int a = 0; a < 30; a++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Dust dust = Dust.NewDustPerfect(item.Center, 182, speed * Main.rand.NextFloat(1f, 3f), Scale: 1.5f);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader(dye, Main.LocalPlayer);
                    }
                    dye = 0;
                }
            }
        }
        
        public override void NetSend(Item item, BinaryWriter writer)
        {
            writer.Write(dye);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            dye = reader.ReadInt32();
        }

        
        public override void SaveData(Item item, TagCompound tag)
        {

            // Wow slight performance wow

            if (!DyeServerConfig.Get.FailSaveLoad)
            {
                tag.Add("dye",dye);
                return;
            }

            // Only save saved dye

            if (DyeAnything.dyeList.Contains(dye)) 
            {
                tag.Add("dye",dye);
            }
            else 
            {
                Mod.Logger.Warn("Missing dye mod , refusing to save");
            }
        }

        public override void LoadData(Item item, TagCompound tag)
        {

            dye = tag.GetAsInt("dye");
            if (!DyeServerConfig.Get.FailSaveLoad) return;

            if (!DyeAnything.dyeList.Contains(dye))
            {
                // use the first dye
                dye = DyeAnything.dyeList[1];
                Mod.Logger.Warn("Missing dye mod , altering dye");
            }
        }

        // only apply to weapons
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.damage > 0 && entity.dye <= 0;
        }

        public override bool PreDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			if (dye > 0) 
            {

                Entity entityShader = DyeClientConfig.Get.ItemPlayerShader ? Main.LocalPlayer: item;
                
                DrawData data = new DrawData
                {
                    position = position - Main.screenPosition,
                    scale = new Vector2(scale, scale),
                    sourceRect = frame,
                    texture = TextureAssets.Item[item.type].Value
                };
                spriteBatch.BeginDyeShader(dye,entityShader,true,true,data);
            }
            return base.PreDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
			if (dye > 0) {spriteBatch.BeginNormal(true,true);}

            if (DyeClientConfig.Get.Debug)
            ChatManager.DrawColorCodedString(spriteBatch,FontAssets.MouseText.Value,":"+dye,position,Color.White,0f,Vector2.One,Vector2.One);
        }

        public override bool PreDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
			if (dye > 0)
            {
                Entity entityShader = DyeClientConfig.Get.ItemPlayerShader ? Main.LocalPlayer: item;
                DrawData data = new DrawData
                {
                    position = item.position - Main.screenPosition,
                    scale = new Vector2(scale, scale),
                    sourceRect = null,
                    texture = TextureAssets.Item[item.type].Value
                };

                spriteBatch.BeginDyeShader(dye,entityShader,true);
            }
            return base.PreDrawInWorld(item, spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void PostDrawInWorld(Item item, SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            
            base.PostDrawInWorld(item, spriteBatch, lightColor, alphaColor, rotation, scale, whoAmI);
			if (dye > 0) {spriteBatch.BeginNormal(true);}
        }
    }

	public class ApplyDye : GlobalItem
	{
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.Add(new TooltipLine(Mod,"applydye","Hold weapon and right click this to dye it"));
            tooltips.Add(new TooltipLine(Mod,"applydye","Dyed item can be washed using water"));

            if (DyeServerConfig.Get.DyeReforges)
            {
                string text = DyeReforge.GetPrefixString(item.type);
                if (text != "") tooltips.Add(new TooltipLine(Mod,"dyeReforge"," + "+text) { OverrideColor = Color.LightGreen} );
                tooltips.Add(new TooltipLine(Mod,"dyeCommonPrefix",DyeReforge.playerStats[item.dye].GetStatText(item)));
            }
        }

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            // THE AMOUNT OF SHIT THIS THING CAUSE TO BUG OUT THE GAME IS HUGE
            return entity.dye > 0 && entity.type != ItemID.ColorOnlyDye;
        }

        public override void RightClick(Item item, Player player)
        {
            if (Main.mouseItem != null) 
			{
				Main.mouseItem.TryGetGlobalItem<DyedItem>(out DyedItem dyedItem);
				if (dyedItem != null) 
				{
					dyedItem.dye = item.dye;
                    SoundEngine.PlaySound(SoundID.Splash);
				}
			}
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (DyeClientConfig.Get.Debug)
            ChatManager.DrawColorCodedString(spriteBatch,FontAssets.MouseText.Value,item.type+" / "+item.dye,position,Color.White,0f,Vector2.One,Vector2.One);
        }

        public override bool CanRightClick(Item item)
        {
			if (Main.mouseItem != null && Main.mouseItem.damage > 0 && Main.mouseItem.dye <= 0) {
				return true;
			}

            return false;
        }
    }
    
}