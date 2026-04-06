using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace PreHardmodeChaos;

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
        if (Main.gameMenu || !Netplay.AnyPlayers) return;

        for (int i = 0; i < Main.maxNPCs; i++)
        {
            NPC npc = Main.npc[i];
            if (!npc.active) continue;

            switch (npc.type)
            {
                case NPCID.KingSlime:
                    // King Slime melompat dan menciptakan ledakan slime berduri (Spiked Slime)
                    if (Main.GameUpdateCount % 240 == 0) // Setiap 4 detik
                        ShootProjectiles(npc, ProjectileID.SpikedSlimeSpike, 6, 10f);
                    break;

                case NPCID.EyeofCthulhu:
                    // Saat HP < 50%, menembakkan Servant of Cthulhu lebih cepat
                    if (npc.life < (npc.lifeMax / 2) && Main.rand.NextBool(150))
                        NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.ServantofCthulhu);
                    break;

                case NPCID.EaterofWorldsHead:
                    // Kepala Eater menembakkan Vile Spit saat mendekat
                    if (Main.rand.NextBool(200))
                        ShootProjectiles(npc, ProjectileID.VileSpitHostile, 1, 8f);
                    break;

                case NPCID.BrainofCthulhu:
                    // Brain menembakkan laser ilusi (seperti Creeper)
                    if (npc.life < (npc.lifeMax / 2) && Main.rand.NextBool(100))
                        ShootProjectiles(npc, ProjectileID.BrainLaser, 3, 7f);
                    break;

                case NPCID.QueenBee:
                    // Queen Bee menjatuhkan sarang lebah (Bee Hive) dari langit-langit saat marah
                    if (Main.GameUpdateCount % 300 == 0)
                        ShootProjectiles(npc, ProjectileID.Stinger, 5, 5f);
                    break;

                case NPCID.SkeletronHead:
                    // Skeletron mengeluarkan gelombang Shadow Flame
                    if (Main.GameUpdateCount % 400 == 0)
                        ShootProjectiles(npc, ProjectileID.ShadowFlameArrow, 8, 12f);
                    break;

                case NPCID.WallofFlesh:
                    // Wall of Flesh menembakkan Hungry lebih sering dan Laser ungu
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
            
            int p = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vel, projId, 15, 0f, Main.myPlayer);
            Main.projectile[p].hostile = true;
            Main.projectile[p].friendly = false;
            
            NetMessage.SendData((int)PacketTypes.ProjectileNew, -1, -1, null, p);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
        base.Dispose(disposing);
    }
}

