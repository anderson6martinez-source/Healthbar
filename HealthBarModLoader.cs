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

        public override void OnPlayerSpawned(PlayerData.SpawnMode spawnMode, ComponentPlayer componentPlayer, Vector3 position)
        {
            ContainerWidget container = componentPlayer.ComponentGui.ControlsContainerWidget;
            foreach (Widget w in container.Children)
            {
                if (w is HealthBarWidget) return;
            }
            container.AddChildren(new HealthBarWidget(componentPlayer));
        }
    }
}
