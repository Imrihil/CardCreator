using CardCreator.Features.Cards.Model;
using CardCreator.Features.Images;
using CardCreator.View;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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

    public class CardGeneratingHandler : IRequestHandler<CardGeneratingCommand, bool>
    {
        private readonly IMediator mediator;
        private readonly IImageProvider imageProvider;
        private readonly ProcessWindow processWindow;

        public CardGeneratingHandler(IMediator mediator, IImageProvider imageProvider, ProcessWindow processWindow)
        {
            this.mediator = mediator;
            this.imageProvider = imageProvider;
            this.processWindow = processWindow;
        }

        public async Task<bool> Handle(CardGeneratingCommand request, CancellationToken cancellationToken)
        {
            processWindow.RegisterCancelationToken(request.Cts);
            processWindow.Show();

            if (!File.Exists(request.FilePath))
            {
                processWindow.LogMessage($"File {request.FilePath} not exists, so action cannot be processed.");
            }

            try
            {
                var readCardFile = await mediator.Send(new ReadCardFileCommand(processWindow, request.FilePath));
                var cardSchema = new CardSchema(processWindow, imageProvider, readCardFile.CardSchemaParams);
            }
            catch (Exception)
            {
                return false;
            }

            return await Task.FromResult(false);
        }
    }
}
