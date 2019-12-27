using MediatR;
using System.Threading;

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
}
