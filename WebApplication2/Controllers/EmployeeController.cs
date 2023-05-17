using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;

        }

        [HttpGet()]
        public JsonResult Get()
        {
            string query = @"
                            select employeeId, employeeName, department, 
                            convert(varchar(10), dateOfJoining,120) as DateOfJoining, photoFileName 
                            from employees
                            ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();

                }
            }
            return new JsonResult(table);

        }
        [HttpPost()]
        public JsonResult Post(Employee emp)
        {
            string query = @"
                            insert into employees
                            (employeeName, department,dateOfJoining,photoFileName) 
                            values (@employeeName, @department, @dateOfJoining, @photoFileName)";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@employeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@department", emp.Department);
                    myCommand.Parameters.AddWithValue("@dateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@photoFileName", emp.PhotoFileName);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();

                }
            }
            return new JsonResult("added successfully");

        }

        [HttpPut()]
        public JsonResult Put(Employee emp)
        {
            string query = @"
                            update employees
                            set employeeName = @employeeName,
                                department = @department,
                                dateOfJoining =@dateOfJoining,
                                photoFileName = @photoFileName
                            where employeeId = @employeeId";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@employeeId", emp.EmployeeId);
                    myCommand.Parameters.AddWithValue("@employeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@department", emp.Department);
                    myCommand.Parameters.AddWithValue("@dateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@photoFileName", emp.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();

                }
            }
            return new JsonResult("updated successfully");

        }

        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                            delete from employees
                            where employeeId = @employeeId";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@employeeId", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();

                }
            }
            return new JsonResult("deleted succefully");

        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {

            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + fileName;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(fileName);

            }
            catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }
    }
}
