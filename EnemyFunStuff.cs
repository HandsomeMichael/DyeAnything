using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.ModLoader;

namespace DyeAnything
{
    class EnemyFunStuff : GlobalNPC
    {

        internal static class FunID
        {
            public const int None = 0;
            public const int OnFire = 1;
            public const int NoWet = 2;
            public const int Gigantic = 3;
            public const int Count = 16;
        }

        int funny = -1;

        public override bool InstancePerEntity => true;
        protected override bool CloneNewInstances => true;

        public override void ModifyTypeName(NPC npc, ref string typeName)
        {
            switch (funny)
            {
                // case FunID.None: typeName += "";break;
                case FunID.OnFire: typeName += " Fired Up";break;
                case FunID.NoWet: typeName += " Dried Up";break;
                case FunID.Gigantic: typeName += " Humangous";break;
                default:break;
            }
        }
        public override bool IsLoadingEnabled(Mod mod) => DyeServerConfig.Get.EnemyFunStuff;

        public void InitFunny(NPC entity)
        {
            funny = Main.rand.Next(FunID.None,FunID.Count);

            switch (funny)
            {
                case FunID.OnFire:
                entity.lifeMax += 100;
                break;
                case FunID.Gigantic:
                entity.scale += Main.rand.NextFloat(0.5f,1f);
                break;
                default:break;
            }
        }

        public override void AI(NPC npc)
        {
            if (funny == -1)
            {
                InitFunny(npc);
                return;
            }

            // actual funny moment
            switch (funny)
            {
                case FunID.OnFire:
                npc.onFire = true;
                break;
                case FunID.NoWet:
                npc.wet = false;
                break;
                default:break;
            }
        }

    }
}