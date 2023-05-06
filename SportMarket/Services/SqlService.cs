using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SportMarket.Intarfaces;
using SportMarket.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportMarket.Services
{
    public class SqlService : ISqlService
    {
        private readonly IConfiguration _configuration;
        public SqlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public RequestViewModel SendRequest(string request)
        {
            string connectionString = $"{_configuration.GetConnectionString("DefaultConnection")}";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(request, connection);
                var result = new RequestViewModel();
                var reader = command.ExecuteReader();
                result.Displays = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    result.Displays[i] = reader.GetName(i);
                }

                while (reader.Read()) // построчно считываем данные
                {
                    string[] value = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        value[i] = reader.GetValue(i).ToString();
                    }

                    result.Result.Add(value);
                }

                return result;
            }
        }
    }
}
