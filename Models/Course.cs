using System.Text.Json;

public class Course
{

    public string? Name { get; set; }
    public int Id { get; set; }

    public Course (JsonElement courseRoot)
    {
        Name = courseRoot.GetProperty("name").GetString();
        Id = courseRoot.GetProperty("id").GetInt32();
    }

}
