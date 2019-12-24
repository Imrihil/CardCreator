using CardCreator.View;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public class CardGeneratingHandler : IRequestHandler<CardGeneratingCommand, bool>
    {
        private readonly ProcessWindow processWindow;

        public CardGeneratingHandler(ProcessWindow processWindow)
        {
            this.processWindow = processWindow;
        }

        public async Task<bool> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            processWindow.RegisterCancelationToken(request.Cts);
            processWindow.Show();

            return await Task.FromResult(false);
        }
    }
}
