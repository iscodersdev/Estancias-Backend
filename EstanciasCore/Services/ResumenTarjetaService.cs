using DAL.Models;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class GeneradorResumenPDF
{
    // El DTO no cambia
    public class DatosParaResumenDTO
    {
        public Usuario Usuario { get; set; }
        public Periodo Periodo { get; set; }
        public List<MovimientoTarjeta> Movimientos { get; set; }
        public decimal SaldoAnterior { get; set; }
        public decimal Pagos { get; set; }
        public decimal Intereses { get; set; }
        public decimal Impuestos { get; set; }
        public decimal SaldoActual { get; set; }
        public decimal PagoMinimo { get; set; }
    }

    public byte[] Generar(DatosParaResumenDTO datos)
    {
        if (datos?.Movimientos == null || !datos.Movimientos.Any()) return new byte[0];

        using (var memoryStream = new MemoryStream())
        {
            // La creación del documento en iText 7 es diferente
            var writer = new PdfWriter(memoryStream);
            var pdfDocument = new PdfDocument(writer);
            var document = new Document(pdfDocument, PageSize.A4);
            document.SetMargins(80, 30, 50, 30); // top, right, bottom, left

            // Asignamos el gestor de eventos para la cabecera y pie de página
            var eventHandler = new GestorDeEventosPDF(datos.Usuario);
            pdfDocument.AddEventHandler(PdfDocumentEvent.START_PAGE, eventHandler);
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, eventHandler);

            // --- CUERPO DEL DOCUMENTO ---
            document.Add(CrearTablaEstadoCuenta(datos.Periodo));
            document.Add(new Paragraph(" "));
            document.Add(CrearTablaSaldosPrincipales(datos));
            document.Add(new Paragraph(" "));
            document.Add(CrearTablaResumenConsolidado(datos));
            document.Add(new Paragraph(" "));
            document.Add(CrearTablaDetalleDelMes(datos.Movimientos, datos.SaldoAnterior));

            document.Close();
            return memoryStream.ToArray();
        }
    }

    // --- MÉTODOS AUXILIARES ADAPTADOS A ITEXT 7 ---

    private Table CrearTablaEstadoCuenta(Periodo p)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 })).UseAllAvailableWidth();
        table.AddCell(new Cell().Add(new Paragraph("Estado de cuenta al:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph($"{p.FechaHasta:dd-MMM-yy}")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("Vencimiento actual:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph($"{p.FechaVencimiento:dd-MMM-yy}")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("Próximo Cierre:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph($"{p.FechaHasta.AddMonths(1):dd-MMM-yy}")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("Próximo Vencimiento:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph($"{p.FechaVencimiento.AddMonths(1):dd-MMM-yy}")).SetBorder(Border.NO_BORDER));
        return table;
    }

    private Table CrearTablaSaldosPrincipales(DatosParaResumenDTO d)
    {
        var table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth();

        table.AddCell(new Cell().Add(new Paragraph("Saldo actual:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("Pago Mínimo:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("Compras Anteriores:")).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("Pago Mínimo Anterior:")).SetBorder(Border.NO_BORDER));

        table.AddCell(new Cell().Add(new Paragraph(d.SaldoActual.ToString("C")).SetBold()).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph(d.PagoMinimo.ToString("C")).SetBold()).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph(d.SaldoAnterior.ToString("C")).SetBold()).SetBorder(Border.NO_BORDER));
        table.AddCell(new Cell().Add(new Paragraph("0.00").SetBold()).SetBorder(Border.NO_BORDER));

        return table;
    }

    private Table CrearTablaResumenConsolidado(DatosParaResumenDTO d)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 75, 25 })).UseAllAvailableWidth().SetMarginTop(10);

        var cellTitle = new Cell(1, 2).Add(new Paragraph("RESUMEN CONSOLIDADO").SetBold())
            .SetBorderBottom(new SolidBorder(ColorConstants.GRAY, 1)).SetPaddingBottom(5);
        table.AddHeaderCell(cellTitle);

        Action<string, decimal> AddRow = (concepto, monto) => {
            table.AddCell(new Cell().Add(new Paragraph(concepto)).SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph(monto.ToString("N2"))).SetTextAlignment(TextAlignment.RIGHT).SetBorder(Border.NO_BORDER));
        };

        AddRow("SALDO ANTERIOR", d.SaldoAnterior);
        if (d.Pagos != 0) AddRow("SU PAGO", d.Pagos);
        AddRow("TOTAL CONSUMOS DEL MES", d.Movimientos.Sum(m => m.Monto));
        AddRow("INTERESES", d.Intereses);
        AddRow("IMPUESTOS", d.Impuestos);

        var cellTotalLabel = new Cell().Add(new Paragraph("SALDO ACTUAL").SetBold()).SetBorderTop(new SolidBorder(ColorConstants.GRAY, 1)).SetPaddingTop(5).SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER).SetBorderBottom(Border.NO_BORDER);
        var cellTotalMonto = new Cell().Add(new Paragraph(d.SaldoActual.ToString("N2")).SetBold()).SetTextAlignment(TextAlignment.RIGHT).SetBorderTop(new SolidBorder(ColorConstants.GRAY, 1)).SetPaddingTop(5).SetBorderLeft(Border.NO_BORDER).SetBorderRight(Border.NO_BORDER).SetBorderBottom(Border.NO_BORDER);
        table.AddCell(cellTotalLabel);
        table.AddCell(cellTotalMonto);

        return table;
    }

    private Table CrearTablaDetalleDelMes(List<MovimientoTarjeta> movimientos, decimal saldoAnterior)
    {
        var table = new Table(UnitValue.CreatePercentArray(new float[] { 20f, 20f, 40f, 15f, 25f })).UseAllAvailableWidth().SetMarginTop(10);

        var cellTitle = new Cell(1, 5).Add(new Paragraph("DETALLE DEL MES").SetBold()).SetBorderBottom(new SolidBorder(ColorConstants.GRAY, 1)).SetPaddingBottom(5);
        table.AddHeaderCell(cellTitle);

        var headers = new List<string> { "Solicitud", "Fecha", "Comercio", "Cuota", "Monto" };
        foreach (var header in headers)
        {
            table.AddHeaderCell(new Cell().Add(new Paragraph(header).SetBold()).SetTextAlignment(TextAlignment.CENTER));
        }

        if (saldoAnterior > 0)
        {
            table.AddCell(new Cell(1, 4).Add(new Paragraph("Saldo Período Anterior").SetBold()).SetPadding(5));
            table.AddCell(new Cell().Add(new Paragraph(saldoAnterior.ToString("N2")).SetBold()).SetTextAlignment(TextAlignment.RIGHT).SetPadding(5));
        }

        foreach (var mov in movimientos)
        {
            table.AddCell(new Cell().Add(new Paragraph(mov.NroSolicitud)));
            table.AddCell(new Cell().Add(new Paragraph(mov.Fecha.ToString("dd-MMM-yy"))));
            table.AddCell(new Cell().Add(new Paragraph(mov.NombreComercio)));
            table.AddCell(new Cell().Add(new Paragraph($"{mov.NroCuota}/{mov.CantidadCuotas}")).SetTextAlignment(TextAlignment.CENTER));
            table.AddCell(new Cell().Add(new Paragraph(mov.Monto.ToString("N2"))).SetTextAlignment(TextAlignment.RIGHT));
        }

        return table;
    }

}

public class GestorDeEventosPDF : IEventHandler
{
    private Usuario Usuario;

    public GestorDeEventosPDF(Usuario usuario)
    {
        this.Usuario = usuario;
    }

    public void HandleEvent(Event @event)
    {
        var docEvent = (PdfDocumentEvent)@event;
        var page = docEvent.GetPage();
        var pageSize = page.GetPageSize();
        var pdfDoc = docEvent.GetDocument();
        var canvas = new PdfCanvas(page);

        if (docEvent.GetEventType() == PdfDocumentEvent.START_PAGE)
        {
            // Lógica para dibujar la cabecera
            var table = new Table(UnitValue.CreatePercentArray(new float[] { 70, 30 })).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("Mi Banco S.A.").SetBold()).SetBorder(Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph($"Hoja: {pdfDoc.GetPageNumber(page)}").SetTextAlignment(TextAlignment.RIGHT)).SetBorder(Border.NO_BORDER));
            //... Añadir más datos a la cabecera como en el ejemplo anterior

            var canvasWrapper = new Canvas(canvas, pageSize);
            canvasWrapper.Add(table.SetMarginLeft(30).SetMarginTop(20)); // Ajustar márgenes
        }
        else if (docEvent.GetEventType() == PdfDocumentEvent.END_PAGE)
        {
            // Lógica para dibujar el pie de página
            var table = new Table(1).UseAllAvailableWidth();
            table.AddCell(new Cell().Add(new Paragraph("USTED DISPONE DE 30 DIAS PARA CUESTIONAR SU RESUMEN DE CUENTA DESDE SU RECEPCION.").SetFontSize(8).SetFontColor(ColorConstants.GRAY)).SetTextAlignment(TextAlignment.CENTER).SetBorder(Border.NO_BORDER));

            var canvasWrapper = new Canvas(canvas, pageSize);
            // Se dibuja en una posición fija en la parte inferior
            canvasWrapper.ShowTextAligned(table.ToString(), pageSize.GetLeft() + 30, pageSize.GetBottom() + 30, TextAlignment.LEFT);
        }
    }
}


