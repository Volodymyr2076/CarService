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
        private CarServiceModel Model { get; set; }

        public AddJobForm()
        {
            InitializeComponent();
            Model = CarServiceModel.Instance;
        }

        public AddJobForm(string id, string name, string model, string year, string vin)
        {
            InitializeComponent();
            label1.Text = id;
            textBox1.Text = name;
            textBox2.Text = model;
            textBox3.Text = year;
            textBox4.Text = vin;
            Model = CarServiceModel.Instance;
        }

        private void AddJobForm_Load(object sender, EventArgs e)
        {
            Helper.FillListControl(listBox1, Model.GetJobs());
            Helper.FillListControl(listBox2, Model.GetWorkers());
            Helper.FillListControl(listBox3, Model.GetBox());
        }

        private void buttonOfJob_Click(object sender, EventArgs e)
        {
            ComboBoxItem box = (ComboBoxItem)listBox3.SelectedItem;
            ComboBoxItem worker = (ComboBoxItem)listBox2.SelectedItem;
            ComboBoxItem job = (ComboBoxItem)listBox1.SelectedItem;
            string carId = label1.Text.Trim();
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            try
            {
                Model.UpdateBox(box.Value, carId);
                Model.UpdateWoker(box.Value, worker.Value);
                string jobRecordId = Model.CreateJobRecords(job.Value, carId, worker.Value, date, box.Value);
                string price = Model.GetPriceByJob(job.Value, date);
                string clientId = Model.GetClientIdByCarId(carId);
                Model.CreatePayment(clientId, jobRecordId, price);
                this.Close();
            }
            catch
            {
                MessageBox.Show("Ошибка: не удалось добавить автомобиль в работу ", "Добавление данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
