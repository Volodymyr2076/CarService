using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarService
{
    internal static class Helper
    {
        public static void FillDataGridView(DataGridView dataGridView, DataTable dt)
        {
            dataGridView.DataSource = dt;
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        public static void FillListControl(ListControl listControl, DataTable dt)
        {
            if (listControl is ListBox lb)
            {
                lb.Items.Clear();
                foreach (DataRow row in dt.Rows)
                    lb.Items.Add(new ComboBoxItem(row[1].ToString(), row[0].ToString()));
            }
            else if (listControl is ComboBox cb)
            {
                cb.Items.Clear();
                foreach (DataRow row in dt.Rows)
                    cb.Items.Add(new ComboBoxItem(row[1].ToString(), row[0].ToString()));
            }
        }
    }
}
