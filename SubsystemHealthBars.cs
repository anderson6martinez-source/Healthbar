using Engine;
using Engine.Graphics;
using GameEntitySystem;
using TemplatesDatabase;

namespace Game
{
    // Componente que se agrega a cada criatura (via HealthBar.xdb) y dibuja su barra de vida.
    public class ComponentHealthBarMod : Component, IDrawable
    {
        const float MaxDistance = 20f;   // bloques
        const float BarWidth = 1.0f;
        const float BarHeight = 0.12f;
        const bool ShowWhenFull = true;  // true = tambien con la vida completa (modo prueba)

        ComponentHealth m_health;
        ComponentBody m_body;
        bool m_isPlayer;

        readonly PrimitivesRenderer3D m_renderer = new PrimitivesRenderer3D();
        static readonly int[] m_drawOrders = { 2000 };

        public int[] DrawOrders => m_drawOrders;

        public override void Load(ValuesDictionary valuesDictionary, IdToEntityMap idToEntityMap)
        {
            base.Load(valuesDictionary, idToEntityMap);
            m_health = Entity.FindComponent<ComponentHealth>(false);
            m_body = Entity.FindComponent<ComponentBody>(false);
            m_isPlayer = Entity.FindComponent<ComponentPlayer>(false) != null;
        }

        public void Draw(Camera camera, int drawOrder)
        {
            if (m_health == null || m_body == null || m_isPlayer) return;

            float life = m_health.Health;
            if (life <= 0f) return;
            if (!ShowWhenFull && life >= 1f) return;

            Vector3 top = m_body.BoundingBox.Max;
            Vector3 pos = new Vector3(m_body.Position.X, top.Y + 0.3f, m_body.Position.Z);

            if (Vector3.Distance(camera.ViewPosition, pos) > MaxDistance) return;
            if (Vector3.Dot(camera.ViewDirection, pos - camera.ViewPosition) <= 0f) return;

            Vector3 right = Vector3.Normalize(Vector3.Cross(camera.ViewDirection, Vector3.UnitY));
            Vector3 up = Vector3.UnitY;

            float half = BarWidth / 2f;
            Vector3 l = pos - right * half;
            Vector3 r = pos + right * half;
            Vector3 fill = l + right * (BarWidth * life);

            Color fillColor = life > 0.5f ? new Color(60, 200, 60)
                            : life > 0.25f ? new Color(230, 200, 40)
                            : new Color(210, 50, 50);

            FlatBatch3D batch = m_renderer.FlatBatch(
                -1, DepthStencilState.DepthRead, RasterizerState.CullNoneScissor, BlendState.AlphaBlend);

            batch.QueueQuad(l, r, r + up * BarHeight, l + up * BarHeight, new Color(40, 40, 40, 200));
            batch.QueueQuad(l, fill, fill + up * BarHeight, l + up * BarHeight, fillColor);

            m_renderer.Flush(camera.ViewProjectionMatrix);
        }
    }
}
