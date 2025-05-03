namespace KitBox_Project
{
    public static class AppState
    {
        // MODIFIER : Propriété statique pour stocker la couleur sélectionnée
        public static string SelectedColor { get; set; } = string.Empty;
        public static int SelectedHeight { get; set; } = 0;
        public static int SelectedLength { get; set; } = 0;
        public static int SelectedDepth { get; set; } = 0;

    }
}