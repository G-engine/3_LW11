using System.Text;
using REST;

var httpClientHandler = new HttpClientHandler() {UseDefaultCredentials = true};
httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;

var client = new HttpClient(httpClientHandler) { BaseAddress = new Uri("https://localhost:7123") };

var chapter = await client.GetFromJsonAsync<Employee>("https://localhost:7123/Employee/1");
if (chapter is not null)
{ 
    Console.WriteLine($"{chapter.FirstName} {chapter.LastName}");
}

Employee e = new Employee()
{
    LastName = "string",
    FirstName = "string",
    Title = "string",
    TitleOfCourtesy = "string",
    BirthDate = DateTime.Parse("2022-12-08"),
    HireDate = DateTime.Parse("2022-12-08"),
    Address = "string",
    City = "string",
    Region = "string",
    PostalCode = "string",
    Country = "string",
    HomePhone = "string",
    Extension = "string",
    Photo = Encoding.UTF8.GetBytes("string"),
    Notes = "string",
    ReportsTo = 0,
    PhotoPath = "string"
};

var isSent = await client.PostAsJsonAsync("https://localhost:7123/Employee", e);
if(isSent.IsSuccessStatusCode)
    Console.WriteLine("Successfully added to the db");
else Console.WriteLine("Fail");
