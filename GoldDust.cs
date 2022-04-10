using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace OverpoweredGoldDust
{
    internal class GoldDust : ModProjectile
    {
        public override void SetDefaults() {
            projectile.CloneDefaults(ProjectileID.PurificationPowder);
            projectile.aiStyle = -1;
        }

        public override void AI() {
            projectile.velocity *= 0.95f;
            projectile.ai[0] += 1f;
            if (projectile.ai[0] == 180f)
                projectile.Kill();

            if (projectile.ai[1] == 0f) {
                projectile.ai[1] = 1f;
                for (int i = 0; i < 30; i++) {
                    Dust.NewDust(projectile.position, projectile.width, projectile.height, ModContent.DustType<GoldDustDust>(), projectile.velocity.X, projectile.velocity.Y, 50);
                }
            }

            // turn stones into gold.
            int startX = ((int)projectile.Left.X >> 4) - 1;
            int endX = ((int)projectile.Right.X >> 4) + 2;
            int startY = ((int)projectile.Top.Y >> 4) - 1;
            int endY = ((int)projectile.Bottom.Y >> 4) + 2;
            if (startX < 0)
                startX = 0;

            if (endX > Main.maxTilesX)
                endX = Main.maxTilesX;

            if (startY < 0)
                startY = 0;

            if (endY > Main.maxTilesY)
                endY = Main.maxTilesY;

            for (int i = startX; i < endX; i++) {
                for (int j = startY; j < endY; j++) {
                    var worldPosition = new Vector2(i << 4, j << 4);
                    if (!(projectile.Right.X > worldPosition.X) || !(projectile.Left.X < worldPosition.X + 16f) || !(projectile.Bottom.Y > worldPosition.Y) || !(projectile.Top.Y < worldPosition.Y + 16f) || Main.myPlayer != projectile.owner || !Main.tile[i, j].active())
                        continue;

                    Tile t = Main.tile[i, j];
                    if (TileID.Sets.Conversion.Stone[t.type]) {
                        t.type = WorldGen.GoldTierOre; // Gold or platium? The world tells.
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j, 1);
                    }

                }
            }
        }

        public static readonly bool[] IsBunnyNotGold = NPCID.Sets.Factory.CreateBoolSet(NPCID.Bunny, NPCID.BunnySlimed, NPCID.BunnyXmas, NPCID.CorruptBunny, NPCID.CrimsonBunny, NPCID.PartyBunny);
        public static readonly bool[] IsBirdNotGold = NPCID.Sets.Factory.CreateBoolSet(NPCID.Bird, NPCID.BirdBlue, NPCID.BirdRed);

        public override void ModifyDamageHitbox(ref Rectangle hitbox) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                var myRect = hitbox;
                foreach (var npc in from n in Main.npc where n.active && myRect.Intersects(n.getRect()) select n) {
                    if (IsBunnyNotGold[npc.type]) {
                        npc.Transform(NPCID.GoldBunny);
                    }
                    if (IsBirdNotGold[npc.type]) {
                        npc.Transform(NPCID.GoldBird);
                    }
                    if (npc.type == NPCID.Squirrel) {
                        npc.Transform(NPCID.SquirrelGold);
                    }
                    if (npc.type == NPCID.Butterfly) {
                        npc.Transform(NPCID.GoldButterfly);
                    }
                    if (npc.type == NPCID.Frog) {
                        npc.Transform(NPCID.GoldFrog);
                    }
                    if (npc.type == NPCID.Grasshopper) {
                        npc.Transform(NPCID.GoldGrasshopper);
                    }
                    if (npc.type == NPCID.Mouse) {
                        npc.Transform(NPCID.GoldMouse);
                    }
                    if (npc.type == NPCID.Worm || npc.type == NPCID.TruffleWorm || npc.type == NPCID.TruffleWormDigger) {
                        npc.Transform(NPCID.GoldWorm);
                    }
                }
            }
        }
    }
}
