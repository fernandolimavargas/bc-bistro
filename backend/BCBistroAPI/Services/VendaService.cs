using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public class VendaService
{
    private readonly VendaRepository _vendaRepository;

    public VendaService(VendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }

    public async Task FinalizarVenda(Venda venda)
    {
        await _vendaRepository.FinalizarVenda(venda);
    }

    public async Task<List<VendasHistoricos>> BuscarVendas(DateTime dataInicial, DateTime dataFinal)
    {
        return await _vendaRepository.BuscarVendas(dataInicial, dataFinal);

    }

    public async Task<byte[]> DownloadExcelVendas(
        DateTime dataInicial,
        DateTime dataFinal)
    {
        var vendas = await BuscarVendas(dataInicial, dataFinal);

        var valorTotalVendas = vendas.Sum(s => s.TotalVenda);

        using var memoryStream = new MemoryStream();

        using (var document = SpreadsheetDocument.Create(
            memoryStream,
            SpreadsheetDocumentType.Workbook))
        {
            WorkbookPart workbookPart =
                document.AddWorkbookPart();

            workbookPart.Workbook = new Workbook();

            WorksheetPart worksheetPart =
                workbookPart.AddNewPart<WorksheetPart>();

            Sheets sheets =
                workbookPart.Workbook.AppendChild(new Sheets());

            Sheet sheet = new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Relatório Vendas"
            };

            sheets.Append(sheet);

            var sheetData = new SheetData();

            worksheetPart.Worksheet =
                new Worksheet(sheetData);

            #region STYLES

            WorkbookStylesPart stylesPart =
                workbookPart.AddNewPart<WorkbookStylesPart>();

            stylesPart.Stylesheet = CreateStylesheet();

            stylesPart.Stylesheet.Save();

            #endregion

            uint rowIndex = 1;

            #region TITULO

            Row tituloRow = new Row();

            tituloRow.Append(
                CreateTextCell(
                    $"A{rowIndex}",
                    "RELATÓRIO DE VENDAS",
                    1
                )
            );

            sheetData.Append(tituloRow);

            rowIndex++;

            #endregion

            #region PERIODO

            Row periodoRow = new Row();

            periodoRow.Append(
                CreateTextCell(
                    $"A{rowIndex}",
                    "PERÍODO:",
                    1
                ),

                CreateTextCell(
                    $"B{rowIndex}",
                    $"{dataInicial:dd/MM/yyyy} até {dataFinal:dd/MM/yyyy}"
                )
            );

            sheetData.Append(periodoRow);

            rowIndex++;

            #endregion

            #region TOTAL GERAL

            Row totalGeralRow = new Row();

            totalGeralRow.Append(
                CreateTextCell(
                    $"A{rowIndex}",
                    "VALOR TOTAL DE VENDAS:",
                    1
                ),

                CreateTextCell(
                    $"B{rowIndex}",
                    valorTotalVendas.ToString("C")
                )
            );

            sheetData.Append(totalGeralRow);

            rowIndex += 2;

            #endregion

            foreach (var venda in vendas)
            {
                #region HEADER VENDA

                Row vendaHeader = new Row();

                vendaHeader.Append(
                    CreateTextCell(
                        $"A{rowIndex}",
                        $"VENDA #{venda.Id}",
                        1
                    ),

                    CreateTextCell(
                        $"D{rowIndex}",
                        "VALOR TOTAL DA VENDA:",
                        1
                    ),

                    CreateTextCell(
                        $"E{rowIndex}",
                        venda.TotalVenda.ToString("C")
                    )
                );

                sheetData.Append(vendaHeader);

                rowIndex++;

                #endregion

                #region DATA VENDA

                Row dataVendaRow = new Row();

                dataVendaRow.Append(
                    CreateTextCell(
                        $"A{rowIndex}",
                        "DATA:",
                        1
                    ),

                    CreateTextCell(
                        $"B{rowIndex}",
                        venda.HoraVenda
                            .ToString("dd/MM/yyyy HH:mm")
                    )
                );

                sheetData.Append(dataVendaRow);

                rowIndex++;

                #endregion

                #region CABEÇALHO PRODUTOS

                Row produtosHeader = new Row();

                produtosHeader.Append(
                    CreateTextCell(
                        $"A{rowIndex}",
                        "PRODUTO",
                        1
                    ),

                    CreateTextCell(
                        $"B{rowIndex}",
                        "CATEGORIA",
                        1
                    ),

                    CreateTextCell(
                        $"C{rowIndex}",
                        "QUANTIDADE",
                        1
                    ),

                    CreateTextCell(
                        $"D{rowIndex}",
                        "VALOR UNITÁRIO",
                        1
                    ),

                    CreateTextCell(
                        $"E{rowIndex}",
                        "VALOR CALCULADO",
                        1
                    )
                );

                sheetData.Append(produtosHeader);

                rowIndex++;

                #endregion

                #region PRODUTOS

                foreach (var produto in venda.ProdutosVendidos)
                {
                    Row produtoRow = new Row();

                    produtoRow.Append(
                        CreateTextCell(
                            $"A{rowIndex}",
                            produto.Produto
                        ),

                        CreateTextCell(
                            $"B{rowIndex}",
                            produto.Categoria
                        ),

                        CreateNumberCell(
                            $"C{rowIndex}",
                            produto.Quantidade
                        ),

                        CreateDecimalCell(
                            $"D{rowIndex}",
                            produto.ValorUnidade
                        ),

                        CreateDecimalCell(
                            $"E{rowIndex}",
                            produto.ValorCalculado
                        )
                    );

                    sheetData.Append(produtoRow);

                    rowIndex++;
                }

                #endregion

                rowIndex++;
            }

            #region COLUNAS

            Columns columns = new Columns(
                new Column()
                {
                    Min = 1,
                    Max = 1,
                    Width = 35,
                    CustomWidth = true
                },

                new Column()
                {
                    Min = 2,
                    Max = 2,
                    Width = 25,
                    CustomWidth = true
                },

                new Column()
                {
                    Min = 3,
                    Max = 3,
                    Width = 15,
                    CustomWidth = true
                },

                new Column()
                {
                    Min = 4,
                    Max = 4,
                    Width = 20,
                    CustomWidth = true
                },

                new Column()
                {
                    Min = 5,
                    Max = 5,
                    Width = 20,
                    CustomWidth = true
                }
            );

            worksheetPart.Worksheet.InsertAt(columns, 0);

            #endregion

            workbookPart.Workbook.Save();
        }

        return memoryStream.ToArray();
    }

    private Stylesheet CreateStylesheet()
    {
        return new Stylesheet(
            new Fonts(
                new Font(),

                new Font(
                    new Bold(),
                    new FontSize()
                    {
                        Val = 12
                    }
                )
            ),

            new Fills(
                new Fill(
                    new PatternFill()
                    {
                        PatternType = PatternValues.None
                    }
                ),

                new Fill(
                    new PatternFill()
                    {
                        PatternType = PatternValues.Gray125
                    }
                )
            ),

            new Borders(
                new Border()
            ),

            new CellFormats(
                new CellFormat(),

                new CellFormat()
                {
                    FontId = 1,
                    ApplyFont = true
                }
            )
        );
    }

    private Cell CreateTextCell(
        string cellReference,
        string text,
        uint styleIndex = 0)
    {
        return new Cell()
        {
            CellReference = cellReference,
            DataType = CellValues.String,
            CellValue = new CellValue(text),
            StyleIndex = styleIndex
        };
    }

    private Cell CreateNumberCell(
        string cellReference,
        int number,
        uint styleIndex = 0)
    {
        return new Cell()
        {
            CellReference = cellReference,
            CellValue = new CellValue(number.ToString()),
            DataType = CellValues.Number,
            StyleIndex = styleIndex
        };
    }

    private Cell CreateDecimalCell(
        string cellReference,
        decimal value,
        uint styleIndex = 0)
    {
        return new Cell()
        {
            CellReference = cellReference,

            CellValue = new CellValue(
                value.ToString(
                    System.Globalization.CultureInfo.InvariantCulture
                )
            ),

            DataType = CellValues.Number,
            StyleIndex = styleIndex
        };
    }
}