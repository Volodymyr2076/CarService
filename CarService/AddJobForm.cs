using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CarService
{
    public partial class AddJobForm : Form
    {
        public AddJobForm()
        {
            InitializeComponent();
        }
        public AddJobForm(string id,string Name,string Model,string Year,string Vin)
        {
            InitializeComponent();
            label1.Text = id;
            textBox1.Text = Name;
            textBox2.Text = Model;
            textBox3.Text = Year;
            textBox4.Text = Vin;
        }
        

        private void AddJobForm_Load(object sender, EventArgs e)
        {
            Form1.FillListBox(listBox1, @"
SELECT j.id, j.types_of_jobs
FROM car_service.jobs AS j
");
            Form1.FillListBox(listBox2, @"
SELECT w.id, w.name
FROM car_service.worker AS w
WHERE w.box_id IS NULL
");
            Form1.FillListBox(listBox3, @"
SELECT b.id, b.box_number
FROM car_service.box AS b
WHERE b.car_id IS NULL
");
        }

        private void buttonOfJob_Click(object sender, EventArgs e)
        {
            ComboBoxItem box = (ComboBoxItem)listBox3.SelectedItem;
            ComboBoxItem worker = (ComboBoxItem)listBox2.SelectedItem;
            ComboBoxItem job = (ComboBoxItem)listBox1.SelectedItem;
            string car_id = label1.Text.Trim();
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            try
            {
                Form1.ExecuteQuery(@"
UPDATE car_service.box SET car_id = " + car_id + " WHERE car_service.box.id = " + box.Value);
                Form1.ExecuteQuery(@"
UPDATE car_service.worker SET box_id = " + box.Value + " WHERE car_service.worker.id = "+ worker.Value);
                DataTable jobRecordIdTable = Form1.ExecuteQuery(@"
INSERT INTO car_service.job_records(job_id,car_id,worker_id,date_begin,box_id) 
VALUES (" + job.Value + ", " + car_id + " ," + worker.Value + ", '" + date + "', " + box.Value + ") RETURNING id", hasReturnedValue: true);
                DataTable priceTable = Form1.ExecuteQuery(@"
SELECT * FROM car_service.get_price(" + job.Value + ", '" + date + "')", hasReturnedValue: true);
                DataTable clientTable = Form1.ExecuteQuery(@"
SELECT cl.id FROM car_service.client AS cl JOIN car_service.car AS c ON cl.id = c.client_id WHERE c.id = " + car_id, hasReturnedValue: true);
                string jobRecordId = jobRecordIdTable.Rows[0]["id"].ToString();
                string price = priceTable.Rows[0]["price"].ToString();
                string clientId = clientTable.Rows[0]["id"].ToString();
                Form1.ExecuteQuery(@"
INSERT INTO car_service.payments(client_id, job_record_id, total_sum) 
VALUES (" + clientId + ", " + jobRecordId + ", " + price + ")");
                this.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка: не удалось добавить автомобиль в работу ", "Добавление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
