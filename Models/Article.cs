namespace KitBox_Project.Models
{
    public class Article
    {
        public int PK_num_article { get; set; }
        public string? DescriptionArticle { get; set; }
        public int FK_num_categorie { get; set; }
    }
}