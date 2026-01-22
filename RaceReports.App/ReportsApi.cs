namespace RaceReports.App;

using System.Net.Http.Json;


public class ReportsApi
{
    private readonly HttpClient _http;

    public ReportsApi(HttpClient http)
    {
        _http = http;
    }

    public Task<List<ReportDto>?> GetAll()
        => _http.GetFromJsonAsync<List<ReportDto>>("api/reports");

    public Task<ReportDto?> GetById(int id)
        => _http.GetFromJsonAsync<ReportDto>($"api/reports/{id}");

    public Task<HttpResponseMessage> Create(CreateReportDto dto)
    => _http.PostAsJsonAsync("api/reports", dto);

    public Task<HttpResponseMessage> Update(int id, UpdateReportDto dto)
        => _http.PutAsJsonAsync($"api/reports/{id}", dto);

    public Task<HttpResponseMessage> Delete(int id, int userId)
        => _http.DeleteAsync($"api/reports/{id}?userId={userId}");



    public Task<List<CategoryDto>?> GetCategories()
    => _http.GetFromJsonAsync<List<CategoryDto>>("api/categories");

    public Task<List<ReportDto>?> Search(string? title, int? categoryId)
    {
        var url = "api/reports/search?";
        if (!string.IsNullOrWhiteSpace(title))
            url += $"title={Uri.EscapeDataString(title)}&";
        if (categoryId is not null)
            url += $"categoryId={categoryId.Value}&";

        url = url.TrimEnd('&', '?');
        return _http.GetFromJsonAsync<List<ReportDto>>(url);
    }

    public Task<HttpResponseMessage> CreateComment(CreateCommentDto dto)
    => _http.PostAsJsonAsync("api/comments", dto);


}


public class ReportDto
{
    public int? id { get; set; }
    public string? title { get; set; }
    public string? text { get; set; }
    public DateTime? createdAt { get; set; }
    public string? user { get; set; }
    public string? category { get; set; }
    public int? categoryId { get; set; }          
    public List<CommentDto>? comments { get; set; }
}


public class CommentDto
{
    public int? id { get; set; }
    public string? text { get; set; }
    public DateTime? createdAt { get; set; }
    public string? user { get; set; }
    public int? userId { get; set; }
}

public sealed class CreateCommentDto
{
    public int UserId { get; set; }
    public int RaceReportId { get; set; }
    public string? Text { get; set; }
}



public class CreateReportDto
{
    public string? title { get; set; }
    public string? text { get; set; }
    public string? category { get; set; }
    public int userId { get; set; }
}

public class UpdateReportDto
{
    public string? title { get; set; }
    public string? text { get; set; }
    public int categoryId { get; set; }
    public int userId { get; set; }
}

public class CategoryDto
{
    public int id { get; set; }
    public string? name { get; set; }
}
