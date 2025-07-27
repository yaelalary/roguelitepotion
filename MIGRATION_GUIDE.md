# Guide de Migration UI Canvas vers Sprites 2D

## Étapes à suivre dans Unity :

### 1. Installation de TextMeshPro
- Aller dans Window > TextMeshPro > Import TMP Essential Resources
- Si vous n'avez pas TextMeshPro, l'installer via Package Manager

### 2. Création du prefab de carte 2D
- Créer un GameObject vide dans votre scène
- Ajouter le script `CardBuilder` 
- Dans l'Inspector, cliquer sur "Create Ingredient Card Prefab"
- Cela créera automatiquement le prefab dans Assets/Prefabs/

### 3. Configuration de la scène
- Supprimer l'ancien Canvas UI s'il existe
- Créer un GameObject vide nommé "CardManager"
- Ajouter le script `IngredientCardManager2D`
- Assigner la référence au `DeckManager`
- Assigner le prefab `IngredientCard2D` créé précédemment

### 4. Configuration de la caméra
- Changer votre caméra en mode "Orthographic" si ce n'est pas déjà fait
- Ajuster la taille (Orthographic Size) pour voir toutes vos cartes (recommandé: 5-8)

### 5. Suppression des anciens scripts
- Vous pouvez maintenant supprimer ou désactiver :
  - `IngredientUI.cs`
  - `IngredientUIManager.cs`

## Nouvelles fonctionnalités ajoutées :

### Interactions améliorées :
- **Hover**: Les cartes grandissent légèrement au survol
- **Sélection**: Les cartes changent de couleur et remontent
- **Multi-sélection**: Jusqu'à 3 ingrédients peuvent être sélectionnés

### Contrôles clavier :
- **Espace**: Créer une potion avec les ingrédients sélectionnés
- **R**: Tirer une nouvelle main

### Système de cartes :
- Position automatique des cartes en ligne
- Repositionnement automatique après utilisation d'ingrédients
- Remplissage automatique de la main

## Paramètres ajustables :

Dans `IngredientCardManager2D` :
- `numberToDraw`: Nombre de cartes dans la main (défaut: 5)
- `cardSpacing`: Espacement entre les cartes (défaut: 2f)
- `handStartPosition`: Position de départ de la main (défaut: -4, -3, 0)
- `maxSelection`: Nombre maximum de cartes sélectionnables (défaut: 3)

Dans `IngredientCard2D` :
- `selectionOffset`: Hauteur de remontée des cartes sélectionnées
- `hoverScale`: Facteur d'agrandissement au survol
- `normalColor` et `selectedColor`: Couleurs de la carte

## Notes importantes :

1. **Layer Order**: Les sprites utilisent des sortingOrder pour l'affichage
2. **Colliders**: BoxCollider2D requis pour les interactions souris
3. **TextMeshPro**: Utilisé pour un meilleur rendu de texte
4. **Performance**: Plus efficace que l'UI Canvas pour les jeux 2D

## Prochaines améliorations possibles :

- Animations de transition (DOTween recommandé)
- Effets visuels pour les combinaisons
- Système de score basé sur les recettes
- Drag & drop pour réorganiser les cartes
