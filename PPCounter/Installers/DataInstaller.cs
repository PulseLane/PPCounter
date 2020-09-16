using PPCounter.Data;
using Zenject;

namespace PPCounter.Installers
{
    class DataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Logger.log.Debug($"Binding {nameof(PPData)}");
            Container.BindInterfacesAndSelfTo<PPData>().AsSingle().NonLazy();
        }
    }
}
