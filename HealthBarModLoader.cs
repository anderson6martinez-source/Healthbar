using Engine;
using Game;
using GameEntitySystem;

namespace HealthBarMod
{
    public class HealthBarModLoader : ModLoader
    {
        public override void __ModInitialize()
        {
            ModsManager.RegisterHook("OnPlayerSpawned", this);
        }

        public override bool OnPlayerSpawned(PlayerData.SpawnMode spawnMode, ComponentPlayer componentPlayer, Vector3 position)
        {
            componentPlayer.ComponentGui.DisplaySmallMessage("HealthBar mod cargado", Color.White, false, false);
            ContainerWidget container = componentPlayer.ComponentGui.ControlsContainerWidget;
            foreach (Widget w in container.Children)
            {
                if (w is HealthBarWidget) return false;
            }
            container.AddChildren(new HealthBarWidget(componentPlayer));
            return false;
        }
    }
}
