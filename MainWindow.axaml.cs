using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Avalonia.Interactivity;

namespace BackdoorGuard;

public partial class MainWindow : Window {
    private MainWindowViewModel ViewModel { get; }
    private readonly HttpClient _httpClient;

    private string? _baseUrl = Environment.GetEnvironmentVariable("BACKDOOR_GUARD_BASE_URL");

    public MainWindow() {
        InitializeComponent();
        ViewModel = new MainWindowViewModel(new ObservableCollection<OutingSchedule>());
        DataContext = ViewModel;
        _httpClient = new HttpClient();
    }

    private void InitializeComponent() {
        AvaloniaXamlLoader.Load(this);
    }

    public async void AddSchedule(object sender, RoutedEventArgs e) {
        String title = ViewModel.Title;
        DateTime startTime = CombineDateAndTime(ViewModel.SelectedDate, ViewModel.StartTime);
        DateTime endTime = CombineDateAndTime(ViewModel.SelectedDate, ViewModel.EndTime);

        var request = new OutingScheduleRequest(
            title,
            startTime.ToString("yyyy-MM-ddTHH:mm:ss"),
            endTime.ToString("yyyy-MM-ddTHH:mm:ss")
        );
        
        var payload = JsonSerializer.Serialize(request);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync($"{_baseUrl}", content);
        try {
            response.EnsureSuccessStatusCode();
        } catch (Exception exception) {
            Console.WriteLine(exception);
            return;
        }
    }

    public async void DeleteSchedule(object sender, RoutedEventArgs e) {
        if (sender is not Button btn) return;
            
        int index = Convert.ToInt32(btn.Tag?.ToString());
        var response = await _httpClient.DeleteAsync($"{_baseUrl}/{index}");
        try {
            response.EnsureSuccessStatusCode();
        } catch (Exception exception) {
            Console.WriteLine(exception);
            return;
        }

        foreach (OutingSchedule schedule in ViewModel.OutingSchedules.ToList()) {
            if (schedule.ScheduleId == index) {
                ViewModel.OutingSchedules.Remove(schedule);
            }
        }
    }

    public async void RefreshSchedules(object sender, RoutedEventArgs e) {
        var response = await _httpClient.GetAsync(_baseUrl);
        try {
            response.EnsureSuccessStatusCode();
        } catch (Exception exception) {
            Console.WriteLine(exception);
            return;
        }
        
        var data = await response.Content.ReadAsStringAsync();
        var jsonObject = JsonSerializer.Deserialize<JsonArray>(data);
        
        ViewModel.OutingSchedules.Clear();
        
        foreach (var outingSchedule in jsonObject!) {
            ViewModel.OutingSchedules.Add(new OutingSchedule(
                outingSchedule!["id"]!.GetValue<long>(),
                outingSchedule["title"]!.GetValue<string>(),
                DateTime.Parse(outingSchedule["outingStartTime"]!.GetValue<string>()),
                DateTime.Parse(outingSchedule["outingEndTime"]!.GetValue<string>())
            ));
        }
    }
    
    private DateTime CombineDateAndTime(DateTimeOffset date, TimeSpan time)
    {
        return new DateTime(
            date.Year,
            date.Month, 
            date.Day,
            time.Hours,
            time.Minutes,
            time.Seconds
        );
    }
}