using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;
using Avalonia;

namespace KitBox_Project.Views
{
    public partial class WeeklyCalendar : UserControl
    {
        // Dictionnaire pour stocker les rendez-vous par jour
        private Dictionary<string, List<string>> appointmentsByDay = new();

        public WeeklyCalendar()
        {
            InitializeComponent();
        }

        private void OnAddAppointment(object? sender, RoutedEventArgs e)
        {
            var selectedDay = (DaySelector.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var selectedHour = (HourSelector.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var appointmentName = AppointmentTextBox.Text;

            if (string.IsNullOrEmpty(selectedDay) || string.IsNullOrEmpty(selectedHour) || string.IsNullOrWhiteSpace(appointmentName))
            {
                return; // On ne fait rien si les champs ne sont pas tous remplis
            }

            var fullAppointment = $"{selectedHour} - {appointmentName}";

            // Ajoute à la liste dans le dictionnaire
            if (!appointmentsByDay.ContainsKey(selectedDay))
                appointmentsByDay[selectedDay] = new List<string>();

            appointmentsByDay[selectedDay].Add(fullAppointment);

            AppointmentTextBox.Text = "";
            RefreshAppointmentsList(selectedDay);
        }

        private void RefreshAppointmentsList(string selectedDay)
        {
            AppointmentsList.ItemsSource = appointmentsByDay.ContainsKey(selectedDay)
                ? appointmentsByDay[selectedDay]
                : new List<string>();
        }

        // Optionnel : si tu veux rafraîchir automatiquement quand on change de jour
        private void OnDayChanged(object? sender, SelectionChangedEventArgs e)
        {
            var selectedDay = (DaySelector.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (!string.IsNullOrEmpty(selectedDay))
                RefreshAppointmentsList(selectedDay);
        }
    }
}
