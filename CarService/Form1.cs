using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarService
{
    public partial class Form1 : Form
    {
        public static string ConnectionString { get; set; } = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=25542554";

        public Form1()
        {
            InitializeComponent();
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void изменитьСтрокуПодключенияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new FormConnectionString().ShowDialog();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadWorkers();
            LoadBox();
            LoadJobs();
            LoadPrice();
            LoadCar();
            LoadWork();
            LoadPayment();
            LoadReportsList();
        }

        public static void FillGridView(DataGridView dataGridView, string query)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=25542554");
            conn.Open();
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            npgsqlCommand.Connection = conn;
            npgsqlCommand.CommandType = CommandType.Text;
            npgsqlCommand.CommandText = query;
            NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
            //if (reader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridView.DataSource = dt;
            }
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            conn.Dispose();
            conn.Close();
        }
        public static void FillListBox(ListControl listControl, string query)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=25542554");
            conn.Open();
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            npgsqlCommand.Connection = conn;
            npgsqlCommand.CommandType = CommandType.Text;
            npgsqlCommand.CommandText = query;
            NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                foreach (DataRow row in dt.Rows)
                {
                    if (listControl is ListBox lb)
                        lb.Items.Add(new ComboBoxItem(row[1].ToString(), row[0].ToString()));
                    else if (listControl is ComboBox cb)
                        cb.Items.Add(new ComboBoxItem(row[1].ToString(), row[0].ToString()));
                }
            }

            conn.Dispose();
            conn.Close();
        }
        public static DataTable ExecuteQuery(string query, bool hasReturnedValue = false)
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=25542554");
            conn.Open();
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            npgsqlCommand.Connection = conn;
            npgsqlCommand.CommandType = CommandType.Text;
            npgsqlCommand.CommandText = query;
            DataTable result = null;
            if (hasReturnedValue)
            {
                NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    result = new DataTable();
                    result.Load(reader);
                }
            }
            else
            {
                npgsqlCommand.ExecuteNonQuery();
            }
            conn.Dispose();
            conn.Close();
            return result;
        }

        private void LoadWorkers()
        {
            FillGridView(dataGridViewWorkers, @"
SELECT w.id AS ID, w.name AS Имя, b.box_number AS Бокс
FROM car_service.worker AS w
LEFT JOIN car_service.box AS b ON b.id = w.box_id
");
            Form1.FillListBox(comboBox2, @"
SELECT w.id, w.name
FROM car_service.worker AS w
");
        }
        private void LoadBox()
        {
            FillGridView(dataGridViewBox, @"
SELECT b.id AS ID, b.box_number AS Номер_Бокса, b.car_id AS Автомобиль
FROM car_service.box AS b
");
            for (int i = 0; i < dataGridViewBox.RowCount - 1; i++)
            {
                comboBoxWorkersBox1.Items.Add(
                    new ComboBoxItem(
                        dataGridViewBox.Rows[i].Cells[1].Value.ToString(),
                        dataGridViewBox.Rows[i].Cells[0].Value.ToString()
                        )
                    );
            }
        }

        private void LoadJobs()
        {
            FillGridView(dataGridViewJobs, @"
SELECT j.id AS ID, j.types_of_jobs AS Наименование_Работ
FROM car_service.jobs AS j
");
            for (int i = 0; i < dataGridViewJobs.RowCount - 1; i++)
            {
                comboBoxPriceTypeOfJobs.Items.Add(
                    new ComboBoxItem(
                        dataGridViewJobs.Rows[i].Cells[1].Value.ToString(),
                        dataGridViewJobs.Rows[i].Cells[0].Value.ToString()
                        )
                    );
            }
        }

        private void LoadPrice()
        {
            FillGridView(dataGridViewPrice, @"
SELECT p.id AS ID, j.types_of_jobs  AS Наименование_Работ, p.price as Цена, p.date as Дата
FROM car_service.price AS p
join car_service.jobs as j on j.id  = p.jobs_id
");
        }

        private void buttonDelWorkers_Click(object sender, EventArgs e)
        {
            List<string> ids = new List<string>();
            foreach (var row in dataGridViewWorkers.SelectedRows)
            {
                DataGridViewRow row2 = row as DataGridViewRow;
                ids.Add(row2.Cells[0].Value.ToString());
            }
            var result = MessageBox.Show("Ви уверены что хотите удалить\nзаписи со следующими номерами:\n" + String.Join(", ", ids), "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ExecuteQuery(@"
DELETE FROM car_service.worker AS w
WHERE w.id IN (" + String.Join(", ", ids) + ")");
                    LoadWorkers();
                }
                catch
                {
                    MessageBox.Show("Ошибка", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void buttonDelBox_Click(object sender, EventArgs e)
        {
            List<string> ids = new List<string>();
            foreach (var row in dataGridViewBox.SelectedRows)
            {
                DataGridViewRow row2 = row as DataGridViewRow;
                ids.Add(row2.Cells[0].Value.ToString());
            }
            var result = MessageBox.Show("Ви уверены что хотите удалить\nзаписи со следующими номерами:\n" + String.Join(", ", ids), "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ExecuteQuery(@"
DELETE FROM car_service.box AS b
WHERE b.id IN (" + String.Join(", ", ids) + ")");
                    LoadBox(); ;
                }
                catch
                {
                    MessageBox.Show("Ошибка", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void buttonDelJob_Click(object sender, EventArgs e)
        {
            List<string> ids = new List<string>();
            foreach (var row in dataGridViewJobs.SelectedRows)
            {
                DataGridViewRow row2 = row as DataGridViewRow;
                ids.Add(row2.Cells[0].Value.ToString());
            }
            var result = MessageBox.Show("Ви уверены что хотите удалить\nзаписи со следующими номерами:\n" + String.Join(", ", ids), "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ExecuteQuery(@"
DELETE FROM car_service.jobs AS j
WHERE j.id IN (" + String.Join(", ", ids) + ")");
                    LoadJobs(); ;
                }
                catch
                {
                    MessageBox.Show("Ошибка", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void buttonDelPrice_Click(object sender, EventArgs e)
        {
            List<string> ids = new List<string>();
            foreach (var row in dataGridViewPrice.SelectedRows)
            {
                DataGridViewRow row2 = row as DataGridViewRow;
                ids.Add(row2.Cells[0].Value.ToString());
            }
            var result = MessageBox.Show("Ви уверены что хотите удалить\nзаписи со следующими номерами:\n" + String.Join(", ", ids), "Удаление данных", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                try
                {
                    ExecuteQuery(@"
DELETE FROM car_service.price AS p
WHERE p.id IN (" + String.Join(", ", ids) + ")");
                    LoadPrice(); ;
                }
                catch
                {
                    MessageBox.Show("Ошибка", "Удаление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void buttonAddWorkers_Click(object sender, EventArgs e)
        {
            string name = textBoxWorkersName1.Text.Trim();
            ComboBoxItem box = comboBoxWorkersBox1.SelectedItem as ComboBoxItem;
            if (name != "" && box != null)
            {
                ExecuteQuery(@"
INSERT INTO car_service.worker(name, box_id) VALUES ('" + name + "', " + box.Value + ")");
                textBoxWorkersName1.Text = "";
                comboBoxWorkersBox1.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Ошибка: все поля должны\nбыть заполнены", "Добавление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadWorkers();
        }

        private void buttonAddJob_Click(object sender, EventArgs e)
        {
            string typesOfJobs = textBoxJobsTypesOfJobs1.Text.Trim();
            if (typesOfJobs != "")
            {
                ExecuteQuery(@"
INSERT INTO car_service.jobs(types_of_jobs) VALUES ('" + typesOfJobs + "')");
                textBoxJobsTypesOfJobs1.Text = "";
            }
            else
            {
                MessageBox.Show("Ошибка: все поля должны\nбыть заполнены", "Добавление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadJobs();
        }

        private void buttonAddBox_Click(object sender, EventArgs e)
        {
            string BoxNumber = textBoxBoxNumber.Text.Trim();
            if (BoxNumber != "")
            {
                ExecuteQuery(@"
INSERT INTO car_service.box(box_number) VALUES ('" + BoxNumber + "')");
                textBoxBoxNumber.Text = "";
            }
            else
            {
                MessageBox.Show("Ошибка: все поля должны\nбыть заполнены", "Добавление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadBox();

        }

        private void buttonAddPrice_Click(object sender, EventArgs e)
        {
            ComboBoxItem typeOfJobs = comboBoxPriceTypeOfJobs.SelectedItem as ComboBoxItem;
            float price = (float)numericUpDownPricePrice.Value;
            DateTime date = dateTimePickerPriceDate.Value;
            if (typeOfJobs != null && price > 0)
            {
                Debug.WriteLine(date.ToString("DD.MM.YYYY"));
                ExecuteQuery(@"
INSERT INTO car_service.price(jobs_id, price, date) VALUES (" + typeOfJobs.Value + ", " + price + ", '" + date.ToString("dd.MM.yyyy") + "')");
                comboBoxPriceTypeOfJobs.SelectedItem = null;
                numericUpDownPricePrice.Value = 0;
                dateTimePickerPriceDate.Value = DateTime.Now;
            }
            else
            {
                MessageBox.Show("Ошибка: все поля должны\nбыть заполнены", "Добавление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            LoadPrice();
        }



        private void LoadCar()
        {
            FillGridView(dataGridViewCar, @"
SELECT c1.id AS ID, c1.make AS Марка, c1.model AS Модель, c1.year AS Год, c1.volume AS Объем, c1.vin_code AS Винкод, c2.name AS Имя, c2.phone AS Телефон
FROM car_service.car AS c1
JOIN car_service.client AS c2 ON c2.id = c1.client_id
");
        }

        private void buttonSearchCar_Click(object sender, EventArgs e)
        {
            string Vin = textBoxVinSearch.Text.Trim();
            if (Vin != "")
            {
                FillGridView(dataGridViewCar, @"
SELECT c1.id AS ID, c1.make AS Марка, c1.model AS Модель, c1.year AS Год, c1.volume AS Объем, c1.vin_code AS Винкод, c2.name AS Имя, c2.phone AS Телефон
FROM car_service.car AS c1
JOIN car_service.client AS c2 ON c2.id = c1.client_id
WHERE c1.vin_code LIKE '%" + Vin + @"%'
");
            }
            else
            {
                LoadCar();
            }
        }
        private void LoadWork()
        {
            FillGridView(dataGridViewWork, @"
SELECT jr.id AS ID, c1.id AS ID_авто, c1.make AS Марка, c1.model AS Модель, c1.vin_code AS Винкод, w.id AS ID_работника, w.name AS Имя, b.box_number AS Номер_Бокса 
FROM car_service.job_records AS jr
left JOIN car_service.car AS c1 ON c1.id = jr.car_id
left JOIN car_service.box AS b ON b.id = jr.box_id
left JOIN car_service.worker AS w ON w.id = jr.worker_id 
WHERE jr.date_completion IS NULL
");
        }

        private void dataGridViewCar_DoubleClick(object sender, EventArgs e)
        {

            string name = dataGridViewCar.SelectedRows[0].Cells[1].Value.ToString();
            string model = dataGridViewCar.SelectedRows[0].Cells[2].Value.ToString();
            string year = dataGridViewCar.SelectedRows[0].Cells[3].Value.ToString();
            string win = dataGridViewCar.SelectedRows[0].Cells[5].Value.ToString();
            string id = dataGridViewCar.SelectedRows[0].Cells[0].Value.ToString();
            new AddJobForm(id, name, model, year, win).ShowDialog();
            LoadWork();
            LoadPayment();
        }

        private void dataGridViewWork_DoubleClick(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("Вы хотите завершить работу с этим\nавто и выдать клиенту?", "Видача авто", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (res == DialogResult.Yes)
            {
                string id = dataGridViewWork.SelectedRows[0].Cells["id"].Value.ToString();
                string box_number = dataGridViewWork.SelectedRows[0].Cells["Номер_Бокса"].Value.ToString();
                string worker_id = dataGridViewWork.SelectedRows[0].Cells["id_работника"].Value.ToString();
                ExecuteQuery(@"
UPDATE car_service.job_records SET date_completion = '" + DateTime.Now.ToString("dd.MM.yyyy") +
    "' WHERE car_service.job_records.id = " + id);
                ExecuteQuery(@"
UPDATE car_service.worker SET box_id = NULL" +
    " WHERE car_service.worker.id = " + worker_id);
                ExecuteQuery(@"
UPDATE car_service.box SET car_id = NULL" +
    " WHERE car_service.box.box_number = " + box_number);
                ExecuteQuery(@"
UPDATE car_service.payments SET pay = true, date_of_payment = '" + DateTime.Now.ToString("dd.MM.yyyy") +
    "' WHERE car_service.payments.job_record_id = " + id);
            }
            LoadWork();
            LoadPayment();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
        private void LoadPayment()
        {
            FillGridView(dataGridViewPayments, @"
SELECT p.id AS ID, c.name AS Клиент, j.types_of_jobs AS Вид_работ, p.date_of_payment AS Дата_выдачи, p.total_sum AS Сумма, p.pay AS Оплачен
FROM car_service.payments AS p
Join car_service.client AS c ON c.id = p.client_id
Join car_service.job_records AS jr ON jr.id = p.job_record_id
Join car_service.jobs AS j ON j.id = jr.job_id
");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string report = (comboBox1.SelectedItem as ComboBoxItem).Value;
            string workerId, date1, date2;
            switch (report)
            {
                case "Report 1":
                    workerId = (comboBox2.SelectedItem as ComboBoxItem).Value;
                    date1 = dateTimePicker1.Value.ToString("dd.MM.yyyy");
                    date2 = dateTimePicker2.Value.ToString("dd.MM.yyyy");
                    FillGridView(dataGridView1, @"
select distinct c.id, c.make, c.model, c.""year"", c.vin_code 
from car_service.job_records jr
join car_service.car c on jr.car_id = c.id 
where jr.worker_id = " + workerId + @" and jr.date_completion  between '" + date1 + @"' and '" + date2 + @"'
order by c.make, c.model, c.""year""
");
                    break;

                case "Report 2":
                    date1 = dateTimePicker1.Value.ToString("dd.MM.yyyy");
                    date2 = dateTimePicker2.Value.ToString("dd.MM.yyyy");
                    FillGridView(dataGridView1, @"
select c.id, c.make, c.model, c.""year"", c.vin_code, COUNT(c.id) AS total
from car_service.job_records jr 
join car_service.car c on jr.car_id = c.id 
where jr.date_completion  between '" + date1 + @"' and '" + date2 + @"'
group by c.id;
");
                    //int total = 0;
                    //foreach (var row in dataGridView1.Rows)
                    //{
                    //    DataGridViewRow row2 = row as DataGridViewRow;
                    //    try
                    //    {
                    //        total += int.Parse(row2.Cells[5].Value.ToString());
                    //    }
                    //    catch (Exception) {}
                    //}
                    //dataGridView1.Rows.Add(new object[] { "", "", "", "", "Общее количество", total });
                    break;

                case "Report 3":
                    FillGridView(dataGridView1, @"
select w.id, w.""name"" , count(*) as total 
from car_service.job_records jr 
join car_service.worker w on jr.worker_id = w.id
group by w.id ;
");
                    break;

                case "Report 4":
                    FillGridView(dataGridView1, @"
select all w.""name"", jr.worker_id, count(*) as Total  
FROM car_service.job_records jr  
join car_service.worker w  on w.id = jr.worker_id
group by w.""name"", jr.worker_id
order by total desc;
");
                    break;

                case "Report 5":
                    FillGridView(dataGridView1, @"
select j.types_of_jobs, w.""name"" , COUNT (w.id) as Total
from car_service.job_records jr 
join car_service.jobs j on j.id = jr.job_id  
join car_service.worker w  on w.id = jr.worker_id
group by j.types_of_jobs, w.""name"" 
order by j.types_of_jobs , total desc;
");

                    break;
                case "Report 6":
                    date1 = dateTimePicker1.Value.ToString("dd.MM.yyyy");
                    date2 = dateTimePicker2.Value.ToString("dd.MM.yyyy");
                    FillGridView(dataGridView1, @"
select w.id , w.""name"" 
from car_service.worker w 
where w.id not in (
     select distinct jr.worker_id 
     from car_service.job_records jr 
     where jr.date_completion between '" + date1 + @"' and '" + date2 + @"')
");
                    break;

                case "Report 7":
                    FillGridView(dataGridView1, @"
select distinct  w.id, w.name, 'Обслужил максимальное количество машин' as description
from car_service.worker w
join car_service.job_records jr on w.id = jr.worker_id 
where w.id in (
	SELECT jr1.worker_id 
	FROM car_service.job_records jr1
	GROUP BY jr1.worker_id  
	HAVING COUNT(*) = (
	    SELECT MAX(client_count)
	    FROM (
	        SELECT COUNT(*) AS client_count
	        FROM car_service.job_records jr2
	        GROUP BY jr2.worker_id 
	    ) AS counts
	)
)
union 
select distinct  w.id, w.name, 'В данный момент не работает в боксе' as description
from car_service.worker w
where w.box_id is null
union 
select distinct  w.id, w.name, 'В данный момент работает в боксе' as description
from car_service.worker w
where w.box_id is not null;
");

                    break;
                default:
                    break;
            }
        }

        private void LoadReportsList()
        {
            comboBox1.Items.Add(new ComboBoxItem("1. Список машин которые обслуживал работник за период.", "Report 1"));
            comboBox1.Items.Add(new ComboBoxItem("2. Сколько работ было выполнено за период.", "Report 2"));
            comboBox1.Items.Add(new ComboBoxItem("3. Сколько обслужил машин каждый работник.", "Report 3"));
            comboBox1.Items.Add(new ComboBoxItem("4. Кто из работников обслужил больше всего машин.", "Report 4"));
            comboBox1.Items.Add(new ComboBoxItem("5. Для видов работ и работников определить количество обслуженных машин.", "Report 5"));
            comboBox1.Items.Add(new ComboBoxItem("6. Кто из работников не работал за период.", "Report 6"));
            comboBox1.Items.Add(new ComboBoxItem("7. Список работников с коментарием.", "Report 7"));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string report = (comboBox1.SelectedItem as ComboBoxItem).Value;
            if (report == "Report 1")
            {
                label18.Visible = true;
                comboBox2.Visible = true;
            }
            else
            {
                label18.Visible = false;
                comboBox2.Visible = false;
            }
            if (report == "Report 1" || report == "Report 2" || report == "Report 6")
            {
                label16.Visible = true;
                label17.Visible = true;
                dateTimePicker1.Visible = true;
                dateTimePicker2.Visible = true;
            }
            else
            {
                label16.Visible = false;
                label17.Visible = false;
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string make = addMarkTextBox.Text;
            string model = addModelTextBox.Text;
            string year = addYearTextBox.Text;
            string volume = addVolumeTextBox.Text;
            string vinCode = addVinTextBox.Text;
            string name = addNameTextBox.Text;
            string phone = addPhoneTextBox.Text;
            DataTable clientIdTable = Form1.ExecuteQuery(@"
INSERT INTO car_service.client(name, phone) 
VALUES ('" + name + "', '" + phone + "') RETURNING id", hasReturnedValue: true);
            string clientId = clientIdTable.Rows[0]["id"].ToString();
            Form1.ExecuteQuery(@"
INSERT INTO car_service.car(make, model, year, volume, vin_code, client_id) 
VALUES ('" + make + "', '" + model + "' ," + year + ", " + volume + ", '" + vinCode + "', " + clientId + ")");
            LoadCar();
        }
    }
}
