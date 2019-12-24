using MediatR;
using System.Threading;

namespace CardCreator.Features.Cards
{
    public class CardGeneratingCommand : IRequest<bool>
    {
        public string FilePath { get; }
        public CancellationTokenSource Cts { get; }

        public CardGeneratingCommand(string filePath, CancellationTokenSource cts)
        {
            FilePath = filePath;
            Cts = cts;
        }
    }
}
