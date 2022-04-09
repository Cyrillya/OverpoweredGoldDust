using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OverpoweredGoldDust
{
    internal class GoldDustItemModify : GlobalItem
    {
        public override bool AppliesToEntity(Item item, bool lateInstatiation) {
            return item.type == ItemID.GoldDust;
        }

        public override void SetDefaults(Item item) {
            item.consumable = true;
            item.useAnimation = 15;
            item.useTime = 15;
            item.shootSpeed = 4f;
            item.shoot = ModContent.ProjectileType<GoldDust>();
            item.useStyle = ItemUseStyleID.Swing;
            item.width = 16;
            item.height = 24;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            int i;
            var tooltip = new TooltipLine(Mod, "GoldDustAddition", Language.GetTextValue("Mods.OverpoweredGoldDust.ItemTooltip.GoldDust"));
            for (i = 0; i < tooltips.Count; i++) {
                if (tooltips[i].mod == "Terraria" && tooltips[i].Name == "Material") {
                    tooltips.Insert(i + 1, tooltip);
                    return;
                }
            }
            tooltips.Add(tooltip);
        }

        public override void AddRecipes() {
            Mod.CreateRecipe(ItemID.GoldDust, 1)
                .AddIngredient(ItemID.GoldBar)
                .AddTile(TileID.Anvils)
                .Register();

            Mod.CreateRecipe(ItemID.GoldDust, 1)
                .AddIngredient(ItemID.PlatinumBar)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
