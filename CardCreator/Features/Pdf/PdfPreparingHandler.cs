using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Pdf
{
    public class PdfPreparingHandler : IRequestHandler<PdfPreparingCommand, bool>
    {
        public async Task<bool> Handle(PdfPreparingCommand request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(false);
        }
    }
}
