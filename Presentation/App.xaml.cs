using System.Windows;
using ChessCore;
using CommonServiceLocator;
using Prism.Ioc;
using Prism.Unity;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IStartingPiecesSetFactory, ClassicStartingPiecesSetFactory>();
            containerRegistry.Register<IBoardFactory, BoardFactory>();
            containerRegistry.Register<IImagesFactory, ImagesFactory>();
        }

        protected override Window CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }
    }
}

