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

        public static readonly int[] BasicDoorsFrameY = { 0, 1, 2, 3, 7, 10, 13, 25, 26, 28, 29, 30 };
        public static readonly int[] BasicWorkBenchesFrameX = { 0, 1, 2, 17, 18, 22, 23, 26 };
        public static readonly int[] BasicChairsFrameY = { 0, 1, 2, 3, 4, 5, 11, 29, 30 };
        public static readonly int[] BasicTablesFrameX = { 0, 1, 2, 3, 6, 8, 16, 26, 28 };
        public static readonly int[] BasicPlatformsFrameY = { 0, 1, 2, 3, 5, 17, 19, 23, 43 };
        public static readonly int[] BasicChestsFrameX = { 0, 3, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 28, 29, 30, 31, 32, 33 };
        public static readonly bool[] WoodBlocks = TileID.Sets.Factory.CreateBoolSet(TileID.WoodBlock, TileID.BorealWood, TileID.DynastyWood, TileID.LivingWood, TileID.PalmWood, TileID.SpookyWood, TileID.Ebonwood, TileID.Pearlwood, TileID.Shadewood);
        public static readonly bool[] WoodWalls = WallID.Sets.Factory.CreateBoolSet(WallID.BorealWood, WallID.Wood, WallID.BlueDynasty, WallID.WhiteDynasty, WallID.LivingWood, WallID.PalmWood, WallID.SpookyWood, WallID.Ebonwood, WallID.Pearlwood, WallID.Shadewood);
        public static readonly bool[] DirtWalls = WallID.Sets.Factory.CreateBoolSet(WallID.Dirt, WallID.DirtUnsafe, WallID.DirtUnsafe1, WallID.DirtUnsafe2, WallID.DirtUnsafe3, WallID.DirtUnsafe4, WallID.CaveUnsafe, WallID.Cave2Unsafe, WallID.Cave3Unsafe, WallID.Cave4Unsafe, WallID.Cave5Unsafe, WallID.Cave6Unsafe, WallID.Cave7Unsafe, WallID.Cave8Unsafe);
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
                    Tile t = Framing.GetTileSafely(i, j);

                    if (!(Projectile.Right.X > worldPosition.X) || !(Projectile.Left.X < worldPosition.X + 16f) || !(Projectile.Bottom.Y > worldPosition.Y) || !(Projectile.Top.Y < worldPosition.Y + 16f) || Main.myPlayer != Projectile.owner)
                        continue;

                    if (WallID.Sets.Conversion.Stone[t.WallType] || WoodWalls[t.WallType] || DirtWalls[t.WallType]) {
                        t.WallType = WallID.GoldBrick; // Gold or platium? The world tells.
                        WorldGen.SquareWallFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j);
                    }

                    if (!Main.tile[i, j].HasTile)
                        continue;

                    if (TileID.Sets.Conversion.Stone[t.TileType]) {
                        t.TileType = (ushort)WorldGen.SavedOreTiers.Gold; // Gold or platium? The world tells.
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j);
                    }

                    if (WoodBlocks[t.TileType]) {
                        t.TileType = TileID.GoldBrick; // Gold or platium? The world tells.
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j);
                    }

                    if (t.TileType == TileID.Platforms && BasicPlatformsFrameY.Contains(t.TileFrameY / 18)) {
                        Main.tile[i, j].TileFrameY = 31 * 18;
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j);
                    }

                    if (t.TileType == TileID.CopperCoinPile || t.TileType == TileID.SilverCoinPile) {
                        t.TileType = TileID.GoldCoinPile; // Gold or platium? The world tells.
                        WorldGen.SquareTileFrame(i, j);
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, i, j);
                    }

                    if ((t.TileType == TileID.Containers || t.TileType == TileID.FakeContainers) && BasicChestsFrameX.Contains(t.TileFrameX / 36)) {
                        // the left-top position of the chest
                        Point key = new Point(i, j);
                        if (t.TileFrameX % 36 != 0)
                            key.X--;
                        if (t.TileFrameY % 36 != 0)
                            key.Y--;

                        for (int k = 0; k <= 1; k++) {
                            for (int l = 0; l <= 1; l++) {
                                Main.tile[key.X + k, key.Y + l].TileFrameX = (short)(36 + 18 * k);
                            }
                        }

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                            NetMessage.SendTileSquare(-1, key.X, key.Y, 2, 2);
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

                foreach (var item in from i in Main.item where i.active && myRect.Intersects(i.getRect()) && (i.type == ItemID.CopperCoin || i.type == ItemID.SilverCoin) select i) {
                    item.SetDefaults(ItemID.GoldCoin);
                }
            }
        }
    }
}
