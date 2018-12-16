using ChessCore;

namespace Presentation
{
    public class ReactiveModelViewModelBase<TModel> : ReactiveViewModelBase
        where TModel : Model
    {
        public TModel Model { get; }

        public ReactiveModelViewModelBase(TModel model)
        {
            Guard.ArgumentNotNull(model, nameof(model));
            Model = model;
        }
    }
}
