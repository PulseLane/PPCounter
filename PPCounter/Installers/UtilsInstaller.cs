using PPCounter.Utilities;
using Zenject;

namespace PPCounter.Installers
{
    class UtilsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Logger.log.Debug($"Binding {nameof(PPUtils)}");
            Container.BindInterfacesAndSelfTo<PPUtils>().AsSingle().NonLazy();
        }
    }
}
