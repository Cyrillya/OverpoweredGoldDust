using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace OverpoweredGoldDust
{
    internal class MagicalGoldDust : ModItem
    {
        private const int SHADER_DYE_TYPE = ItemID.ShiftingPearlSandsDye;

        public override string Texture => $"Terraria/Images/Item_{ItemID.GoldDust}";

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.GoldDust);
            Item.consumable = true;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.shootSpeed = 4f;
            Item.shoot = ModContent.ProjectileType<GoldDust>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.width = 16;
            Item.height = 24;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
        }

        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset) {
            if (line.mod == "Terraria" && line.Name == "ItemName") {
                Main.spriteBatch.End(); //end and begin main.spritebatch to apply a shader
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(SHADER_DYE_TYPE), Item, null);
                Utils.DrawBorderString(Main.spriteBatch, line.text, new Vector2(line.X, line.Y), Color.White, 1.05f);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                return false;
            }
            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(SHADER_DYE_TYPE), Item, null);

            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Vector2 drawPos = position + origin;
            spriteBatch.Draw(texture, drawPos, frame, drawColor, 0f, origin, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI) {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            GameShaders.Armor.Apply(GameShaders.Armor.GetShaderIdFromItemId(SHADER_DYE_TYPE), Item, null);

            Texture2D texture = TextureAssets.Item[Item.type].Value;
            Rectangle frame = texture.Frame();
            Vector2 frameOrigin = frame.Size() / 2f;
            Vector2 offset = new Vector2(Item.width / 2 - frameOrigin.X, Item.height - frame.Height);
            Vector2 drawPos = Item.position - Main.screenPosition + frameOrigin + offset;
            spriteBatch.Draw(texture, drawPos, frame, alphaColor, rotation, frameOrigin, scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            return false;
        }

        public override void AddRecipes() {
            CreateRecipe(3)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddTile(TileID.Furnaces)
                .Register();

            CreateRecipe(3)
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.PlatinumBar, 5)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
