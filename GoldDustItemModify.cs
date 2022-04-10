using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OverpoweredGoldDust
{
    internal class GoldDustItemModify : GlobalItem
    {
        public override void SetDefaults(Item item) {
            if (item.type != ItemID.GoldDust)
                return;

            item.consumable = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.shootSpeed = 4f;
            item.shoot = ModContent.ProjectileType<GoldDust>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.width = 16;
            item.height = 24;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            if (item.type != ItemID.GoldDust)
                return;

            var tooltip = new TooltipLine(mod, "GoldDustAddition", Language.GetTextValue("Mods.OverpoweredGoldDust.ItemTooltip.GoldDust"));
            for (int i = 0; i < tooltips.Count; i++) {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "Material") {
                    tooltips.Insert(i + 1, tooltip);
                    return;
                }
            }
            tooltips.Add(tooltip);
        }

        public override void AddRecipes() {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.GoldDust);

            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.GoldDust);
        }
    }
}
