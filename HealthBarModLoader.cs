using Game;
using GameEntitySystem;

namespace HealthBarMod
{
    public class HealthBarModLoader : ModLoader
    {
        public override void __ModInitialize()
        {
            ModsManager.RegisterHook("OnProjectLoaded", this);
        }

        public override void OnProjectLoaded(Project project)
        {
            // Punto a ajustar segun tu version del API: aqui se registra el subsistema.
            var sub = new SubsystemHealthBars();
            project.AddSubsystem(sub);
        }
    }
}
