// // Ce fichier sert de base commune pour tous les ViewModels. C'est une classe intermédiaire que je définis moi-même pour structurer le projet, ce qui est une bonne pratiquelorsqu'il y en a plusieurs. Cela permet d'ajouter des fonctionnalités communes et éviter la duplication de code.On peut le voir ça comme une couche d'organisation pour centraliser certaines logiques.
// Au départ, j’ai utilisé ReactiveObject, adapté aux petits projets. Cependant, pour mieux structurer l’application, j’ai créé ViewModelBase, qui en hérite.

using ReactiveUI;

namespace KitBox_Project.ViewModel
{
    public class ViewModelBase : ReactiveObject
    {
    }
}
