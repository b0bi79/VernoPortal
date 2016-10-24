using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;

using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.Util;

using Verno;

namespace ImportFromSql
{
    public class XlsOutputFormatter : OutputFormatter
    {
        private readonly Stream _stream;
        private readonly IWorkbook _workbook;
        private ISheet _sheet;
        private readonly ICellStyle _dataCellStyle;
        private readonly ICellStyle _headerCellStyle;
        private readonly ICellStyle _dateCellStyle;
        private readonly ICellStyle[] _groupCellStyles;
        private List<AggregationFunc> _aggregation;
        private List<ICellStyle> _columnStyles;

        public XlsOutputFormatter(Stream stream)
        {
            _stream = stream;
            _workbook = new HSSFWorkbook();

            // палитра цветов. 40 используется для заголовка, остальные для групп
            var pallete = ((HSSFWorkbook)_workbook).GetCustomPalette();
            pallete.SetColorAtIndex(40, 73, 69, 41);
            /*pallete.SetColorAtIndex(41, 0x31, 0x86, 0x9b);
            pallete.SetColorAtIndex(42, 0x4b, 0xac, 0xc6);
            pallete.SetColorAtIndex(43, 0x92, 0xcd, 0xdc);
            pallete.SetColorAtIndex(44, 0xb7, 0xde, 0xe8);
            pallete.SetColorAtIndex(45, 0xd7, 0xee, 0xf3);*/
            pallete.SetColorAtIndex(41, 0xD2, 0xB4, 0x8C);
            pallete.SetColorAtIndex(42, 0xF5, 0xDE, 0xB3);
            pallete.SetColorAtIndex(43, 0xFF, 0xDE, 0xAD);
            pallete.SetColorAtIndex(44, 0xFF, 0xE4, 0xB5);
            pallete.SetColorAtIndex(45, 0xFF, 0xDA, 0xB9);
            pallete.SetColorAtIndex(46, 0xFF, 0xE4, 0xC4);
            pallete.SetColorAtIndex(47, 0xFF, 0xEB, 0xCD);
            pallete.SetColorAtIndex(48, 0xFF, 0xEE, 0xD5);
            pallete.SetColorAtIndex(49, 0xFA, 0xEB, 0xD7);
            pallete.SetColorAtIndex(50, 0xFF, 0xF8, 0xDC);
            pallete.SetColorAtIndex(51, 0xF5, 0xF5, 0xDC);
            pallete.SetColorAtIndex(52, 0xFF, 0xFF, 0xF0);

            IFont boldFont = _workbook.CreateFont();
            boldFont.Boldweight = (short)FontBoldWeight.Bold;
            boldFont.Color = HSSFColor.White.Index;

            _dataCellStyle = NpoiHelper.CreateCellStyle(_workbook, HSSFColor.Black.Index, BorderStyle.Thin);
            _dateCellStyle = NpoiHelper.CreateCellStyle(_workbook, HSSFColor.Black.Index, BorderStyle.Thin);
            _dateCellStyle.DataFormat = _workbook.CreateDataFormat().GetFormat("dd.mm.yyyy hh:mm");
            _dateCellStyle.Alignment = HorizontalAlignment.Center;

            _headerCellStyle = NpoiHelper.CreateCellStyle(_workbook, HSSFColor.Black.Index, BorderStyle.Medium);
            _headerCellStyle.SetFont(boldFont);
            _headerCellStyle.Alignment = HorizontalAlignment.Center;
            _headerCellStyle.FillBackgroundColor = 40;//pallete.GetColor().GetIndex();
            _headerCellStyle.FillForegroundColor = 40;
            _headerCellStyle.FillPattern = FillPattern.SolidForeground;

            _groupCellStyles = new ICellStyle[13];
            for (short i = 0; i < _groupCellStyles.Length; i++)
            {
                _groupCellStyles[i] = NpoiHelper.CreateCellStyle(_workbook, HSSFColor.Black.Index, BorderStyle.Thin);
                _groupCellStyles[i].FillBackgroundColor = (short)(41 + i); // если надо больше цветов для групп, то можно считать с 41 или добавить ещё цвета
                _groupCellStyles[i].FillForegroundColor = (short)(41 + i);
                _groupCellStyles[i].FillPattern = FillPattern.SolidForeground;
                _groupCellStyles[i].VerticalAlignment = VerticalAlignment.Top - 1; // Top-1 NPOI Bug Fix
            }
        }

        public override void Write(object[] datas)
        {
            var row = _sheet.CreateRow(_sheet.LastRowNum + 1);
            for (int i = 0; i < datas.Length; i++)
            {
                var cell = row.CreateCell(i);
                NpoiHelper.SetCellValue(cell, datas[i]);
                cell.CellStyle = _columnStyles[i];
            }
        }

        public override void Close()
        {
            CloseSheet();
            _workbook.Write(_stream);
        }

        private void CloseSheet()
        {
            GroupRows();
            NpoiHelper.FitWidth(_sheet);
        }

        private void GroupRows()
        {
            int colIdx = GetNextGroup(0);
            if (colIdx >= 0)
                new Group(colIdx, (HSSFSheet)_sheet, _aggregation, new GroupStyles(_columnStyles, _groupCellStyles, _sheet.Workbook))
                    .CreateChildGroups(null, new CellRangeAddress(3, _sheet.LastRowNum + 1, 0, 0));
        }

        private int GetNextGroup(int start)
        {
            for (int i = start; i < _aggregation.Count; i++)
            {
                if (_aggregation[i] != null && _aggregation[i].Group)
                    return i;
            }
            return -1;
        }

        public override void NewTable(string name, IDataReader reader)
        {
            if (_sheet != null)
                CloseSheet();

            // Создание листа
            if (string.IsNullOrEmpty(name)) name = "Sheet" + _workbook.NumberOfSheets;
            name = name.Replace(":", "_");
            _sheet = _workbook.CreateSheet(name);
            _sheet.DefaultRowHeightInPoints = 15;

            // Вывод названия таблицы
            IRow hRow = _sheet.CreateRow(1);
            hRow.HeightInPoints = 35;
            var hcell = hRow.CreateCell(0);
            hcell.SetCellValue(name);
            var hStyle = _workbook.CreateCellStyle();
            var hFont = _workbook.CreateFont();
            hFont.Boldweight = (short)FontBoldWeight.Bold;
            hFont.IsItalic = true;
            hFont.FontHeightInPoints = 15;
            hStyle.SetFont(hFont);
            hStyle.WrapText = true;
            hStyle.Alignment = HorizontalAlignment.Center;
            hStyle.VerticalAlignment = VerticalAlignment.Top;
            hcell.CellStyle = hStyle;
            _sheet.AddMergedRegion(new CellRangeAddress(hRow.RowNum, hRow.RowNum, 0, reader.FieldCount - 1));

            _aggregation = new List<AggregationFunc>(reader.FieldCount);
            _columnStyles = new List<ICellStyle>(reader.FieldCount);

            // Заполнение заголовков столбцов
            IRow row = _sheet.CreateRow(2);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var cell = row.CreateCell(i);
                var colName = reader.GetName(i).Trim();
                string format = ParseColumnFormat(ref colName);
                _aggregation.Add(AggregationFunc.Parse(ref colName));
                cell.SetCellValue(colName);
                cell.CellStyle = _headerCellStyle;

                var cellStyle = _workbook.CreateCellStyle();
                Type fType = reader.GetFieldType(i);
                cellStyle.CloneStyleFrom(fType == typeof(DateTime) ? _dateCellStyle : _dataCellStyle);
                if (fType.IsNumeric())
                    cellStyle.Alignment = HorizontalAlignment.Right;
                if (!string.IsNullOrEmpty(format))
                {
                    var customFormat = _workbook.CreateDataFormat();
                    cellStyle.DataFormat = customFormat.GetFormat(format);
                }
                _columnStyles.Add(cellStyle);
            }
        }

        private string ParseColumnFormat(ref string colName)
        {
            int colonIdx = colName.IndexOf(':');
            if (colonIdx < 0)
                return "";
            string format = colName.Substring(colonIdx + 1);
            colName = colName.Substring(0, colonIdx);
            return format;
        }
    }

    public static class NpoiHelper
    {
        public static ICellStyle CreateCellStyle(IWorkbook workbook, short borderColor, BorderStyle all)
        {
            return CreateCellStyle(workbook, borderColor, all, all, all, all);
        }

        public static void SetCellValue(ICell cell, object value)
        {
            if (value is DateTime)
                cell.SetCellValue((DateTime)value);
            else if (value is bool)
                cell.SetCellValue((bool)value);
            else if (value is int || value is short || value is long || value is decimal || value is float || value is double || value is byte)
                cell.SetCellValue(Convert.ToDouble(value));
            else
                cell.SetCellValue(value.ToString());
        }

        public static ICellStyle CreateCellStyle(IWorkbook workbook, short borderColor, BorderStyle left, BorderStyle top, BorderStyle right, BorderStyle bottom)
        {
            ICellStyle style = workbook.CreateCellStyle();
            style.BorderLeft = left;
            style.LeftBorderColor = borderColor;
            style.BorderTop = top;
            style.TopBorderColor = borderColor;
            style.BorderRight = right;
            style.RightBorderColor = borderColor;
            style.BorderBottom = bottom;
            style.BottomBorderColor = borderColor;
            style.Alignment = HorizontalAlignment.Left;
            style.VerticalAlignment = VerticalAlignment.Center;
            return style;
        }

        public static void FitWidth(ISheet sheet)
        {
            var topRow = sheet.GetRow(sheet.FirstRowNum + 1);
            for (int i = topRow.FirstCellNum; i <= topRow.LastCellNum; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        public static IRow InsertRow(HSSFSheet sheet, int fromRowIndex)
        {
            sheet.ShiftRows(fromRowIndex, sheet.LastRowNum, 1, true, false, true);

            IRow rowSource = sheet.GetRow(fromRowIndex + 1);
            IRow rowInsert = sheet.CreateRow(fromRowIndex);
            rowInsert.Height = rowSource.Height;
            for (int colIndex = 0; colIndex < rowSource.LastCellNum; colIndex++)
            {
                ICell cellSource = rowSource.GetCell(colIndex);
                ICell cellInsert = rowInsert.CreateCell(colIndex);
                if (cellSource != null)
                {
                    cellInsert.CellStyle = cellSource.CellStyle;
                }
            }
            return rowInsert;
        }

        public static object GetCellValue(int column, IRow row)
        {
            if (row == null) return null;

            switch (row.Cells[column].CellType)
            {
                case CellType.Boolean:
                    return row.Cells[column].BooleanCellValue;
                case CellType.Numeric:
                    return row.Cells[column].NumericCellValue;
                default:
                    return row.Cells[column].StringCellValue;
            }
        }
    }

    public class Group
    {
        private readonly HSSFSheet _sheet;
        private readonly List<AggregationFunc> _aggregation;
        private readonly List<Group> _childGroups = new List<Group>();
        private readonly GroupStyles _groupStyles;
        private List<AggregationFunc.AggregationMemoryState> _totals;

        public Group(int column, HSSFSheet sheet, List<AggregationFunc> aggregation, GroupStyles groupStyles)
        {
            Column = column;
            _sheet = sheet;
            _aggregation = aggregation;
            _groupStyles = groupStyles;
        }

        public List<Group> ChildGroups { get { return _childGroups; } }
        public List<AggregationFunc.AggregationMemoryState> Totals { get { return _totals; } }
        public int RowStart { get; private set; }
        public int RowEnd { get; private set; }
        public int Column { get; private set; }

        public Group[] CreateChildGroups(GroupKey parentGroup, CellRangeAddress range)
        {
            int rowIdx = range.FirstRow;
            RowStart = rowIdx;
            object prev = null;

            while (rowIdx <= range.LastRow)
            {
                var row = _sheet.GetRow(rowIdx);
                var value = NpoiHelper.GetCellValue(Column, row);
                if (row != null && _aggregation[Column] != null && _aggregation[Column].Group)
                    row.Cells[Column].CellStyle = _groupStyles.GetBaseStyle(Column);
                // если группа закончилась, добавим строку выше группы и сгруппируем ячейки
                if (!Equals(prev, value) || (parentGroup != null && !parentGroup.IsMatch(row)))
                {
                    if (rowIdx - RowStart > 0)
                    {
                        // добавление строки итогов над группой
                        var totalsRow = NpoiHelper.InsertRow(_sheet, RowStart);
                        range.LastRow++;
                        rowIdx++;
                        // поиск подгрупп
                        int colIdx = GetNextGroup(Column + 1);
                        if (colIdx >= 0)
                        {
                            var group = new Group(colIdx, _sheet, _aggregation, _groupStyles);
                            var subrange = new CellRangeAddress(RowStart + 1, rowIdx, 0, 0);
                            group.CreateChildGroups(new GroupKey(0, Column, _aggregation, _sheet.GetRow(RowStart + 1)), subrange);
                            _childGroups.Add(group);
                            var rowInc = subrange.LastRow - rowIdx;
                            range.LastRow += rowInc;
                            rowIdx += rowInc;
                        }
                        // заполнение строки итогов
                        var groupCell = totalsRow.Cells[Column];
                        groupCell.SetCellValue((prev ?? "").ToString());
                        var styles = _groupStyles.GetTotalRowStyles(Column);
                        for (int i = Column + 1; i < totalsRow.Cells.Count; i++)
                        {
                            groupCell = totalsRow.Cells[i];
                            groupCell.CellStyle = styles[i - Column - 1];
                            if (_totals != null && _totals[i] != null)
                                NpoiHelper.SetCellValue(groupCell, _totals[i].GetResult());
                            /*groupCell.CellStyle = _sheet.Workbook.CreateCellStyle();
                            groupCell.CellStyle.CloneStyleFrom(_groupCellStyles[Column]);
                            if (_totals[i] != null)
                            {
                                NpoiHelper.SetCellValue(groupCell, _totals[i].GetResult());
                                groupCell.CellStyle.DataFormat = _columnStyles[i].DataFormat;
                                groupCell.CellStyle.Alignment = _columnStyles[i].Alignment;
                            }*/
                        }
                        // группировка строк
                        _sheet.GroupRow(RowStart + 1, rowIdx - 1);
                        _sheet.SetRowGroupCollapsed(rowIdx - 1, true);
                        // объединение ячеек
                        _sheet.AddMergedRegion(new CellRangeAddress(RowStart, rowIdx - 1, Column, Column));
                    }
                    _totals = null;
                    RowStart = rowIdx;
                    prev = value;
                }
                row = _sheet.GetRow(rowIdx);
                if (row != null) TotalsAdd(row);
                rowIdx++;
            }
            RowEnd = rowIdx - 1;
            return _childGroups.ToArray();
        }

        private void TotalsAdd(IRow row)
        {
            if (_totals == null)
            {
                _totals = new List<AggregationFunc.AggregationMemoryState>();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (_aggregation[i] != null)
                        _totals.Add(_aggregation[i].CreateMemoryState());
                    else
                        _totals.Add(null);
                }
            }
            for (int i = 0; i < row.Cells.Count; i++)
            {
                if (_totals[i] != null)
                {
                    _totals[i].Add(NpoiHelper.GetCellValue(i, row));
                }
            }
        }

        private int GetNextGroup(int start)
        {
            for (int i = start; i < _aggregation.Count; i++)
            {
                if (_aggregation[i] != null && _aggregation[i].Group)
                    return i;
            }
            return -1;
        }
    }

    public class GroupStyles
    {
        private readonly Dictionary<int, ICellStyle[]> _totalRowStyles;
        private readonly List<ICellStyle> _columnStyles;
        private readonly ICellStyle[] _groupCellStyles;
        private readonly IWorkbook _workbook;

        public GroupStyles(List<ICellStyle> columnStyles, ICellStyle[] groupCellStyles, IWorkbook workbook)
        {
            _columnStyles = columnStyles;
            _groupCellStyles = groupCellStyles;
            _workbook = workbook;
            _totalRowStyles = new Dictionary<int, ICellStyle[]>();
        }

        public ICellStyle GetBaseStyle(int column)
        {
            return _groupCellStyles[column];
        }

        public ICellStyle[] GetTotalRowStyles(int column)
        {
            ICellStyle[] styles;
            if (!_totalRowStyles.TryGetValue(column, out styles))
            {
                styles = CreateStyles(column);
                _totalRowStyles[column] = styles;
            }
            return styles;
        }

        private ICellStyle[] CreateStyles(int column)
        {
            var styles = new ICellStyle[_columnStyles.Count - column - 1];
            for (int i = column + 1; i < _columnStyles.Count; i++)
            {
                var style = _workbook.CreateCellStyle();
                style.CloneStyleFrom(GetBaseStyle(column));
                style.DataFormat = _columnStyles[i].DataFormat;
                style.Alignment = _columnStyles[i].Alignment;
                styles[i - column - 1] = style;
            }
            return styles;
        }
    }

    public class GroupKey
    {
        public int[] ColumnIndexes { get; set; }
        public object[] ColumnValues { get; set; }

        public GroupKey(int clmnStart, int clmnEnd, List<AggregationFunc> groups, IRow row)
        {
            var idxs = new List<int>();
            var vals = new ArrayList();
            for (int i = clmnStart; i <= clmnEnd; i++)
            {
                if (groups[i] != null && groups[i].Group)
                {
                    idxs.Add(i);
                    vals.Add(NpoiHelper.GetCellValue(i, row));
                }
            }
            ColumnIndexes = idxs.ToArray();
            ColumnValues = vals.ToArray();
        }

        public GroupKey(int[] columnIndexes, object[] columnValues)
        {
            if (columnIndexes.Length != columnValues.Length)
                throw new ArgumentException("columnIndexes.Length != columnValues.Length");
            ColumnIndexes = columnIndexes;
            ColumnValues = columnValues;
        }

        public bool IsMatch(IRow row)
        {
            for (int i = 0; i < ColumnIndexes.Length; i++)
            {
                var testVal = NpoiHelper.GetCellValue(ColumnIndexes[i], row);
                if (!Equals(ColumnValues[i], testVal))
                    return false;
            }
            return true;
        }
    }

    public abstract class AggregationFunc
    {
        public static AggregationFunc Parse(ref string colName)
        {
            bool group = true;
            int lt = colName.IndexOf('<');
            int gt = colName.IndexOf('>');

            if (lt < 0 || gt < 0)
            {
                lt = colName.IndexOf('{');
                gt = colName.IndexOf('}');
                group = false;
            }

            if (lt < 0 || gt < 0)
                return null;
            else if (lt == 0)
            {
                colName = colName.Substring(lt + 1, gt - lt - 1);
                return new GroupAggregationFunc();
            }
            else
            {
                var funcStr = colName.Substring(0, lt).ToLower();
                colName = colName.Substring(lt + 1, gt - lt - 1);
                AggregationFunc result;
                switch (funcStr)
                {
                    case "sum": result = new SumAggregationFunc(); break;
                    case "avg": result = new AvgAggregationFunc(); break;
                    case "min": result = new MinAggregationFunc(); break;
                    case "max": result = new MaxAggregationFunc(); break;
                    case "count": result = new CountAggregationFunc(); break;
                    case "countdistinct": result = new CountDistinctAggregationFunc(); break;
                    default:
                        throw new NotSupportedException("Not supported aggregation function " + funcStr.ToLower());
                }
                result.Group = group;
                return result;
            }
        }

        public bool Group { get; set; }
        public abstract AggregationMemoryState CreateMemoryState();

        public abstract class AggregationMemoryState
        {
            public abstract void Add(object value);
            public abstract object GetResult();
        }
    }

    class GroupAggregationFunc : AggregationFunc
    {
        public GroupAggregationFunc()
        {
            Group = true;
        }

        public override AggregationMemoryState CreateMemoryState()
        {
            return null;
        }
    }

    class SumAggregationFunc : AggregationFunc
    {
        public override AggregationMemoryState CreateMemoryState()
        {
            return new MemoryState();
        }

        class MemoryState : AggregationMemoryState
        {
            private double _sum = 0;
            public override void Add(object value)
            {
                try
                {
                    _sum += Convert.ToDouble(value);
                }catch(FormatException)
                {
                }
            }

            public override object GetResult()
            {
                return _sum;
            }
        }
    }

    class AvgAggregationFunc : AggregationFunc
    {
        public override AggregationMemoryState CreateMemoryState()
        {
            return new MemoryState();
        }

        class MemoryState : AggregationMemoryState
        {
            private double _sum = 0;
            private int _cnt = 0;

            public override void Add(object value)
            {
                try
                {
                    _sum += Convert.ToDouble(value);
                    _cnt++;
                }catch(FormatException)
                { }
            }

            public override object GetResult()
            {
                return _cnt > 0 ? _sum / _cnt : 0;
            }
        }
    }

    class MinAggregationFunc : AggregationFunc
    {
        public override AggregationMemoryState CreateMemoryState()
        {
            return new MemoryState();
        }

        class MemoryState : AggregationMemoryState
        {
            private object _min;
            private double _minInt;
            public override void Add(object value)
            {
                try
                {
                    var v = Convert.ToDouble(value);
                    _min = Math.Min(_minInt, v);
                    _minInt = v;
                }
                catch (FormatException)
                {
                    string v = value.ToString();
                    if (_minInt >= v.Length)
                    {
                        _min = value;
                        _minInt = v.Length;
                    }
                }
            }

            public override object GetResult()
            {
                return _min;
            }
        }
    }

    class MaxAggregationFunc : AggregationFunc
    {
        public override AggregationMemoryState CreateMemoryState()
        {
            return new MemoryState();
        }

        class MemoryState : AggregationMemoryState
        {
            private object _max = int.MinValue;
            private double _maxInt;
            public override void Add(object value)
            {
                try
                {
                    var v = Convert.ToDouble(value);
                    _max = Math.Max(_maxInt, v);
                    _maxInt = v;
                }
                catch (FormatException)
                {
                    string v = value.ToString();
                    if (_maxInt <= v.Length)
                    {
                        _max = value;
                        _maxInt = v.Length;
                    }
                } 
            }

            public override object GetResult()
            {
                return _max;
            }
        }
    }

    class CountAggregationFunc : AggregationFunc
    {
        public override AggregationMemoryState CreateMemoryState()
        {
            return new MemoryState();
        }

        class MemoryState : AggregationMemoryState
        {
            private int _cnt = 0;
            public override void Add(object value)
            {
                _cnt++;
            }

            public override object GetResult()
            {
                return _cnt;
            }
        }
    }

    class CountDistinctAggregationFunc : AggregationFunc
    {
        public override AggregationMemoryState CreateMemoryState()
        {
            return new MemoryState();
        }

        class MemoryState : AggregationMemoryState
        {
            private readonly Hashtable _hash = new Hashtable();
            public override void Add(object value)
            {
                if (!_hash.ContainsKey(value))
                    _hash.Add(value, value);
            }

            public override object GetResult()
            {
                return _hash.Count;
            }
        }
    }
}