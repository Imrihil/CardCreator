using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public class CardGeneratingHandler : IRequestHandler<CardGeneratingCommand, bool>
    {
        public async Task<bool> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(false);
        }
    }
}
