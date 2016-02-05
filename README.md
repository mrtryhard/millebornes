# millebornes
Mille Bornes - School project / Projet Cégep  (C#, WPF, WCF, EF)
(English below)

Cette version de Mille Bornes est conçue pour jouer en réseau. Dans le cadre du cours de Composants et services web, nous avions l'obligation d'utiliser 
un service WCF et d'avoir au moins deux services, expliquant certaines pratiques.  

## Fonctionnalitée  
Le jeu supporte une salle de discussion générale pour que tous les joueurs hors parties puissent discuter. Les joueurs étant dans une partie possèdent un nom rouge, les joueurs disponibles un nom vert.  
  
Il y a également une salle de discussion par salle n'empêchant pas la salle de discussion générale.  
  
Afin de pousser un peu plus loin le salon, le jeu dispose également d'un système de messagerie privé, 1j à 1j, avec notifications. 
  
Bien entendu, durant la partie, il y a une salle de discussion dédiée.  
  
## Dépendance
Le jeu a été conçu avec **WCF**, **WPF** et **MS SQL** comme technologies, utilisant Entity Framework. Les paquets NuGet devraient permettre une génération sans problème.
  
## Démarrer le jeu
Pour démarrer le jeu, il faut suivre ces étapes:
* Regénérer la solution (par défaut la base de données sera effacée)
* Lancer une instance du projet *LibrairieService* 
* Lancer l'instance du projet Lobby. Il est normal que la génération de l'exécutable échoue. Utilisez la version précédente.
* Créez un nouvel utilisateur
* Connectez-vous

## Notes 
Le démarrage du projet Lobby peut prendre un certain moment.  
Dû à l'abus des fonctionnalités **async** et **await**, avoir beaucoup de salles de discussions privées peuvent causer un ralentissement dû au rafraîchissement de l'interface usager.

# Mille Bornes (EN)
This Mille Bornes version was built to play on network. It is a school project for a specific class and we therefore had to use two WCF services which probably explains some practices in there.

## Functionnality  
The game supports a general chat room in the lobby so all connected players who are not in-game can chat. Players being red are in-game and won't see the messages while orange players are in room and may see and respond. Finally, green players are fully available.
  
There is also a Room Chat available when the player is in a room.
  
Just so we could push it further, the lobby allows players to communicate through private messages just like MSN, with notifications. The private messages are limited to 2p groups (you and another). 
  
Of course, there's also a dedicated chat functionnality for matches.
  
## Pre-requisite
The game has been developped with **WCF**, **WPF** and **MS SQL** technologies, using **Entity Framework** as well. The NuGet packages should be downloaded and built properly on the first full build.
  
## Start the game
To setup everything properly, please do:
* Rebuild complete solution 
* Launch a single instance of LibrairieService
* Launch Lobby instance. If the building process fail -it's okay-, just use the "previous build". Just make sure Librairie and Lobby has been built together at least once before that.
* "Inscription" means to register a new player, do that. In order: username, pass, email.
* Then connect

## Notes 
The Lobby start up might take a while.  
Because of an awesome **async** & **await** abuse, having too much chat room opened at the same time might cause a big lag since it refreshes the UI. Sorry about that.

