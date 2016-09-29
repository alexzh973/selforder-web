using System;
using System.Diagnostics;
using ensoCom;


public enum ExcelTypeString{Just,TableRow,Bold,TableHeader,RegionHeader};
	public enum ExcelFormatValue{Int,Decimal,Percent,String,Code5,Code6,Code10,Code13};

	/// <summary>
	/// Summary description for ExportToExcel.
	/// </summary>
	public class ExportToExcel
	{
        //пример использования:
        /*
          private bool SaveToExcel( string path)
                {
                    string numf = "";
                    Cursor = Cursors.WaitCursor;
                    bool ret = false;
                    try 
                    { 
                        Excel.ApplicationClass excel = new Excel.ApplicationClass();
                        Excel.Workbook wb = (Excel.Workbook)excel.Workbooks.Add(System.Reflection.Missing.Value); 
                        Excel.Worksheet ws = (Excel.Worksheet)excel.ActiveSheet; 
                        Excel.Range oRange;

                        numf = ExportToExcel.GetNumberFormat(ws);

                        DataView dv = (DataView)dgList1.DataSource;
				
			
                        int rowI=2;
                        int colI = 1;
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"Менеджер","@",20,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"Счет №","@",8,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"группа","@",10,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"номер","@",15,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"наименование","@",30,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"выписано","@",12,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"отгружено","@",12,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"в упаковке","@",12,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"дельта","@",12,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"результат","@",12,ExcelTypeString.TableHeader);
                        ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],"недобор","@",12,ExcelTypeString.TableHeader);
                        rowI +=1;
                        foreach(DataRow r in docsdt.Select(dv.RowFilter,dv.Sort))
                        {	
                            colI = 1;				
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],r["ТА"].ToString(),"000000",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],r["Счет"].ToString(),"000000",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],r["группа"].ToString(),"00000",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],r["номер"].ToString(),"0000000000000",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],r["название"].ToString(),"@",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],cNum.cToDecimal(r["кол_в_док"]),numf,-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],cNum.cToDecimal(r["кол_отгр"]),numf,-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],cNum.cToDecimal(r["в_упаковке"]),numf,-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],cNum.cToDecimal(r["дельта"]),numf+"%",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],r["результат"].ToString(),"@",-1,ExcelTypeString.Just);
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++],cNum.cToDecimal(r["недобор"]),numf,-1,ExcelTypeString.Just);
                            rowI++;
                        }
			
					
                        wb.SaveAs(path, Excel.XlFileFormat.xlExcel7, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            Excel.XlSaveAsAccessMode.xlExclusive, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value); 
                        excel.Quit(); 
                        excel = null; 
                        MessageBox.Show("Записан файл "+path);
                        ret = true;
                    } 
                    catch (System.Runtime.InteropServices.COMException exc) 
                    { 
                        MessageBox.Show(exc.Message); 
                    } 
                    finally 
                    { 
                        GC.Collect();
                    } 
                    Cursor = Cursors.Default;
                    return ret;
                }
         
         */
        static string _numberformat;
		public static string GetNumberFormat(Excel.Worksheet ws)
		{
			if(_numberformat!=null)
				return _numberformat;
			
			try
			{
				((Excel.Range)ws.Cells[1, 1]).NumberFormatLocal = "0.00";
				return "0.00";
			}
			catch
			{
				return "0,00";
			}

		}
		private static string getNumberFormat(Excel.Range oRange)
		{
			if(_numberformat!=null)
				return _numberformat;
			
			try
			{
				oRange.NumberFormatLocal = "0.00";
				return "0.00";
			}
			catch
			{
				return "0,00";
			}

		}
		public static void setExcelCell(Excel.Range oRange,object Value, ExcelFormatValue format, int width, ExcelTypeString type, int FontSize)
		{
			setExcelCell(oRange, Value, format, width,  type);
			oRange.Font.Size = FontSize;	
		}
		public static void setExcelCell(Excel.Range oRange,object Value,ExcelFormatValue format, int width, ExcelTypeString type)
		{
			string frm="";
			switch(format)
			{
				case ExcelFormatValue.Decimal:
					frm = getNumberFormat(oRange);
					break;
				case ExcelFormatValue.Int:
					frm="0";
					break;
				case ExcelFormatValue.Percent:
					frm = getNumberFormat(oRange)+"%";
					break;
				case ExcelFormatValue.Code5:
					frm="00000";
					break;
				case ExcelFormatValue.Code6:
					frm="000000";
					break;
				case ExcelFormatValue.Code10:
					frm="0000000000";
					break;
				case ExcelFormatValue.Code13:
					frm="0000000000000";
					break;
				case ExcelFormatValue.String:
				default:
					frm = "@";
					break;
			}
			setExcelCell(oRange, Value, frm, width,  type);
		}
		public static void setExcelCell(Excel.Range oRange,object Value,string format, int width, ExcelTypeString type, int FontSize)
		{
			setExcelCell(oRange, Value, format, width,  type);
			oRange.Font.Size = FontSize;	
		}
		
		public static void setExcelCell(Excel.Range oRange,object Value,string format, int width, ExcelTypeString type)
		{
			if (width>=0) oRange.ColumnWidth = width;
			oRange.FormulaLocal = Value; 
			oRange.NumberFormatLocal = format;

			switch (type)
			{
				case ExcelTypeString.RegionHeader:
					oRange.Font.Size = 12;	
					oRange.WrapText = false; 
					oRange.Font.ColorIndex = 1;
					//oRange.Interior.ColorIndex = 48;
					oRange.Font.Bold = true;
					break;
				case ExcelTypeString.Bold:
					oRange.Font.Bold = true;
					break;
				case ExcelTypeString.TableHeader:
					oRange.Borders.ColorIndex = 1;
					oRange.Font.Size = 8;	
					oRange.WrapText = true; 
					//oRange.HorizontalAlignment = ;
					oRange.Font.ColorIndex = 2;
					oRange.Interior.ColorIndex = 48;
					oRange.Font.Bold = true;
					break;
				case ExcelTypeString.TableRow:
					oRange.Borders.ColorIndex = 1;
					break;
				case ExcelTypeString.Just:
				default:
					break;
			}
		}

        public static bool ExportJast(System.Data.DataTable sourse, string filepath)
        {
            bool res = false;
           // sourse.WriteXml();
                    string numf = "";
                    
                    
                    try 
                    { 
                        Excel.ApplicationClass excel = new Excel.ApplicationClass();
                        Excel.Workbook wb = (Excel.Workbook)excel.Workbooks.Add(System.Reflection.Missing.Value); 
                        Excel.Worksheet ws = (Excel.Worksheet)excel.ActiveSheet; 
                        Excel.Range oRange;

                        numf = ExportToExcel.GetNumberFormat(ws);
			
                        int rowI=2;
                        int colI = 1;
                        foreach (System.Data.DataColumn th in sourse.Columns)
                            ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, colI++], th.ColumnName, "@", 20, ExcelTypeString.TableHeader);
                        rowI +=1;
                        foreach (System.Data.DataRow r in sourse.Rows)
                        {	
                            colI = 1;
                            for (int i = 0; i < sourse.Columns.Count; i++)
                            {
                                if (sourse.Columns[i].DataType == typeof(int))
                                    ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, i + 1], cNum.cToInt(r[i]), "000000", -1, ExcelTypeString.Just);
                                else if (sourse.Columns[i].DataType == typeof(decimal))
                                    ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, i + 1], cNum.cToDecimal(r[i]), numf, -1, ExcelTypeString.Just);
                                else if (sourse.Columns[i].DataType == typeof(DateTime))
                                    ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, i + 1], cDate.cToDate(r[i]), "ДД.ММ.ГГГГ", -1, ExcelTypeString.Just);
                                else
                                    ExportToExcel.setExcelCell((Excel.Range)ws.Cells[rowI, i+1], ""+r[i], "000000", -1, ExcelTypeString.Just);
                            }
                            rowI++;
                        }
                        wb.SaveAs(filepath.Replace(@"\", "/").Replace("//", "/").Replace("/", @"\"), 
                            Excel.XlFileFormat.xlExcel7, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            Excel.XlSaveAsAccessMode.xlExclusive, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value, 
                            System.Reflection.Missing.Value); 
                        excel.Quit(); 
                        excel = null; 
                        res = true;
                    } 
                    catch (System.Runtime.InteropServices.COMException exc) 
                    { 
                    } 
                    finally 
                    { 
                        GC.Collect();
                    } 
                    return res;
                
            return res;
        }
	}

