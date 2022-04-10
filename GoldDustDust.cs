using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OverpoweredGoldDust
{
    internal class GoldDustDust : ModDust
    {
        public override void OnSpawn(Dust dust) {
            dust.noGravity = true;
            dust.fadeIn = 2f;
        }

        public override bool Update(Dust dust) {
            dust.scale += 0.005f;
            dust.velocity *= 0.94f;
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
