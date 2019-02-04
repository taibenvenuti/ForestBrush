using Harmony;
using ICities;
using System.Reflection;

namespace ForestBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private AppMode appMode;
        private HarmonyInstance harmony;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            appMode = loading.currentMode;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            
            if (appMode == AppMode.Game || appMode == AppMode.MapEditor)
            {
                bool initialized = false;
                while (!initialized)
                {
                    if (LoadingManager.instance.m_loadingComplete)
                    {
                        initialized = true;
                        harmony = HarmonyInstance.Create("com.tpb.forestbrush");
                        harmony.PatchAll(Assembly.GetExecutingAssembly());

                        ForestBrushMod.instance.Initialize();
                    }
                }
            }
        }

        public override void OnReleased()
        {
            base.OnReleased();
            ForestBrushMod.instance.CleanUp();
        }
    }
}
