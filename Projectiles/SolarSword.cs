
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DyeAnything.Projectiles
{
    class SolarSword : ModProjectile
    {
		public override string Texture => $"Terraria/Images/Item_{ItemID.FieryGreatsword}";
		public override void SetDefaults() 
        {
			//Projectile.CloneDefaults(ProjectileID.WoodenBoomerang);
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = false;
			Projectile.hostile = false;
			Projectile.aiStyle = -1;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
            Projectile.Opacity = 0f;
            // Projectile.alpha = 255;
			//aiType = ProjectileID.WoodenBoomerang;
		}

        const float maxTimer = 80f;
		public int target {get => (int)Projectile.ai[1];set => Projectile.ai[1] = value;}
		public float timer {get => Projectile.ai[0];set => Projectile.ai[0] = value;}

        public override bool ShouldUpdatePosition() => target == -1;

        public override void AI() 
		{

            
            // Just move
            if (target < 0 || target > Main.maxNPCs) return;

            // Find the NPC
            NPC npc = Main.npc[target];
            if (npc == null || !npc.active)
            {
                target = -1;
                return;
            }



            // Smooth appereances

            timer++;
            Projectile.position.Y += 0.5f * (1 - (timer /  maxTimer));
            Projectile.Opacity += 0.1f;
            Projectile.velocity = Projectile.DirectionTo(npc.Center) * 5f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Projectile.Opacity += 0.1f;
            if (timer > maxTimer) target = -1;

        }
    }
}