using CardCreator.View;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Pdf
{
    public class PdfPreparingHandler : IRequestHandler<PdfPreparingCommand, bool>
    {
        private readonly ProcessWindow processWindow;

        public PdfPreparingHandler(ProcessWindow processWindow)
        {
            this.processWindow = processWindow;
        }

        public async Task<bool> Handle(PdfPreparingCommand request, CancellationToken cancellationToken)
        {
            processWindow.RegisterCancelationToken(request.Cts);
            processWindow.Show();

            return await Task.FromResult(false);
        }
    }
}
