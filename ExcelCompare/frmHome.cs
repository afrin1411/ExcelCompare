using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace ExcelCompare
{
    public partial class frmHome : Form
    {
        public frmHome()
        {
            InitializeComponent();
        }

        private void frmHome_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "d:\\";
                openFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void btnCompare_Click(object sender, EventArgs e)
        {
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet1;
            Excel.Range rangeSheet1;

            string str;
            int rCnt;
            int cCnt;
            int rw = 0;
            int cl = 0;

            xlApp = new Excel.Application();
            xlWorkBook = xlApp.Workbooks.Open(txtPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
            xlWorkSheet1 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
            var xlWorkSheet2 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(2);
            //var xlWorkSheet3 = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(3);
            var diffList = new List<string>();

            try
            {

                rangeSheet1 = xlWorkSheet1.UsedRange;
                rw = rangeSheet1.Rows.Count;
                cl = rangeSheet1.Columns.Count;

                var rowdataSheet1 = new Dictionary<string, int>();
                var columnsToExclude = txtExcludedColumn.Text.Split(',');
               

                for (rCnt = 1; rCnt <= rw; rCnt++)
                {
                    str = "";
                   
                    for (cCnt = 1; cCnt <= cl; cCnt++)
                    {
                        var rangeSelected = (rangeSheet1.Cells[rCnt, cCnt] as Excel.Range);
                        var address = rangeSelected.Address;
                        if (!columnsToExclude.Contains(address.Substring(1, 1)))
                        {
                            str += (string)rangeSelected.Value2 + ",";
                        }
                    }
                    str = str.TrimEnd(',');
                    if (!string.IsNullOrEmpty(str))
                    {
                        //if (!rowdataSheet1.ContainsKey(str)) //TO CHECK DUPLICATE
                        {
                            rowdataSheet1.Add(str.ToLower(), rCnt);
                        }
                                              
                    }
                }

                //sheet2
                var rangeSheet2 = xlWorkSheet2.UsedRange;
                rw = rangeSheet2.Rows.Count;
                cl = rangeSheet2.Columns.Count;

                int k = 1;
                for (rCnt = 1; rCnt <= rw; rCnt++)
                {
                    str = "";

                    for (cCnt = 1; cCnt <= cl; cCnt++) 
                    {
                        var rangeSelected = (rangeSheet2.Cells[rCnt, cCnt] as Excel.Range);
                        var address = rangeSelected.Address;
                        if (!columnsToExclude.Contains(address.Substring(1, 1)))
                        {
                            str += (string)rangeSelected.Value2 + ",";
                        }
                    }
                    str = str.TrimEnd(',');
                    if (!string.IsNullOrEmpty(str))
                    {
                        if (!rowdataSheet1.ContainsKey(str.ToLower()))
                        {
                            diffList.Add(str);
                        }
                    }
                }
                File.WriteAllLines("d:\\diff_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".csv", diffList.ToArray());
                MessageBox.Show("Done");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);

            }
            finally
            {
                xlWorkBook.Close(true, null, null);
                xlApp.Quit();

                Marshal.ReleaseComObject(xlWorkSheet1);
                Marshal.ReleaseComObject(xlWorkBook);
                Marshal.ReleaseComObject(xlApp);

            }

        }

        
    }
}
