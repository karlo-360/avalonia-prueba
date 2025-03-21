using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.Json;
using System.Net.Http;
using System;

using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Media;

namespace crisApp;

public partial class MainWindow : Window
{

    private HttpClient client = new HttpClient();

    public MainWindow()
    {

        InitializeComponent();

        this.WindowState = WindowState.FullScreen;

        this.Closing += (s, e) => 
        {
            e.Cancel = true;
        };

        BotonGet.Background = Brushes.Blue;
        BotonPost.Background = Brushes.Red;
        
    }

    private async void Button_Get_OnClick(object? sender, RoutedEventArgs e)
    {

        //var request = new HttpRequestMessage(HttpMethod.Get, "https://cep.prototipo-web.com/api/alumno/2018088928");
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://cep.prototipo-web.com/api/alumno/{Nombre.Text}");

        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        using JsonDocument doc = JsonDocument.Parse(content);

        JsonElement root = doc.RootElement;

        JsonElement studentRoot = root.GetProperty("data").GetProperty("alumno");

        Student student = new Student(studentRoot);

        JsonElement coursesRoot = root.GetProperty("data").GetProperty("materias");

        List<Course> courses = new List<Course>();

        for (int i = 0; i < coursesRoot.GetArrayLength(); i++)
        {
            Course course = new Course(coursesRoot[i]);
            courses.Add(course);
        }

        Console.WriteLine(student.Name);
        Console.WriteLine(student.Id);

        foreach (var course in courses)
        {
           Console.WriteLine(course.Name); 
           Console.WriteLine(course.Id);
        }

        //Console.WriteLine(await response.Content.ReadAsStringAsync());
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Console.WriteLine("linux");

            //Console.WriteLine(RunCommandWithBash("-h now"));
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine("windows");

            //Console.WriteLine(RunCommandWithBash("-s -t 0"));
        }

        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

        foreach (NetworkInterface networkInterface in networkInterfaces)
        {
            if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }
            
            if (networkInterface.OperationalStatus == OperationalStatus.Up)
            {
                Console.WriteLine($"hola {networkInterface.GetPhysicalAddress().ToString()}");
            }

        }

        Ping ping = new Ping();

        try 
        {
            PingReply reply = ping.Send("www.google.com");
            Console.WriteLine(reply.Status.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        

        //Console.WriteLine($"Click: {Nombre.Text}");
    }

    //private async void Button_Post_OnClick(object? sender, RoutedEventArgs e)
    private void Button_Post_OnClick(object? sender, RoutedEventArgs e)
    {

        DateTime date = DateTime.Now;

        Dictionary<string, string> postData = new Dictionary<string, string>();
        postData.Add("alumno_id", "1");
        postData.Add("materia_id", "1");
        postData.Add("register_date", date.ToString("yyyy/MM/dd"));
        postData.Add("mac_address", "4C:ED:FB:3D:60:4E");

        try
        {
            //string result = await PostHTTPRequestAsync("https://cep.prototipo-web.com/api/register-use", postData);
            //Console.WriteLine(result);
            Console.WriteLine("");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
        this.Closing += (s, e) => 
        {
            e.Cancel = false;
        };

        this.Close();
    }

    private async Task<string> PostHTTPRequestAsync(string url, Dictionary<string, string> data)
    {
        using (HttpContent formContent = new FormUrlEncodedContent(data))
        {
            using (HttpResponseMessage response = await client.PostAsync(url, formContent).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

    }

    public string RunCommandWithBash(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo();

        psi.FileName = "shutdown";
        psi.Arguments = command;
        psi.RedirectStandardOutput = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = false;

        using var process = Process.Start(psi);

        process.WaitForExit();

        var output = process.StandardOutput.ReadToEnd();

        return output;
    }
}
