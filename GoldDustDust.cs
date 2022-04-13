using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OverpoweredGoldDust
{
    internal class GoldDustDust : ModDust
    {
        public override void SetStaticDefaults() {
            UpdateType = DustID.VilePowder;
        }

        public override void OnSpawn(Dust dust) {
            dust.noLightEmittence = true; // avoid vanilla color adding
        }

        public override bool Update(Dust dust) {
            float lightScale = dust.scale * 0.8f;
            if (lightScale > 1f)
                lightScale = 1f;
            Lighting.AddLight((int)(dust.position.X / 16f), (int)(dust.position.Y / 16f), lightScale, lightScale * 0.7f, lightScale * 0.2f);
            return base.Update(dust);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor) {
            return new Color(lightColor.R, lightColor.G, lightColor.B, 25);
        }
    }
}
