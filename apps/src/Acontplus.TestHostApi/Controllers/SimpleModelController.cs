namespace Acontplus.TestHostApi.Controllers
{
    public class SimpleModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
    }

    public class SimpleModelController : BaseApiController
    {
        [HttpGet]
        [Route("map-data-row")]
        public ActionResult<SimpleModel> GetMappedModel()
        {
            try
            {
                DataTable dt = CreateSampleDataTable();
                SimpleModel model = DataTableMapper.MapDataRowToModel<SimpleModel>(dt.Rows[0]);
                return Ok(model);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }


        }

        [HttpGet]
        [Route("map-data-list")]
        public ActionResult<List<SimpleModel>> GetMappedModels()
        {
            try
            {
                DataTable dt = CreateSampleDataTable();
                var serialized = DataConverters.DataTableToJson(dt);

                List<SimpleModel> models = DataTableMapper.MapDataTableToList<SimpleModel>(dt);
                return Ok(models);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        private DataTable CreateSampleDataTable()
        {
            DataTable dataTable = new DataTable("SampleTable");
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Age", typeof(int));
            dataTable.Columns.Add("Email", typeof(string));
            // Adding sample data
            dataTable.Rows.Add("John Doe", 30, "john.doe@example.com"); dataTable.Rows.Add("Jane Smith", 25, "jane.smith@example.com");
            return dataTable;
        }
    }
}
