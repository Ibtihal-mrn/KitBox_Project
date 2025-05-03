namespace KitBox_Project.Services{
public class AuthenticationService
{
    private static AuthenticationService? _instance;
    public static AuthenticationService Instance => _instance ??= new AuthenticationService();

    public bool IsAuthenticated { get; private set; }
    public string? Username { get; private set; }

    private AuthenticationService() { 

        DatabaseManager.InitializeDatabase();
        DatabaseManager.InsertTestUser();

    }

    

    public bool Login(string username, string password)
    {
        // Vérification dans la base de données
        string? role = DatabaseManager.AuthenticateUser(username, password);

        if (!string.IsNullOrEmpty(role))
        {
            // Si un rôle est retourné, cela signifie que l'utilisateur est authentifié
            IsAuthenticated = true;
            Username = username;
            return true;
        }

        // Si les identifiants sont incorrects
        return false;
    }

    public void Logout()
    {
        IsAuthenticated = false;
        Username = null;
    }

    
}
}