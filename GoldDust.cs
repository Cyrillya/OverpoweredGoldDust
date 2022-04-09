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
            Projectile.CloneDefaults(ProjectileID.PurificationPowder);
            Projectile.aiStyle = -1;
        }

        public override void AI() {
            Projectile.velocity *= 0.95f;
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] == 180f)
                Projectile.Kill();

            if (Projectile.ai[1] == 0f) {
                Projectile.ai[1] = 1f;
                for (int i = 0; i < 30; i++) {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GoldDustDust>(), Projectile.velocity.X, Projectile.velocity.Y, 50);
                }
            }

            // turn stones into gold.
            int startX = ((int)Projectile.Left.X >> 4) - 1;
            int endX = ((int)Projectile.Right.X >> 4) + 2;
            int startY = ((int)Projectile.Top.Y >> 4) - 1;
            int endY = ((int)Projectile.Bottom.Y >> 4) + 2;
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
                    if (!(Projectile.Right.X > worldPosition.X) || !(Projectile.Left.X < worldPosition.X + 16f) || !(Projectile.Bottom.Y > worldPosition.Y) || !(Projectile.Top.Y < worldPosition.Y + 16f) || Main.myPlayer != Projectile.owner || !Main.tile[i, j].HasTile)
                        continue;

                    Tile t = Main.tile[i, j];
                    if (TileID.Sets.Conversion.Stone[t.TileType]) {
                        t.TileType = (ushort)WorldGen.SavedOreTiers.Gold; // Gold or platium? The world tells.
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j);
                    }

                }
            }
        }

        public static readonly bool[] IsBunnyNotGold = NPCID.Sets.Factory.CreateBoolSet(NPCID.Bunny, NPCID.GemBunnyTopaz, NPCID.GemBunnySapphire, NPCID.GemBunnyRuby, NPCID.GemBunnyEmerald, NPCID.GemBunnyDiamond, NPCID.GemBunnyAmethyst, NPCID.GemBunnyAmber, NPCID.ExplosiveBunny, NPCID.BunnySlimed, NPCID.BunnyXmas, NPCID.CorruptBunny, NPCID.CrimsonBunny, NPCID.PartyBunny);
        public static readonly bool[] IsSquirrelNotGold = NPCID.Sets.Factory.CreateBoolSet(NPCID.Squirrel, NPCID.SquirrelRed, NPCID.GemSquirrelTopaz, NPCID.GemSquirrelSapphire, NPCID.GemSquirrelRuby, NPCID.GemSquirrelEmerald, NPCID.GemSquirrelDiamond, NPCID.GemSquirrelAmethyst, NPCID.GemSquirrelAmber);
        public static readonly bool[] IsButterflyNotGold = NPCID.Sets.Factory.CreateBoolSet(NPCID.Butterfly, NPCID.HellButterfly, NPCID.EmpressButterfly);
        public static readonly bool[] IsBirdNotGold = NPCID.Sets.Factory.CreateBoolSet(NPCID.Bird, NPCID.BirdBlue, NPCID.BirdRed);
        public static readonly bool[] IsSlime = NPCID.Sets.Factory.CreateBoolSet(1, 147, 537, 184, 204, 16, 59, 71, 667, 50, 535, 225, 302, 333, 334, 335, 336, 141, 81, 121, 183, 122, 138, 244, 657, 658, 659, 660, 304);

        public override void ModifyDamageHitbox(ref Rectangle hitbox) {
            if (Main.netMode != NetmodeID.MultiplayerClient) {
                var myRect = hitbox;
                foreach (var npc in from n in Main.npc where n.active && myRect.Intersects(n.getRect()) select n) {
                    if (IsSlime[npc.type] && !npc.boss && npc.aiStyle == NPCAIStyleID.Slime && npc.type != NPCID.GoldenSlime) {
                        npc.Transform(NPCID.GoldenSlime);
                    }
                    if (IsBunnyNotGold[npc.type]) {
                        npc.Transform(NPCID.GoldBunny);
                    }
                    if (IsSquirrelNotGold[npc.type]) {
                        npc.Transform(NPCID.SquirrelGold);
                    }
                    if (IsButterflyNotGold[npc.type]) {
                        npc.Transform(NPCID.GoldButterfly);
                    }
                    if (IsBirdNotGold[npc.type]) {
                        npc.Transform(NPCID.GoldBird);
                    }
                    if (NPCID.Sets.IsDragonfly[npc.type] && npc.type != NPCID.GoldDragonfly) {
                        npc.Transform(NPCID.GoldDragonfly);
                    }
                    if (npc.type == NPCID.Frog) {
                        npc.Transform(NPCID.GoldFrog);
                    }
                    if (npc.type == NPCID.Goldfish) {
                        npc.Transform(NPCID.GoldGoldfish);
                    }
                    if (npc.type == NPCID.GoldfishWalker) {
                        npc.Transform(NPCID.GoldGoldfishWalker);
                    }
                    if (npc.type == NPCID.Grasshopper) {
                        npc.Transform(NPCID.GoldGrasshopper);
                    }
                    if (npc.type == NPCID.LadyBug) {
                        npc.Transform(NPCID.GoldLadyBug);
                    }
                    if (npc.type == NPCID.Mouse) {
                        npc.Transform(NPCID.GoldMouse);
                    }
                    if (npc.type == NPCID.Seahorse) {
                        npc.Transform(NPCID.GoldSeahorse);
                    }
                    if (npc.type == NPCID.WaterStrider) {
                        npc.Transform(NPCID.GoldWaterStrider);
                    }
                    if (npc.type == NPCID.Worm || npc.type == NPCID.TruffleWorm || npc.type == NPCID.TruffleWormDigger) {
                        npc.Transform(NPCID.GoldWorm);
                    }
                }
            }
        }
    }
}
