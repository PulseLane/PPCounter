using PPCounter.Data;
using Zenject;

namespace PPCounter.Installers
{
    class DataInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<PPDownloader>().FromNewComponentOnNewGameObject().AsSingle();

            Container.BindInterfacesAndSelfTo<PPData>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SSData>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<BeatLeaderData>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AccSaberData>().AsSingle().NonLazy();
        }
    }
}
