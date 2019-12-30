using CardCreator.Features.Cards.Model;
using CardCreator.View;
using MediatR;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CardCreator.Features.Cards
{
    public class ReadCardFileCommand : IRequest<ReadCardFileResults>
    {
        public ProcessWindow ProcessWindow { get; set; }
        public string FilePath { get; set; }

        public ReadCardFileCommand(ProcessWindow processWindow, string filePath)
        {
            ProcessWindow = processWindow;
            FilePath = filePath;
        }
    }

    public class ReadCardFileHandler : IRequestHandler<ReadCardFileCommand, ReadCardFileResults>
    {
        public async Task<ReadCardFileResults> Handle(ReadCardFileCommand request, CancellationToken cancellationToken)
        {
            var results = new ReadCardFileResults();

            try
            {
                using var xlPackage = new ExcelPackage(new FileInfo(request.FilePath));

                var worksheet = xlPackage.Workbook.Worksheets.First();
                var totalColumns = worksheet.Dimension.End.Column;
                var totalRows = worksheet.Dimension.End.Row;

                results.CardSchemaParams = ListFromRange(worksheet, 1, 1, CardSchema.ParamsNumber, 1, true).First();
                results.ElementSchemasParams = ListFromRange(worksheet, 1, 2, ElementSchema.ParamsNumber, totalColumns, true);
                results.CardsElements = ListFromRange(worksheet, ElementSchema.ParamsNumber + 1, 2, totalRows, totalColumns);
            }
            catch (Exception ex)
            {
                request.ProcessWindow.LogMessage(ex.ToString());
                throw;
            }

            request.ProcessWindow.LogMessage($"Plik {request.FilePath} został odczytany.");

            return await Task.FromResult(results);
        }

        private List<List<string>> ListFromRange(ExcelWorksheet worksheet, int rowStart, int colStart, int rowEnd, int colEnd, bool transpose = false)
        {
            var range = new List<List<string>>();
            if (transpose)
            {
                for (var colN = colStart; colN <= colEnd; ++colN)
                {
                    var col = new List<string>();
                    for (var rowN = rowStart; rowN <= rowEnd; ++rowN)
                    {
                        var cell = worksheet.Cells[rowN, colN];
                        col.Add(cell.Value == null ? string.Empty : cell.Value.ToString());
                    }
                    range.Add(col);
                }
            }
            else
            {
                for (var rowN = rowStart; rowN <= rowEnd; ++rowN)
                {
                    var row = new List<string>();
                    for (var colN = colStart; colN <= colEnd; ++colN)
                    {
                        var cell = worksheet.Cells[rowN, colN];
                        row.Add(cell.Value == null ? string.Empty : cell.Value.ToString());
                    }
                    range.Add(row);
                }
            }

            return range;
        }
    }
}
