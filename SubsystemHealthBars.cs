using Engine;
using Engine.Graphics;
using Game;
using GameEntitySystem;

namespace HealthBarMod
{
    // Widget a pantalla completa que dibuja barras de vida sobre las criaturas.
    public class HealthBarWidget : Widget
    {
        const float MaxDistance = 15f;

        readonly ComponentPlayer m_player;
        readonly SubsystemCreatureSpawn m_creatureSpawn;

        public HealthBarWidget(ComponentPlayer player)
        {
            m_player = player;
            m_creatureSpawn = player.Project.FindSubsystem<SubsystemCreatureSpawn>(true);
            IsHitTestVisible = false;
        }

        public override void MeasureOverride(Vector2 parentAvailableSize)
        {
            IsDrawRequired = true;
            DesiredSize = parentAvailableSize;
        }

        public override void Draw(DrawContext dc)
        {
            Camera camera = m_player.GameWidget.ActiveCamera;
            if (camera == null) return;

            FlatBatch2D batch = dc.PrimitivesRenderer2D.FlatBatch(
                1, DepthStencilState.None, RasterizerState.CullNoneScissor, BlendState.AlphaBlend);

            Matrix viewProj = camera.ViewProjectionMatrix;

            foreach (ComponentCreature creature in m_creatureSpawn.Creatures)
            {
                ComponentHealth health = creature.ComponentHealth;
                if (health == null || health.Health <= 0f || health.Health >= 1f) continue;

                Vector3 body = creature.ComponentBody.Position;
                Vector3 pos = new Vector3(body.X, creature.ComponentBody.BoundingBox.Max.Y + 0.3f, body.Z);

                float dist = Vector3.Distance(camera.ViewPosition, pos);
                if (dist > MaxDistance) continue;

                Vector4 clip = Vector4.Transform(new Vector4(pos, 1f), viewProj);
                if (clip.W <= 0.01f) continue;

                float nx = clip.X / clip.W;
                float ny = clip.Y / clip.W;
                if (nx < -1f || nx > 1f || ny < -1f || ny > 1f) continue;

                Vector2 local = new Vector2((nx * 0.5f + 0.5f) * ActualSize.X, (0.5f - ny * 0.5f) * ActualSize.Y);
                Vector2 center = Vector2.Transform(local, GlobalTransform);

                float width = MathUtils.Clamp(600f / dist, 24f, 90f);
                float height = 6f;
                Vector2 topLeft = center - new Vector2(width / 2f, height / 2f);
                Vector2 bottomRight = center + new Vector2(width / 2f, height / 2f);
                Vector2 fillRight = new Vector2(topLeft.X + width * health.Health, bottomRight.Y);

                Color fillColor = health.Health > 0.5f ? new Color(60, 200, 60)
                                : health.Health > 0.25f ? new Color(230, 200, 40)
                                : new Color(210, 50, 50);

                batch.QueueQuad(topLeft, bottomRight, 0f, new Color(40, 40, 40, 200));
                batch.QueueQuad(topLeft, fillRight, 0f, fillColor);
            }
        }
    }
}
