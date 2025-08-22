# Blast Royale — Monster Battle

> **Jeu mobile multijoueur (Unity + Nakama)**

**Résumé**

Blast Royale — *Monster Battle* est un jeu mobile multijoueur développé en client Unity (C#) et en backend TypeScript (Nakama). Le projet vise un jeu de stratégie tour par tour où les joueurs collectionnent, entraînent et affrontent des créatures appelées *Blast*.

---

## Table des matières

1. [Fonctionnalités](#fonctionnalit%C3%A9s)
2. [Stack technique](#stack-technique)
3. [Dépôts GitHub](#d%C3%A9p%C3%B4ts-github)
4. [Prérequis](#pr%C3%A9requis)
5. [Installation & exécution locale](#installation--ex%C3%A9cution-locale)

   * Client Unity
   * Serveur Nakama (Docker)
6. [Build Android (CI/CD)](#build-android-cicd)
7. [Tests](#tests)
8. [Architecture & organisation du code](#architecture--organisation-du-code)
9. [Sécurité & bonnes pratiques](#s%C3%A9curit%C3%A9--bonnes-pratiques)
10. [Accessibilité](#accessibilit%C3%A9)
11. [Suivi des bugs & support](#suivi-des-bugs--support)
12. [Contribution](#contribution)
13. [Roadmap & journal de versions](#roadmap--journal-de-versions)
14. [Licence & crédits](#licence--cr%C3%A9dits)

---

## Fonctionnalités

* Jeu multijoueur 1v1 en tour par tour (JcJ) et joueur vs IA (JcE)
* Système de capture et d'entraînement de créatures (*42 Blast*)
* Système d'attaques (54 attaques) et objets (9 objets)
* Boutiques (journalière, permanente, premium)
* Wiki intégré (Blast, attaques, objets, zones)
* Classements globaux et par zone
* Récompenses journalières, quêtes et systèmes de progression

---

## Stack technique

* **Client :** Unity 2022.3.54 LTS (C#)
* **Serveur :** Nakama (modules TypeScript)
* **Base de données :** PostgreSQL
* **Conteneurisation & déploiement :** Docker + docker-compose
* **Hébergement :** DigitalOcean (droplets)
* **CI/CD :** GitHub Actions (build Android, tests Unity)
* **Sérialisation :** Newtonsoft.Json

---

## Dépôts GitHub

* Client Unity : `https://github.com/ChronoBlasted/Blast-Royale`
* Serveur Nakama : `https://github.com/ChronoBlasted/BlastRoyaleServer`

> Ces liens pointent aux répertoires principaux du projet (branches `dev` utilisées pour le développement).

---

## Prérequis

* **Pour le client :**

  * Unity 2022.3.54 LTS
  * Unity Hub
  * Visual Studio (ou autre IDE C# compatible)
* **Pour le serveur :**

  * Docker & docker-compose
  * Node.js + npm (si vous développez / testez des modules TypeScript Nakama)
  * PostgreSQL (conteneurisé via Docker dans la plupart des cas)
* **Pour le build Android :**

  * JDK (version compatible avec Unity)
  * Keystore pour signature
* **Accès serveur :** clé SSH (pour se connecter aux droplets DigitalOcean)

---

## Installation & exécution locale

### Client Unity (développement / tests)

1. Ouvrir Unity Hub et importer le projet `Blast-Royale`.
2. Installer les packages nécessaires (Packages/Plugins mentionnés dans le dépôt).
3. Ouvrir la scène de démarrage (ex: `Assets/Scenes/...`).
4. Dans l'éditeur, configurer l'IP publique du serveur Nakama pour pointer vers votre instance de test.
5. Lancer le jeu via l'éditeur pour tests locaux.

### Serveur Nakama (exécution avec Docker)

1. Cloner le dépôt serveur :

```bash
git clone https://github.com/ChronoBlasted/BlastRoyaleServer.git
cd BlastRoyaleServer/Nakama
```

2. Démarrer Nakama via `docker-compose` :

```bash
docker-compose up --build nakama
```

3. Vérifier les logs Docker et la console Nakama pour confirmer que le service est en ligne.

> **Conseil:** déployer d'abord un environnement de test (droplet DigitalOcean) pour valider les correctifs avant promotion en production.

---

## Build Android (CI/CD)

Le projet intègre un workflow GitHub Actions (`.github/workflows/build-project.yml`) qui :

* lance les tests unitaires Unity au push ;
* build automatiquement l'APK Android ;
* signe l'APK si le keystore est configuré.

Pour configurer le build automatique :

1. Ajouter les secrets nécessaires sur GitHub (keystore, clés, variables d'environnement).
2. Vérifier le runner et les versions Unity utilisés par le workflow.

---

## Tests

* **Unity Test Framework** est utilisé pour les tests unitaires côté client (logique de combat, consommation de mana, résolution de tours, etc.).
* Couverture estimée pour la logique de combat : \~80 %.
* Les tests s'exécutent automatiquement via GitHub Actions à chaque push sur la branche `main`.

---

## Architecture & organisation du code

### Client (structure projet)

* `Assets/`

  * `Animations/`, `Audio/`, `Assets/`, `Fonts/`, `Sprites/`, `Video/`
  * `Editor/` (scripts personnalisés)
  * `Packages/`, `Plugins/`, `Prefabs/`, `Materials/`, `Scenes/`, `Settings/`, `Scripts/`, `ScriptableObjects/`

### Serveur (structure projet)

* `src/` : modules Nakama (RPC, logiques de match, gestion des récompenses, verrous, etc.)
* `docker-compose.yml` : orchestration des services (nakama, postgres, ...)

---

## Sécurité & bonnes pratiques

* Authentification : tokens JWT, aucun mot de passe stocké côté client.
* Requêtes sécurisées : validation côté serveur, utilisation de requêtes préparées.
* Communication chiffrée (HTTPS) entre client et serveur.
* Vérifications côté serveur pour éviter les actions clients falsifiées (ex: contrôle du mana avant exécution d'une attaque).
* Politique de mise à jour des dépendances : veille hebdomadaire pour Nakama/Unity, mensuelle pour librairies annexes.

---

## Accessibilité

Le jeu suit des recommandations adaptées (inspirées du RGAA 4.1) :

* Contraste de couleurs optimisé
* Taille minimale de texte (21px)
* UI responsive via Canvas Scaler et anchors
* Feedback visuel et sonore clair pour actions

---

## Suivi des bugs & support

* Canal communautaire Discord `#bug-report` pour remontées joueurs (format structuré demandé : title, description, how to reproduce, priority).
* Les tickets validés sont transformés en issues GitHub pour suivi technique.
* Exemple de workflow : signalement → qualification → issue GitHub → correctif → tests → déploiement.

---

## Contribution

Vous souhaitez contribuer ? Merci — voici comment :

1. Forkez le dépôt principal.
2. Créez une branche `feat/ma-fonctionnalite` ou `fix/mon-correctif`.
3. Respectez la convention de commit (Conventional Commits).
4. Ouvrez une Pull Request vers `dev`.
5. Les PR doivent inclure des tests unitaires si pertinent et une description claire.

> Pour les rapports de bugs, utilisez d'abord le canal Discord pour recueillir des steps-to-reproduce avant d'ouvrir une issue.

---

## Roadmap & journal de versions

* Versionning : Git Flow (branches `main`, `dev`, `feat/*`, `fix/*`) + tags (ex : `v0.1.0`, `v0.1.2`, ...).
* Exemples de versions passées :

  * `v0.0.0` — Base serveur Nakama (03/01/2025)
  * `v0.1.0` — Navigation & combats simples (15/04/2025)
  * `v0.1.2` — Gestion du mana, animations (21/05/2025)
  * `v0.1.4` — Quêtes, récompenses, sécurité renforcée (30/06/2025)

---

## Licence & crédits

* **Licence :** (à définir — ex : MIT, Apache-2.0). Merci d'indiquer la licence choisie et d'ajouter un fichier `LICENSE`.
* **Crédits :** Alexis Gelin — auteur du livrable et contributeur principal.

---

## Contact

Pour toute question ou support technique :

* Discord : canal `#bug-report` (voir README du repo pour le lien)
* Issues GitHub : ouvrez une issue dans le dépôt approprié
