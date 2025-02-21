
// Ce fichier permet d'afficher automatiquement la bonne View lorsqu'un ViewModel est chargé. Il remplace la liaison manuelle des ViewModels aux Views dans certaines situations. Si un View n'est pas trouvée, il affiche un TextBlock avec "No View Found".
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using KitBox_Project.ViewModel;

namespace KitBox_Project
{
    public class ViewLocator : IDataTemplate
    {
        public Control Build(object? data)
        {
            if (data == null) return new TextBlock { Text = "No View Found" };

            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object? data) => data is ViewModelBase;
    }
}
