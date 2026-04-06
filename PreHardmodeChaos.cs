using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System;
using System.Linq;

namespace PreHardmodeChaos
{
    [ApiVersion(2, 1)]
    public class PreHardmodeChaos : TerrariaPlugin
    {
        public override string Name => "Pre-Hardmode Boss Variations";
        public override string Author => "AI Assistant";
        public override string Description => "Serangan variasi untuk semua boss Pre-Hardmode";
        public override Version Version => new(1, 0, 0);

        public PreHardmodeChaos(Main game) : base(game) { }

        public override void Initialize()
        {
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void OnUpdate(EventArgs args)
        {
            // Perbaikan: Cek pemain menggunakan TShock atau Main.player
            if (Main.gameMenu || !Main.player.Any(p => p != null && p.active)) return;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active) continue;

                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        if (Main.GameUpdateCount % 240 == 0)
                            ShootProjectiles(npc, ProjectileID.SpikedSlimeSpike, 6, 10f);
                        break;

                    case NPCID.EyeofCthulhu:
                        // Perbaikan: Ganti NextBool(150) dan hapus GetSource_FromAI
                        if (npc.life < (npc.lifeMax / 2) && Main.rand.Next(150) == 0)
                            NPC.NewNPC(null, (int)npc.Center.X, (int)npc.Center.Y, NPCID.ServantofCthulhu);
                        break;

                    case NPCID.EaterofWorldsHead:
                        // Perbaikan: VileSpitHostile (ID 307)
                        if (Main.rand.Next(200) == 0)
                            ShootProjectiles(npc, 307, 1, 8f);
                        break;

                    case NPCID.BrainofCthulhu:
                        // Perbaikan: BrainLaser (ID 290)
                        if (npc.life < (npc.lifeMax / 2) && Main.rand.Next(100) == 0)
                            ShootProjectiles(npc, 290, 3, 7f);
                        break;

                    case NPCID.QueenBee:
                        if (Main.GameUpdateCount % 300 == 0)
                            ShootProjectiles(npc, ProjectileID.Stinger, 5, 5f);
                        break;

                    case NPCID.SkeletronHead:
                        if (Main.GameUpdateCount % 400 == 0)
                            ShootProjectiles(npc, ProjectileID.ShadowFlameArrow, 8, 12f);
                        break;

                    case NPCID.WallofFlesh:
                        if (Main.GameUpdateCount % 180 == 0)
                            ShootProjectiles(npc, ProjectileID.DeathLaser, 2, 15f);
                        break;
                }
            }
        }

        private void ShootProjectiles(NPC npc, int projId, int count, float speed)
        {
            for (int i = 0; i < count; i++)
            {
                float rotation = (float)(Main.rand.NextDouble() * Math.PI * 2);
                Vector2 vel = new Vector2((float)Math.Cos(rotation) * speed, (float)Math.Sin(rotation) * speed);
                
                // Perbaikan: Gunakan overload NewProjectile yang didukung Server (tanpa EntitySource)
                int p = Projectile.NewProjectile(null, npc.Center.X, npc.Center.Y, vel.X, vel.Y, projId, 15, 0f, Main.myPlayer);
                
                if (p < 1000) // Pastikan ID proyektil valid
                {
                    Main.projectile[p].hostile = true;
                    Main.projectile[p].friendly = false;
                    
                    // Beritahu client bahwa ada proyektil baru
                    NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, p);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            base.Dispose(disposing);
        }
    }
}
