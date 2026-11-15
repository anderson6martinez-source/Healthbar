using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;
using TemplatesDatabase;

namespace HealthBarMod
{
    public class SubsystemHealthBars : Subsystem, IDrawable
    {
        const float MaxDistance = 15f;   // bloques
        const float BarWidth = 1.0f;
        const float BarHeight = 0.12f;

        SubsystemCreatureSpawn m_creatureSpawn;
        readonly PrimitivesRenderer3D m_renderer = new PrimitivesRenderer3D();
        static readonly int[] m_drawOrders = { 2000 };

        public int[] DrawOrders => m_drawOrders;

        public override void Load(ValuesDictionary valuesDictionary)
        {
            base.Load(valuesDictionary);
            m_creatureSpawn = Project.FindSubsystem<SubsystemCreatureSpawn>(true);
        }

        public void Draw(Camera camera, int drawOrder)
        {
            FlatBatch3D batch = m_renderer.FlatBatch(
                -1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend);

            Vector3 right = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY));
            Vector3 up = Vector3.UnitY;

            foreach (ComponentCreature creature in m_creatureSpawn.Creatures)
            {
                ComponentHealth health = creature.ComponentHealth;
                if (health == null || health.Health <= 0f || health.Health >= 1f) continue;

                Vector3 top = creature.ComponentBody.BoundingBox.Max;
                Vector3 pos = new Vector3(creature.ComponentBody.Position.X, top.Y + 0.3f, creature.ComponentBody.Position.Z);

                if (Vector3.Distance(camera.ViewPosition, pos) > MaxDistance) continue;
                if (Vector3.Dot(camera.ViewDirection, pos - camera.ViewPosition) <= 0f) continue;

                float half = BarWidth / 2f;
                float h = BarHeight;
                Vector3 l = pos - right * half;
                Vector3 r = pos + right * half;
                Vector3 fill = l + right * (BarWidth * health.Health);

                // fondo
                batch.QueueQuad(l, r, r + up * h, l + up * h, new Color(40, 40, 40, 200));
                // relleno (verde -> rojo segun vida)
                Color c = health.Health > 0.5f ? new Color(60, 200, 60) : health.Health > 0.25f ? new Color(230, 200, 40) : new Color(210, 50, 50);
                batch.QueueQuad(l, fill, fill + up * h, l + up * h, c);
            }

            m_renderer.Flush(camera.ViewProjectionMatrix);
        }
    }
}
