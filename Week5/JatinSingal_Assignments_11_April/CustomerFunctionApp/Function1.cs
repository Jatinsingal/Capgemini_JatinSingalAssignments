using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using CustomerFunctionApp.Services;
using CustomerFunctionApp.Models;

namespace CustomerFunctionApp
{
    public class CustomerFunction
    {
        private readonly CustomerService _service;

        public CustomerFunction(CustomerService service)
        {
            _service = service;
        }

        // GET ALL
        [Function("GetAllCustomers")]
        public async Task<HttpResponseData> GetAllCustomers(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers")] HttpRequestData req)
        {
            var customers = await _service.GetAllCustomers();

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customers);
            return response;
        }

        // GET BY ID
        [Function("GetCustomerById")]
        public async Task<HttpResponseData> GetCustomerById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "customers/{id:int}")] HttpRequestData req,
            int id)
        {
            var customer = await _service.GetCustomerById(id);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(customer);
            return response;
        }

        // CREATE
        [Function("CreateCustomer")]
        public async Task<HttpResponseData> CreateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "customers")] HttpRequestData req)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = JsonSerializer.Deserialize<Customer>(requestBody);

            await _service.CreateCustomer(customer);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Customer Created");
            return response;
        }

        // UPDATE
        [Function("UpdateCustomer")]
        public async Task<HttpResponseData> UpdateCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "customers/{id:int}")] HttpRequestData req,
            int id)
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var customer = JsonSerializer.Deserialize<Customer>(requestBody);

            await _service.UpdateCustomer(id, customer);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Customer Updated");
            return response;
        }

        // DELETE
        [Function("DeleteCustomer")]
        public async Task<HttpResponseData> DeleteCustomer(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "customers/{id:int}")] HttpRequestData req,
            int id)
        {
            await _service.DeleteCustomer(id);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteStringAsync("Customer Deleted");
            return response;
        }
    }
}