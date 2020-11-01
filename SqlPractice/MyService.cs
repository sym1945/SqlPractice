using System;
using System.Reflection;
using System.Transactions;

namespace SqlPractice
{
    
    public class MyService
    {
        private readonly TestRepository _TestRepo = new TestRepository();
        private readonly ProductRepository _ProductRepo = new ProductRepository();

        public void TransactionTest()
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    _TestRepo.Insert(new Test
                    {
                        아이디 = 3,
                        이름 = "Two",
                        생성시간 = DateTime.Now,
                    });

                    _ProductRepo.Insert(new Product
                    {
                        Id = 2,
                        Name = "Two",
                        Price = 4000,
                    });

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}