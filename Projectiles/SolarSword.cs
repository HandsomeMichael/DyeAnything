
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DyeAnything.Projectiles
{
    class SolarSword : ModProjectile
    {
        // public override string Texture => "Terraria/Projectile_"+ProjectileID.PurificationPowder;
		public override string Texture => $"Terraria/Images/Item_{ItemID.MoltenFury}";
		public override void SetDefaults() 
		{
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 120;
            // Projectile.Opacity = 0f;
		}

        const float appearTime = 30f;
        const float maxTimer = 60f;
		public int target {get => (int)Projectile.ai[1];set => Projectile.ai[1] = value;}
		public float timer {get => Projectile.ai[0];set => Projectile.ai[0] = value;}

        public override bool ShouldUpdatePosition() => target == -1;
        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void AI() 
		{

            Projectile.rotation = Projectile.velocity.ToRotation();

            // Just move
            if (target < 0 || target > Main.maxNPCs) return;

            // Find the NPC
            NPC npc = Main.npc[target];
            if (npc == null || !npc.CanBeChasedBy(Projectile))
            {
                target = -1;
                return;
            }


            // Smooth appereances

            timer++;
            Projectile.velocity = Projectile.DirectionTo(npc.Center) * 5f;

            Projectile.Opacity += 0.1f;
            if (timer > maxTimer) target = -1;

        }
    }
}