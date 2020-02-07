using CardCreator.Features.Cards.Model;
using CardCreator.Features.Logging;
using CardCreator.Settings;
using MediatR;
using Microsoft.Extensions.Options;
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
        public ILogger Logger { get; set; }
        public FileInfo File { get; set; }

        public ReadCardFileCommand(ILogger logger, FileInfo file)
        {
            Logger = logger;
            File = file;
        }
    }

    public class ReadCardFileHandler : IRequestHandler<ReadCardFileCommand, ReadCardFileResults>
    {
        public int ColumnLimit { get; set; }
        public int RowLimit { get; set; }

        public ReadCardFileHandler(IOptions<AppSettings> settings)
        {
            ColumnLimit = settings.Value.ColumnLimit;
            RowLimit = settings.Value.RowLimit;
        }

        public async Task<ReadCardFileResults> Handle(ReadCardFileCommand request, CancellationToken cancellationToken)
        {
            var results = new ReadCardFileResults();

            using var xlPackage = new ExcelPackage(request.File);

            var worksheet = xlPackage.Workbook.Worksheets.First();
            var totalColumns = ColumnLimit > 0 ? Math.Min(ColumnLimit, worksheet.Dimension.End.Column) : worksheet.Dimension.End.Column;
            var totalRows = RowLimit > 0 ? Math.Min(RowLimit, worksheet.Dimension.End.Row) : worksheet.Dimension.End.Row;

            request.Logger?.LogMessage($"{totalRows} rows and {totalColumns} columns to read...");

            results.CardSchemaParams = ListFromRange(worksheet, 1, 1, CardSchema.ParamsNumber, 1, true).First();
            results.ElementSchemasParams = ListFromRange(worksheet, 1, 2, ElementSchema.ParamsNumber, totalColumns, true);
            results.CardsElements = ListFromRange(worksheet, ElementSchema.ParamsNumber + 1, 2, totalRows, totalColumns);
            results.CardsRepetitions = ListFromRange(worksheet, ElementSchema.ParamsNumber + 1, 1, totalRows, 1, true).First().Select(x => int.TryParse(x, out var n) ? n : 1).ToList();

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
