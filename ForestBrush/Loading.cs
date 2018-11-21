using ICities;

namespace ForestBrush
{
    public class LoadingExtension : LoadingExtensionBase
    {
        private AppMode appMode;

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);
            appMode = loading.currentMode;
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);
            if(appMode == AppMode.Game || appMode == AppMode.MapEditor) ForestBrushMod.instance.Initialize();
        }

        public override void OnReleased()
        {
            base.OnReleased();
            ForestBrushMod.instance.CleanUp();
        }
    }
}
