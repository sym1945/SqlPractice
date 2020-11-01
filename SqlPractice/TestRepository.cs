using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SqlPractice
{
    public class TestRepository
    {
        private const string CONN_STRING = @"Server=.;Database=TestDb;Trusted_Connection=True;MultipleActiveResultSets=True";

        public List<Test> SelectAll()
        {
            var query = @"
                SELECT * FROM Test
                ";

            List<Test> result = new List<Test>();

            using (var conn = new SqlConnection(CONN_STRING))
            {
                conn.Open();

                using (var command = new SqlCommand(query, conn))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        var mapper = new TestMapper<Test>();

                        while (reader.Read())
                        {
                            var entity = mapper.MapRow(reader);

                            result.Add(entity);
                        }
                    }
                }
            }

            return result;
        }


        public int Insert(Test test)
        {
            var query = @"
                INSERT INTO Test VALUES
                (
                    @Id
                    ,@Name
                    ,@CreateTime
                )
                ";

            using (var conn = new SqlConnection(CONN_STRING))
            {
                conn.Open();
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("Id", test.아이디);
                    command.Parameters.AddWithValue("Name", test.이름);
                    command.Parameters.AddWithValue("CreateTime", test.생성시간);

                    return command.ExecuteNonQuery();
                }
            }
        }


    }
}
