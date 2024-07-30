using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DyeAnything.Items
{
    /// <summary>
    /// have you ever wondered why life is such a short experience yet so expandable
    /// existence it self is too tied too history and descended experiences
    /// humanity now has achieved its peak and hardest moment
    /// and yet despite all of this , my code was still shitty
    /// </summary>
    public class ShittiestWayToCode : ModSystem
	{
		// rn the worst implementation i made
		// the only way to edit Held Item drawlayer shader is by directly injecting a code in the held item method
		// currently i dont fucking know how to IL edit

        public override void Load()
        {
            Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_27_HeldItem += ShittyPatch;
        }

		public override void Unload()
        {
            Terraria.DataStructures.On_PlayerDrawLayers.DrawPlayer_27_HeldItem -= ShittyPatch;
        }

        private void ShittyPatch(On_PlayerDrawLayers.orig_DrawPlayer_27_HeldItem orig, ref PlayerDrawSet drawinfo)
        {

			int oldCount = drawinfo.DrawDataCache.Count;

            orig(ref drawinfo);

			int newCount = drawinfo.DrawDataCache.Count;

			if (drawinfo.drawPlayer.JustDroppedAnItem)  return;

			Item heldItem = drawinfo.heldItem;
			if (heldItem == null) return;

			int shaderNum = GetShader(heldItem);
			if (shaderNum <= 0) return;

			for (int i = oldCount; i < newCount; i++)
			{
				DrawData cloneData = drawinfo.DrawDataCache[i];
				cloneData.shader = shaderNum;
				drawinfo.DrawDataCache.Add(cloneData);
			}
        }

        private static int GetShader(Item heldItem)
        {
            if (heldItem.TryGetGlobalItem<DyedItem>(out DyedItem dyedItem))
			{
				return dyedItem.dye;
			}
			return 0;
        }

		public static void TryGetShader(ref DrawData item, Item heldItem)
		{
			if (heldItem.TryGetGlobalItem<DyedItem>(out DyedItem dyedItem))
			{
				if (dyedItem.dye > 0)
				{
					item.shader = dyedItem.dye;
				}
			}
		}
	}
}

		// Blud tried to use GPT and fail hard 

		// public void HeldItemEdit(ILContext context)
		// {
		// 	var il = new ILCursor(context);
		// 	var drawDataCacheAddMethod = typeof(System.Collections.Generic.List<DrawData>).GetMethod("Add");
		// 	var tryGetShaderMethod = typeof(ShittiestWayToCode).GetMethod("TryGetShader");

		// 	bool found = false;

		// 	while (il.Next != null)
		// 	{
		// 		if (il.Next.OpCode == OpCodes.Callvirt && il.Next.Operand is MethodReference methodRef && methodRef.FullName == context.Module.ImportReference(drawDataCacheAddMethod).FullName)
		// 		{
		// 			found = true;

		// 			// Move to the instruction before Callvirt to get the DrawData variable
		// 			il.GotoPrev(MoveType.Before, i => i.OpCode == OpCodes.Ldloc && ((VariableDefinition)i.Operand).VariableType.Name == "DrawData");

		// 			// Duplicate the item on the stack to pass it by reference
		// 			il.Emit(OpCodes.Dup);

		// 			// Load the heldItem from the argument
		// 			il.Emit(OpCodes.Ldarg_1);
		// 			il.Emit(OpCodes.Ldfld, typeof(PlayerDrawSet).GetField("heldItem"));

		// 			// Call TryGetShader
		// 			il.Emit(OpCodes.Call, context.Module.ImportReference(tryGetShaderMethod));

		// 			// Move the cursor forward to avoid reprocessing the same instruction
		// 			il.Index++;
		// 		}
		// 		else
		// 		{
		// 			il.Index++;
		// 		}
		// 	}

		// 	if (!found)
		// 	{
		// 		throw new Exception("Failed to find 'DrawDataCache.Add' call in DrawPlayer_27_HeldItem method.");
		// 	}
		// }
