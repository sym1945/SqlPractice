using System.Data.SqlClient;

namespace SqlPractice
{
    public class ProductRepository
    {
        private const string CONN_STRING = @"Server=.;Database=TestDb;Trusted_Connection=True;MultipleActiveResultSets=True";


        public int Insert(Product test)
        {
            var query = @"
                INSERT INTO Product VALUES
                (
                    @Id
                    ,@Name
                    ,@Price
                )
                ";

            using (var conn = new SqlConnection(CONN_STRING))
            {
                conn.Open();
                using (var command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("Id", test.Id);
                    command.Parameters.AddWithValue("Name", test.Name);
                    command.Parameters.AddWithValue("Price", test.Price);

                    return command.ExecuteNonQuery();
                }
            }
        }
    }
}