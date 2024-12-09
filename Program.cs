﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_loup_garou
{
    public enum Role
    {
        Villagois,
        LoupGarou,
        Voyante,
        Sorciere,
        Chasseur,
        Cupidon,
        Petite_Fille,
    }

    public class Joueur
    {
        public string Nom { get; set; }
        public Role Role { get; set; }
        public bool Est_Vivant { get; set; } = true;

        public Joueur(string nom)
        {
            Nom = nom;
        }
    }

    public class Jeu
    {
        private List<Joueur> joueurs;
        private List<Joueur> eliminationNuit = new List<Joueur>();

        public Jeu(List<Joueur> joueurs)
        {
            this.joueurs = joueurs;
        }


        public void Jouer()
        {
            while (true)
            {
                Nuit();
                if (Condition_Fin_Partie()) break;
                Jour();
                if (Condition_Fin_Partie()) break;
            }
        }

        private bool Condition_Fin_Partie()
        {
            var vivants = joueurs.Where(j => j.Est_Vivant).ToList();
            var loupsGarous = vivants.Count(j => j.Role == Role.LoupGarou);
            var autres = vivants.Count(j => j.Role != Role.LoupGarou);

            if (loupsGarous == vivants.Count) // Reste que des Loups-Garous
            {
                Console.WriteLine("Les Loups-Garous ont gagné !");
                Thread.Sleep(5000);
                return true;
            }
            else if (autres == vivants.Count) // Reste que les roles du village
            {
                Console.WriteLine("Les Villagois ont gagné !");
                Thread.Sleep(5000);
                return true;
            }
            else if (vivants.Count == 0) // Tout le monde est mort
            {
                Console.WriteLine("Tout le monde est mort. Egaliter !");
                Thread.Sleep(5000);
                return true;
            }
            return false; // La partie continue
        }

        private void Nuit()
        {
            Console.WriteLine("Cest la nuit.");
            eliminationNuit.Clear(); // Réinitialiser la liste des éliminations

            var voyante = joueurs.FirstOrDefault(j => j.Role == Role.Voyante && j.Est_Vivant);
            if (voyante != null)
                role_Voyante();
        }

        private void Jour()
        {
            Console.WriteLine("C'est le jour. Les joueurs discutent et votent pour éliminer un joueur.");

            // Afficher les joueurs vivants
            Console.WriteLine("Joueurs vivants :");
            foreach (var joueur in joueurs.Where(j => j.Est_Vivant))
            {
                Console.WriteLine($"- {joueur.Nom} ({joueur.Role})");
            }

            //  Voter pour éliminer un joueur
            Console.Write("Choisissez un joueur a eliminer : ");
            var nomJoueurAEliminer = Console.ReadLine();
            var victimeJour = joueurs.FirstOrDefault(j => j.Nom.Equals(nomJoueurAEliminer, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant);
            
            if (victimeJour != null)
            {
                victimeJour.Est_Vivant = false;
                Console.WriteLine($"{victimeJour.Nom} a ete eliminer par le village, il etait {victimeJour.Role}.");
                if (victimeJour.Role == Role.Chasseur)
                    role_Chasseur();
            }
        }


        private void role_Voyante()
        {
            Console.Write("La voyante va choisi un joueur a observer : ");
            var nomInspecte = Console.ReadLine();
            var joueurInspecte = joueurs.FirstOrDefault(j => j.Nom.Equals(nomInspecte, StringComparison.OrdinalIgnoreCase));
            if (joueurInspecte != null && joueurInspecte.Est_Vivant)
                Console.WriteLine($"{voyante.Nom} a inspecte {joueurInspecte.Nom} et a decouvert qu'il est : {joueurInspecte.Role}.");
        }

        private void role_Chasseur()
        {
            Console.Write("Le chasseur peut choisir un joueur a eliminer : ");
            var nomCible = Console.ReadLine();
            var cible = joueurs.FirstOrDefault(j => j.Nom.Equals(nomCible, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant);
            if (cible != null)
            {
                cible.Est_Vivant = false;
                Console.WriteLine($"Le chasseur a elimine {cible.Nom}, il etait {cible.Role}.");
            }
            else
                Console.WriteLine("Le chasseur a decider de ne pas eliminer de joueur");
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            // demande le nombre de joueur
            int nombreJoueurs;
            do
            {
                Console.Write("Nombre de joueurs : ");
                nombreJoueurs = int.Parse(Console.ReadLine());
                if (nombreJoueurs < 5)
                    Console.WriteLine("veuiller choisir plus de personne !");
            } while (nombreJoueurs < 5);

            var noms = new List<string>();
            for (int i = 1; i <= nombreJoueurs; i++)
            {
                Console.Write($"Entrez le pseudo du joueur {i} : ");
                noms.Add(Console.ReadLine());
            }

            var roles = new List<Role>
            {
                Role.Villagois,
                Role.LoupGarou,
                Role.Cupidon,
                Role.Voyante,
                Role.Petite_Fille,
                Role.Sorciere,
                Role.Chasseur,
            };

            // Mélanger les roles
            Random rand = new Random();
            roles = roles.OrderBy(x => rand.Next()).ToList();

            // Créer les joueurs avec des roles aléatoires
            var joueurs = new List<Joueur>();
            for (int i = 0; i < noms.Count; i++)
                joueurs.Add(new Joueur(noms[i]) { Role = roles[i] });

            // Initialiser et démarrer le jeu
            Jeu jeu = new Jeu(joueurs);
            jeu.Jouer();
        }
    }
}