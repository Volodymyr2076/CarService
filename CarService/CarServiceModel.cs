using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CarService
{
    // using pattern Singleton
    public class CarServiceModel
    {
        private static NpgsqlConnection connection;
        private static CarServiceModel instance;
        private static string connectionstring = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=25542554";

        private CarServiceModel()
        {
            connection = new NpgsqlConnection(connectionstring);
            connection.Open();
        }

        public static CarServiceModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new CarServiceModel();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        public void Close()
        {
            connection.Dispose();
            connection.Close();
            instance = null;
        }

        private DataTable ExecuteQuery(string sql)
        {
            NpgsqlCommand npgsqlCommand = new NpgsqlCommand();
            npgsqlCommand.Connection = connection;
            npgsqlCommand.CommandType = CommandType.Text;
            npgsqlCommand.CommandText = sql;
            NpgsqlDataReader reader = npgsqlCommand.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(reader);
            return dt;
        }

        public DataTable GetWorkers()
        {
            return ExecuteQuery(@"
SELECT w.id AS ID, w.name AS Имя, b.box_number AS Бокс
FROM car_service.worker AS w
LEFT JOIN car_service.box AS b ON b.id = w.box_id
");
        }

        public DataTable GetBox()
        {
            return ExecuteQuery(@"
SELECT b.id AS ID, b.box_number AS Номер_Бокса, b.car_id AS Автомобиль
FROM car_service.box AS b
");
        }

        public DataTable GetJobs()
        {
            return ExecuteQuery(@"
SELECT j.id AS ID, j.types_of_jobs AS Наименование_Работ
FROM car_service.jobs AS j
");
        }

        public DataTable GetPrice()
        {
            return ExecuteQuery(@"
SELECT p.id AS ID, j.types_of_jobs  AS Наименование_Работ, p.price as Цена, p.date as Дата
FROM car_service.price AS p
join car_service.jobs as j on j.id  = p.jobs_id
");
        }

        public string GetPriceByJob(string job, string date)
        {
            DataTable dt = ExecuteQuery(@"
SELECT * FROM car_service.get_price(" + job + ", '" + date + "')");
            return dt.Rows[0]["price"].ToString();
        }

        public string GetClientIdByCarId(string carId)
        {
            DataTable dt = ExecuteQuery(@"
SELECT cl.id FROM car_service.client AS cl JOIN car_service.car AS c ON cl.id = c.client_id WHERE c.id = " + carId);
            return dt.Rows[0]["id"].ToString();
        }

        public DataTable GetCar(string vinCode = "")
        {
            return ExecuteQuery(@"
SELECT c1.id AS ID, c1.make AS Марка, c1.model AS Модель, c1.year AS Год, c1.volume AS Объем, c1.vin_code AS Винкод, c2.name AS Имя, c2.phone AS Телефон
FROM car_service.car AS c1
JOIN car_service.client AS c2 ON c2.id = c1.client_id" +
((vinCode != "") ? "WHERE c1.vin_code LIKE '%" + vinCode + @"%'" : ""));
        }

        public DataTable GetWork()
        {
            return ExecuteQuery(@"
SELECT jr.id AS ID, c1.id AS ID_авто, c1.make AS Марка, c1.model AS Модель, c1.vin_code AS Винкод, w.id AS ID_работника, w.name AS Имя, b.box_number AS Номер_Бокса 
FROM car_service.job_records AS jr
left JOIN car_service.car AS c1 ON c1.id = jr.car_id
left JOIN car_service.box AS b ON b.id = jr.box_id
left JOIN car_service.worker AS w ON w.id = jr.worker_id 
WHERE jr.date_completion IS NULL
");
        }

        public DataTable GetPayment()
        {
            return ExecuteQuery(@"
SELECT p.id AS ID, c.name AS Клиент, j.types_of_jobs AS Вид_работ, p.date_of_payment AS Дата_выдачи, p.total_sum AS Сумма, p.pay AS Оплачен
FROM car_service.payments AS p
Join car_service.client AS c ON c.id = p.client_id
Join car_service.job_records AS jr ON jr.id = p.job_record_id
Join car_service.jobs AS j ON j.id = jr.job_id
");
        }

        public void DeleteWorkers(List<string> ids)
        {
            ExecuteQuery(@"
DELETE FROM car_service.worker AS w
WHERE w.id IN (" + string.Join(", ", ids) + ")");
        }

        public void DeleteBox(List<string> ids)
        {
            ExecuteQuery(@"
DELETE FROM car_service.box AS b
WHERE b.id IN (" + string.Join(", ", ids) + ")");
        }

        public void DeleteJob(List<string> ids)
        {
            ExecuteQuery(@"
DELETE FROM car_service.jobs AS j
WHERE j.id IN (" + string.Join(", ", ids) + ")");
        }

        public void DeletePrice(List<string> ids)
        {
            ExecuteQuery(@"
DELETE FROM car_service.price AS p
WHERE p.id IN (" + string.Join(", ", ids) + ")");
        }

        public string CreateWorker(string name, string boxId)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.worker(name, box_id) VALUES ('" + name + "', " + boxId + ") RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreateJobs(string typesOfJobs)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.jobs(types_of_jobs) VALUES ('" + typesOfJobs + "') RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreateBox(string boxNumber)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.box(box_number) VALUES ('" + boxNumber + "') RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreatePrice(string typeOfJobsId, string price, string date)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.price(jobs_id, price, date) VALUES (" + typeOfJobsId + ", " + price + ", '" + date + "') RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreateClient(string name, string phone)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.client(name, phone) 
VALUES ('" + name + "', '" + phone + "') RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreateCar(string make, string model, string year, string volume, string vinCode, string clientId)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.car(make, model, year, volume, vin_code, client_id) 
VALUES ('" + make + "', '" + model + "' ," + year + ", " + volume + ", '" + vinCode + "', " + clientId + ") RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreatePayment(string clientId, string jobRecordId, string price)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.payments(client_id, job_record_id, total_sum) 
VALUES (" + clientId + ", " + jobRecordId + ", " + price + ") RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public string CreateJobRecords(string jobId, string carId, string workerId, string date, string boxId)
        {
            DataTable dt = ExecuteQuery(@"
INSERT INTO car_service.job_records(job_id,car_id,worker_id,date_begin,box_id) 
VALUES (" + jobId + ", " + carId + " ," + workerId + ", '" + date + "', " + boxId + ") RETURNING id");
            return dt.Rows[0]["id"].ToString();
        }

        public void UpdateBox(string boxId, string carId)
        {
            ExecuteQuery(@"
UPDATE car_service.box SET car_id = " + carId + " WHERE car_service.box.id = " + boxId);
        }

        public void UpdateBoxByBoxNumber(string boxNumber, string carId)
        {
            ExecuteQuery(@"
UPDATE car_service.box SET car_id = " + carId + " WHERE car_service.box.box_number = " + boxNumber);
        }

        public void UpdateWoker(string workerId, string boxId)
        {
            ExecuteQuery(@"
UPDATE car_service.worker SET box_id = " + boxId + " WHERE car_service.worker.id = " + workerId);
        }

        public void UpdateJobRecordsDateCompletion(string jobRecordsId)
        {
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            ExecuteQuery(@"
UPDATE car_service.job_records SET date_completion = '" + date + "' WHERE car_service.job_records.id = " + jobRecordsId);
        }

        public void UpdatePaymentsStatus(string jobRecordsId)
        {
            string date = DateTime.Now.ToString("dd.MM.yyyy");
            ExecuteQuery(@"
UPDATE car_service.payments SET pay = true, date_of_payment = '" + date + "' WHERE car_service.payments.job_record_id = " + jobRecordsId);
        }

        public DataTable GetReport1(string workerId, string date1, string date2)
        {
            return ExecuteQuery(@"
select distinct c.id, c.make, c.model, c.""year"", c.vin_code 
from car_service.job_records jr
join car_service.car c on jr.car_id = c.id 
where jr.worker_id = " + workerId + @" and jr.date_completion  between '" + date1 + @"' and '" + date2 + @"'
order by c.make, c.model, c.""year""
");
        }

        public DataTable GetReport2(string date1, string date2)
        {
            return ExecuteQuery(@"
select c.id, c.make, c.model, c.""year"", c.vin_code, COUNT(c.id) AS total
from car_service.job_records jr 
join car_service.car c on jr.car_id = c.id 
where jr.date_completion  between '" + date1 + @"' and '" + date2 + @"'
group by c.id;
");
        }

        public DataTable GetReport3()
        {
            return ExecuteQuery(@"
select w.id, w.""name"" , count(*) as total 
from car_service.job_records jr 
join car_service.worker w on jr.worker_id = w.id
group by w.id ;
");
        }

        public DataTable GetReport4()
        {
            return ExecuteQuery(@"
select all w.""name"", jr.worker_id, count(*) as Total  
FROM car_service.job_records jr  
join car_service.worker w  on w.id = jr.worker_id
group by w.""name"", jr.worker_id
order by total desc;
");
        }

        public DataTable GetReport5()
        {
            return ExecuteQuery(@"
select j.types_of_jobs, w.""name"" , COUNT (w.id) as Total
from car_service.job_records jr 
join car_service.jobs j on j.id = jr.job_id  
join car_service.worker w  on w.id = jr.worker_id
group by j.types_of_jobs, w.""name"" 
order by j.types_of_jobs , total desc;
");
        }

        public DataTable GetReport6(string date1, string date2)
        {
            return ExecuteQuery(@"
select w.id , w.""name"" 
from car_service.worker w 
where w.id not in (
     select distinct jr.worker_id 
     from car_service.job_records jr 
     where jr.date_completion between '" + date1 + @"' and '" + date2 + @"')
");
        }

        public DataTable GetReport7()
        {
            return ExecuteQuery(@"
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
        }
    }
}
