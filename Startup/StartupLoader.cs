using System;
using KitBox_Project.Data;

public class StartupLoader
{
    public static void LoadArticlesFromDatabase()
    {
        ConsoleDataAccess dataAccess = new ConsoleDataAccess();
        StaticArticleDatabase.AllArticles = dataAccess.GetArticles();
        Console.WriteLine($"📦 {StaticArticleDatabase.AllArticles.Count} articles chargés depuis la base.");
    }
}
