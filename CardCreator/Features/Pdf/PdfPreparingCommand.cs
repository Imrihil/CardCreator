using CardCreator.View;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator
{
    public class PdfPreparingCommand : IRequest<bool>
    {
        public string FileName { get; set; }
        public CancellationTokenSource Cts { get; set; }

        public PdfPreparingCommand(string fileName, CancellationTokenSource cts)
        {
            FileName = fileName;
            Cts = cts;
        }
    }

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
