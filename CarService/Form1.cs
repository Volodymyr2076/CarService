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
        private CarServiceModel Model { get; set; }

        public Form1()
        {
            InitializeComponent();
            Model = CarServiceModel.Instance;
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Тут должен быть код с выведением информации
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

        private void LoadWorkers()
        {
            DataTable workersTable = Model.GetWorkers();
            Helper.FillDataGridView(dataGridViewWorkers, workersTable);
            Helper.FillListControl(comboBox2, workersTable);
        }

        private void LoadBox()
        {
            DataTable boxTable = Model.GetBox();
            Helper.FillDataGridView(dataGridViewBox, boxTable);
            Helper.FillListControl(comboBoxWorkersBox1, boxTable);
        }

        private void LoadJobs()
        {
            DataTable jobsTable = Model.GetJobs();
            Helper.FillDataGridView(dataGridViewJobs, jobsTable);
            Helper.FillListControl(comboBoxPriceTypeOfJobs, jobsTable);
        }

        private void LoadPrice()
        {
            DataTable priceTable = Model.GetPrice();
            Helper.FillDataGridView(dataGridViewPrice, priceTable);
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
                    Model.DeleteWorkers(ids);
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
                    Model.DeleteBox(ids);
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
                    Model.DeleteJob(ids);
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
                    Model.DeletePrice(ids);
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
                Model.CreateWorker(name, box.Value);
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
                Model.CreateJobs(typesOfJobs);
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
            string boxNumber = textBoxBoxNumber.Text.Trim();
            if (boxNumber != "")
            {
                Model.CreateBox(boxNumber);
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
                Model.CreatePrice(typeOfJobs.Value, price.ToString(), date.ToString("dd.MM.yyyy"));
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
            DataTable CarTable = Model.GetCar();
            Helper.FillDataGridView(dataGridViewCar, CarTable);
        }

        private void buttonSearchCar_Click(object sender, EventArgs e)
        {
            string Vin = textBoxVinSearch.Text.Trim();
            DataTable CarTable = Model.GetCar(Vin);
            Helper.FillDataGridView(dataGridViewCar, CarTable);
        }

        private void LoadWork()
        {
            DataTable WorkTable = Model.GetWork();
            Helper.FillDataGridView(dataGridViewWork, WorkTable);
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
                string workerId = dataGridViewWork.SelectedRows[0].Cells["id_работника"].Value.ToString();
                string boxNumber = dataGridViewWork.SelectedRows[0].Cells["Номер_Бокса"].Value.ToString();
                Model.UpdateJobRecordsDateCompletion(id);
                Model.UpdatePaymentsStatus(id);
                Model.UpdateWoker(workerId, "NULL");
                Model.UpdateBoxByBoxNumber(boxNumber, "NULL");
            }
            LoadWork();
            LoadPayment();
        }

        private void LoadPayment()
        {
            DataTable PaymentTable = Model.GetPayment();
            Helper.FillDataGridView(dataGridViewPayments, PaymentTable);
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
                    Helper.FillDataGridView(dataGridView1, Model.GetReport1(workerId, date1, date2));
                    break;

                case "Report 2":
                    date1 = dateTimePicker1.Value.ToString("dd.MM.yyyy");
                    date2 = dateTimePicker2.Value.ToString("dd.MM.yyyy");
                    Helper.FillDataGridView(dataGridView1, Model.GetReport2(date1, date2));
                    break;

                case "Report 3":
                    Helper.FillDataGridView(dataGridView1, Model.GetReport3());

                    break;

                case "Report 4":
                    Helper.FillDataGridView(dataGridView1, Model.GetReport4());
                    break;

                case "Report 5":
                    Helper.FillDataGridView(dataGridView1, Model.GetReport5());
                    break;
                case "Report 6":
                    date1 = dateTimePicker1.Value.ToString("dd.MM.yyyy");
                    date2 = dateTimePicker2.Value.ToString("dd.MM.yyyy");
                    Helper.FillDataGridView(dataGridView1, Model.GetReport6(date1, date2));
                    break;

                case "Report 7":
                    Helper.FillDataGridView(dataGridView1, Model.GetReport7());
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

        private void button2_Click(object sender, EventArgs e)
        {
            string make = addMarkTextBox.Text;
            string model = addModelTextBox.Text;
            string year = addYearTextBox.Text;
            string volume = addVolumeTextBox.Text;
            string vinCode = addVinTextBox.Text;
            string name = addNameTextBox.Text;
            string phone = addPhoneTextBox.Text;
            string clientId = Model.CreateClient(name, phone);
            Model.CreateCar(make, model, year, volume, vinCode, clientId);
            LoadCar();
        }
    }
}
