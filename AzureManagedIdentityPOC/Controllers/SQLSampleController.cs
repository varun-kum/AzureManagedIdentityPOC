using AzureManagedIdentityPOCAPI.DataAccess;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AzureManagedIdentityPOCAPI.Controllers
{
    public class SQLSampleController : ApiController
    {
        // GET: api/SQLSample
        public string Get()
        {
            try
            {
                SQLSampleDataAccess dataAccess = new SQLSampleDataAccess();
                string name = dataAccess.GetSampleData(1);
                return name;
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
            }
            return "Error";
        }

        // GET: api/SQLSample/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/SQLSample
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/SQLSample/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/SQLSample/5
        public void Delete(int id)
        {
        }
    }
}
