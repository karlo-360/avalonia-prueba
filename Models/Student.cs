using System.Text.Json;

public class Student
{

    public string? Name { get; set; }
    public int Id { get; set; }
    public Course[]? Courses { get; set; }

    public Student (string name, int id)
    {
        Name = name;
        Id = id;
    }

    public Student (JsonElement studentRoot)
    {
        Name = studentRoot.GetProperty("name").GetString();
        Id = studentRoot.GetProperty("id").GetInt32();
    }

}
