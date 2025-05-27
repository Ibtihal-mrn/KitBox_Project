using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Services;
using System;
using System.Collections.Generic;
using Avalonia;
using System.Text.RegularExpressions;

namespace KitBox_Project.Views
{
    public partial class WeeklyCalendar : UserControl
    {
        private readonly List<string> _days = new() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
        private readonly List<string> _hours = new();

        public WeeklyCalendar()
        {
            InitializeComponent();
            InitializeHours();
            DayBox.ItemsSource = _days;
            HourBox.ItemsSource = _hours;
            DayBox.SelectedIndex = 0;
            HourBox.SelectedIndex = 0;
            LoadAppointments();
        }

        private void InitializeHours()
        {
            for (int hour = 8; hour < 16; hour++)
            {
                _hours.Add($"{hour}:00");
                _hours.Add($"{hour}:30");
            }
        }

        private void LoadAppointments()
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.ColumnDefinitions.Clear();
            CalendarGrid.RowDefinitions.Clear();

            for (int i = 0; i <= _days.Count; i++)
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());

            for (int i = 0; i <= _hours.Count; i++)
                CalendarGrid.RowDefinitions.Add(new RowDefinition());

            for (int i = 0; i < _days.Count; i++)
                CalendarGrid.Children.Add(new TextBlock { Text = _days[i], FontWeight = Avalonia.Media.FontWeight.Bold, Margin = new Thickness(2) }.PlaceInGrid(i + 1, 0));

            for (int i = 0; i < _hours.Count; i++)
                CalendarGrid.Children.Add(new TextBlock { Text = _hours[i], FontWeight = Avalonia.Media.FontWeight.Bold, Margin = new Thickness(2) }.PlaceInGrid(0, i + 1));

            foreach (var appt in DatabaseCalendar.GetAllUsers())
            {
                int col = _days.IndexOf(appt.Day) + 1;
                int row = _hours.IndexOf(appt.Hour) + 1;

                if (col > 0 && row > 0)
                {
                    var button = new Button
                    {
                        Content = $"{appt.Name_of_customer}\n{appt.Phone_number}",
                        Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.LightSalmon),
                        FontSize = 11,
                        Tag = appt.Name_of_customer
                    };
                    button.Click += DeleteAppointment_Click;
                    CalendarGrid.Children.Add(button.PlaceInGrid(col, row));
                }
            }
        }

        private void AddAppointment_Click(object sender, RoutedEventArgs e)
        {
            if (NameBox.Text == "" || PhoneBox.Text == "")
            {
                ConfirmationText.Text = "Please fill out all fields.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Red;
                return;
            }

            var day = DayBox.SelectedItem?.ToString() ?? "";
            var hour = HourBox.SelectedItem?.ToString() ?? "";

            if (DatabaseCalendar.GetAppointment(day, hour) != null)
            {
                ConfirmationText.Text = "Time slot already taken.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Red;
                return;
            }

            var name = NameBox.Text ?? "";
            var phone = PhoneBox.Text ?? "";

            var regexPhone = new Regex(@"^(?:\+\d{2} \d{3} \d{2} \d{2} \d{2}|00\d{3} \d{2} \d{2} \d{2} \d{2})$");
            if (!regexPhone.IsMatch(phone))
            {
                ConfirmationText.Text = "Invalid phone number format. Example: +32 546 56 90 98 or 00485 66 88 67 50";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Red;
                return;
            }

            try
            {
                DatabaseCalendar.AddAppointment(day, hour, name, phone);
                ConfirmationText.Text = "Appointment added.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Green;
                LoadAppointments();
            }
            catch (Exception ex)
            {
                ConfirmationText.Text = "Error: slot already taken or invalid.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Red;
                Console.WriteLine(ex.Message);
            }
        }

        private KitBox_Project.Services.DatabaseCalendar.Appointment? selectedAppointment;

        private void DeleteAppointment_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string name)
            {
                var content = button.Content?.ToString();
                var lines = content?.Split('\n');
                if (lines is not { Length: >1 }) return;

                var day = "";
                var hour = "";
                foreach (var child in CalendarGrid.Children)
                {
                    if (child == button)
                    {
                        day = _days[Grid.GetColumn(button) - 1];
                        hour = _hours[Grid.GetRow(button) - 1];
                        break;
                    }
                }

                var appt = DatabaseCalendar.GetAppointment(day, hour);
                if (appt != null)
                {
                    selectedAppointment = appt;
                    SelectedAppointmentInfo.Text = $"Client: {appt.Name_of_customer}\nPhone: {appt.Phone_number}\nSlot: {appt.Day} at {appt.Hour}";
                    CommentBox.Text = appt.Comment ?? "";
                    SelectedAppointmentPanel.IsVisible = true;
                }
            }
        }

        private void ConfirmDeleteAppointment_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedAppointment != null)
            {
                // Delete the appointment from the database
                DatabaseCalendar.DeleteAppointment(selectedAppointment.Day, selectedAppointment.Hour, selectedAppointment.Name_of_customer);
                Console.WriteLine($"Appointment deleted: {selectedAppointment.Day}, {selectedAppointment.Hour}, {selectedAppointment.Name_of_customer}");

                // Update confirmation text
                ConfirmationText.Text = "Appointment canceled.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Gray;

                // Hide selected appointment panel
                SelectedAppointmentPanel.IsVisible = false;

                // Reload appointments to refresh grid
                LoadAppointments();

                // Reset selected object
                selectedAppointment = null;
            }
        }

        private void SaveComment_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedAppointment != null)
            {
                DatabaseCalendar.UpdateComment(selectedAppointment.Day, selectedAppointment.Hour, CommentBox.Text ??"");
                ConfirmationText.Text = "Comment saved.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Green;
            }
        }
    }

    public static class GridHelper
    {
        public static T PlaceInGrid<T>(this T control, int col, int row) where T : Control
        {
            Grid.SetColumn(control, col);
            Grid.SetRow(control, row);
            return control;
        }
    }
}
