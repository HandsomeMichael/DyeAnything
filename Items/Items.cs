using System.Collections.Generic;
using DyeAnything.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace DyeAnything.Items
{

	class DyeTraderSellShit : GlobalNPC
	{
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
        {
            return entity.type == NPCID.DyeTrader;
        }

        public override void ModifyShop(NPCShop shop)
        {
            shop.Add<DyeSprayer>();
			shop.Add<RandomDye>();
        }
    }
	// uncraftable due to epilepsy
	public class SuperIntenseRandomDye : RandomDye
	{
		public override string Texture => $"Terraria/Images/Item_{ItemID.GrimDye}";
        public override void UpdateDye()
        {
            timer++;
			if (timer > 3) {
				Item.dye = Main.rand.Next(DyeAnything.dyeList);
				timer = 0;
			}
        }
    }
	public class IntenseRandomDye : RandomDye
	{
        public override string Texture => $"Terraria/Images/Item_{ItemID.GelDye}";
        public override void UpdateDye()
        {
            timer++;
			if (timer > 15) {
				Item.dye = Main.rand.Next(DyeAnything.dyeList);
				timer = 0;
			}
        }
    }
	public class RandomDye : ModItem
	{
		public override void SetDefaults() {
			Item.CloneDefaults(ItemID.GelDye);
			ItemID.Sets.ShimmerTransformToItem[Item.type] = ModContent.ItemType<IntenseRandomDye>();
		}
		public int timer;

		public virtual void UpdateDye()
		{
			timer++;
			if (timer > 30) {
				Item.dye = Main.rand.Next(DyeAnything.dyeList);
				timer = 0;
			}
		}

		public override void UpdateInventory(Player player) 
		{
			UpdateDye();
		}

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            UpdateDye();
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			spriteBatch.BeginDyeShader(Item.dye,Item,true,true);
			return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			spriteBatch.BeginNormal(true,true);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
			spriteBatch.BeginDyeShader(Item.dye,Item,true);
			return true;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
			spriteBatch.BeginNormal(true);
        }
	}
	public class MagicDyer : ModItem
	{
		public override void SetDefaults() 
		{
			Item.width = 32;
			Item.height = 38;
			Item.value = 1000;
			Item.accessory = true;
			Item.rare = ItemRarityID.LightRed;
		}
		public override void UpdateAccessory(Player player, bool hideVisual) 
		{
			if (player == null || !player.active) return;
			player.TryGetModPlayer<DyePlayer>(out DyePlayer dyePlayer);
			if (dyePlayer == null) return;

			int b = 0;
			for (int i = 0; i < player.armor.Length; i++)
			{
				if (player.armor[i] == Item) 
				{
					b = i;
					break;
				}
			}
			dyePlayer.dye = player.dye[b].dye;
		}
	}

	public class DyeSprayer : ModItem
	{
        public override void SetDefaults() 
		{
			Item.width = 40; // hitbox width of the item
			Item.height = 20; // hitbox height of the item
			Item.useStyle = ItemUseStyleID.Shoot; // how you use the item (swinging, holding out, etc)
			Item.noMelee = true; //so the item's animation doesn't do damage
			Item.value = 10000; // how much the item sells for (measured in copper)
			Item.rare = ItemRarityID.Green; // the color that the item's name will be in-game
			Item.UseSound = SoundID.Item11; // The sound that this item plays when used.
			Item.autoReuse = true; // if you can hold click to automatically use it again
			Item.useAnimation = 23;
			Item.useTime = 23;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<DyeSprayerProj>();
			Item.shootSpeed = 16f;
		}
		public override void ModifyTooltips(List<TooltipLine> tooltips) 
		{
			string text = (dye == 0 ? "[c/F62B2B:None]": $"[i:{DyeAnything.dyeToItemID[dye]}]");
			tooltips.Add(new TooltipLine(Mod, "amogus", $"Current dye : {text}"));
			tooltips.Add(new TooltipLine(Mod, "boom", "'boom boom colorfull'"));
		}
		private int dye;
		public override void UpdateInventory(Player player) 
		{
			dye = 0;
			foreach (var i in player.inventory)
			{
				if (i != null && i.dye > 0) {
					dye = i.dye;
					return;
				}
			}
		}
		public override Vector2? HoldoutOffset() => new Vector2(-13f,0f);

        public override bool CanShoot(Player player) => dye > 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			position += velocity;
			Projectile.NewProjectile(source,position,velocity,type,damage,knockback,player.whoAmI,0f,dye);
			return false;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
			Texture2D extraTexture = ModContent.Request<Texture2D>(Texture+"_dye").Value;

			spriteBatch.BeginDyeShader(dye,Item,true,true);
			spriteBatch.Draw(extraTexture,position,frame,drawColor,0f,origin,scale, SpriteEffects.None, 0f);
			spriteBatch.BeginNormal(true,true);
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Texture2D extraTexture = ModContent.Request<Texture2D>(Texture+"_dye").Value;
			Vector2 orig = extraTexture.Size() / 2f;

			spriteBatch.BeginDyeShader(dye,Item,true);
			spriteBatch.Draw(extraTexture,Item.Center - Main.screenPosition,null,lightColor,rotation,orig,scale,SpriteEffects.None,0f);
			spriteBatch.BeginNormal(true);
        }
    }

	public class DyeSprayerProj : ModProjectile
	{
		// public override string Texture => "Terraria/Projectile_"+ProjectileID.PurificationPowder;
		public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.PurificationPowder}";
		public override void SetDefaults() 
		{
			Projectile.CloneDefaults(ProjectileID.PurificationPowder);
			Projectile.aiStyle = -1;
			Projectile.width = 5;
			Projectile.height = 5;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.timeLeft = 120;
		}
		public float dye {get => Projectile.ai[1];set => Projectile.ai[1] = value;}
		// public float timer {get => Projectile.ai[0];set => Projectile.ai[0] = value;}
		public override void AI() 
		{

			Player player = Main.player[Projectile.owner];
			Vector2 Center = Projectile.Center;
			Projectile.width += 1;
			Projectile.height += 1;
			Projectile.velocity *= 0.98f;

			if (Projectile.height > 60) {
				Projectile.width = 60;
				Projectile.height = 60;
				Projectile.Kill();
			}
			Projectile.Center = Center;
			
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Main.dust[Dust.NewDust(new Vector2(Projectile.Hitbox.X, Projectile.Hitbox.Y), Projectile.Hitbox.Width, Projectile.Hitbox.Height, DustID.TheDestroyer, 0f, 0f, 100, default(Color), 1f)];
				dust.noGravity = true;
				dust.noLight = true;
				dust.shader = GameShaders.Armor.GetSecondaryShader((int)dye, Main.LocalPlayer);
			}
			
			for (int i = 0; i < Main.maxNPCs; i++)
            {
                // bool flag = true;
                // bool flag = !Main.npc[i].HasBuff(BuffID.Wet);
                // if (!PepeConfig.get.weakWater) {flag = true;}

                NPC npc = Main.npc[i];
                if (!npc.active || !npc.Hitbox.Intersects(Projectile.Hitbox)){continue;}

                npc.TryGetGlobalNPC<DyedNPC>(out DyedNPC dyeNPC);
                if (dyeNPC != null && dyeNPC.dye != (int)dye)
                {
                    for (int a = 0; a < 30; a++)
                    {
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Dust dust = Dust.NewDustPerfect(Main.npc[i].Center, 182, speed * Main.rand.NextFloat(1f, 3f), Scale: 1.5f);
                        dust.noGravity = true;
                        dust.noLight = true;
                        dust.shader = GameShaders.Armor.GetSecondaryShader((int)dye, Main.LocalPlayer);
                    }
					SoundEngine.PlaySound(SoundID.Shimmer1,Projectile.Center);
                    dyeNPC.dye = (int)dye;
                }
            }
        }
	}
}