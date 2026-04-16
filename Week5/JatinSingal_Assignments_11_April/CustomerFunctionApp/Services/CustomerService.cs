using Microsoft.Data.SqlClient;
using CustomerFunctionApp.Models;
using Microsoft.Extensions.Configuration;

namespace CustomerFunctionApp.Services
{
    public class CustomerService
    {
        private readonly string _connectionString = string.Empty;

        public CustomerService(IConfiguration configuration)
        {
            _connectionString = configuration["SqlConnectionString"];
        }

        // GET ALL
        public async Task<List<Customer>> GetAllCustomers()
        {
            var customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM Customers";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        customers.Add(new Customer
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString()
                        });
                    }
                }
            }
            return customers;
        }

        // GET BY ID
        public async Task<Customer?> GetCustomerById(int id)
        {
            Customer customer = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "SELECT * FROM Customers WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            customer = new Customer
                            {
                                Id = (int)reader["Id"],
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"].ToString()
                            };
                        }
                    }
                }
            }
            return customer;
        }

        // CREATE
        public async Task CreateCustomer(Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "INSERT INTO Customers (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // UPDATE
        public async Task UpdateCustomer(int id, Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "UPDATE Customers SET Name=@Name, Email=@Email, Phone=@Phone WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Email", customer.Email);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);

                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // DELETE
        public async Task DeleteCustomer(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                string query = "DELETE FROM Customers WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}