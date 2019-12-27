using MediatR;
using System.Threading;

namespace CardCreator
{
    public class PdfPreparingCommand : IRequest<bool>
    {
        private string FileName { get; set; }
        private CancellationTokenSource Cts { get; set; }

        public PdfPreparingCommand(string fileName, CancellationTokenSource cts)
        {
            FileName = fileName;
            Cts = cts;
        }
    }
}
