using Avalonia.Controls;
using Avalonia.Interactivity;
using KitBox_Project.Services;
using System;
using System.Collections.Generic;
using Avalonia;

namespace KitBox_Project.Views
{
    public partial class WeeklyCalendar : UserControl
    {
        private readonly List<string> _days = new() { "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi" };
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
                ConfirmationText.Text = "Veuillez remplir tous les champs.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Red;
                return;
            }

            var day = DayBox.SelectedItem?.ToString() ?? "";
            var hour = HourBox.SelectedItem?.ToString() ?? "";

            if (DatabaseCalendar.GetAppointment(day, hour) != null)
            {
                ConfirmationText.Text = "Créneau déjà pris.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Red;
                return;
            }

            var name = NameBox.Text;
            var phone = PhoneBox.Text;

            try
            {
                DatabaseCalendar.AddAppointment(day, hour, name, phone);
                ConfirmationText.Text = "Rendez-vous ajouté.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Green;
                LoadAppointments();
            }
            catch (Exception ex)
            {
                ConfirmationText.Text = "Erreur : créneau déjà pris ou invalide.";
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
                    SelectedAppointmentInfo.Text = $"Client: {appt.Name_of_customer}\nTéléphone: {appt.Phone_number}\nCréneau: {appt.Day} à {appt.Hour}";
                    CommentBox.Text = appt.Comment ?? "";
                    SelectedAppointmentPanel.IsVisible = true;
                }
            }
        }

        private void ConfirmDeleteAppointment_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedAppointment != null)
            {
                // Supprimer le rendez-vous de la base de données
                DatabaseCalendar.DeleteAppointment(selectedAppointment.Day, selectedAppointment.Hour, selectedAppointment.Name_of_customer);
                Console.WriteLine($"Rendez-vous supprimé : {selectedAppointment.Day}, {selectedAppointment.Hour}, {selectedAppointment.Name_of_customer}");

                // Mettre à jour le texte de confirmation
                ConfirmationText.Text = "Rendez-vous annulé.";
                ConfirmationText.Foreground = Avalonia.Media.Brushes.Gray;

                // Masquer le panneau de rendez-vous sélectionné
                SelectedAppointmentPanel.IsVisible = false;

                // Recharger les rendez-vous pour mettre à jour la grille
                LoadAppointments();

                // Réinitialiser l'objet sélectionné
                selectedAppointment = null;
            }
        }

        private void SaveComment_Click(object? sender, RoutedEventArgs e)
        {
            if (selectedAppointment != null)
            {
                DatabaseCalendar.UpdateComment(selectedAppointment.Day, selectedAppointment.Hour, CommentBox.Text);
                ConfirmationText.Text = "Commentaire enregistré.";
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
