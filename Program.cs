using Projet_loup_garou;
using System;
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
        public bool presque_mort { get; set; } = false;
        public bool Est_Vivant { get; set; } = true;
        public bool Est_amoureux { get; set; } = false;
        public bool potion_vie { get; set; } = false;
        public bool potion_mort { get; set; } = false;

        public Joueur(string nom)
        {
            this.Nom = nom;
        }
    }

    public class Jeu
    {
        private List<Joueur> joueurs;
        private List<Joueur> amoureux = new List<Joueur>();
        private List<Joueur> eliminationNuit = new List<Joueur>();

        public Jeu(List<Joueur> joueurs)
        {
            this.joueurs = joueurs;
        }


        public void Jouer()
        {
            role_Cupidon();
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

            if (loupsGarous == vivants.Count) // Reste que des Loups-Garous
            {
                Console.WriteLine("Les Loups-Garous ont gagné !");
                Thread.Sleep(1500);
                return true;
            }
            else if (loupsGarous == 0) // Reste que les roles du village
            {
                Console.WriteLine("Les Villagois ont gagné !");
                Thread.Sleep(1500);
                return true;
            }
            else if (amoureux.All(a => a.Est_Vivant) && vivants.Count == amoureux.Count) // Reste que le couple
            {
                Console.WriteLine("Les amoureux ont gagné !");
                Thread.Sleep(1500);
                return true;
            }
            else if (vivants.Count == 0) // Tout le monde est mort
            {
                Console.WriteLine("Tout le monde est mort. Egaliter !");
                Thread.Sleep(1500);
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

            var loupGarou = joueurs.FirstOrDefault(j => j.Role == Role.LoupGarou && j.Est_Vivant);
            if (loupGarou != null)
                role_loup_garou();

            var sorciere = joueurs.FirstOrDefault(j => j.Role == Role.Sorciere && j.Est_Vivant);
            if (sorciere != null)
                role_Sorciere(sorciere);

            affichageMort();
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
            if (victimeJour == null)
            {
                Console.WriteLine("Personne n'a ete designer par le village.");
            }
        }

        private void role_Cupidon()
        {
            var cupidon = joueurs.FirstOrDefault(j => j.Role == Role.Cupidon && j.Est_Vivant);
            if (cupidon != null)
            {
                Console.WriteLine("Cupidon va choisir deux joueurs a mettre en couple.");

                // Choisi le premier amoureux
                Console.WriteLine("Premier joueur : ");
                var nomAmoureux1 = Console.ReadLine();
                var amoureux1 = joueurs.FirstOrDefault(j => j.Nom.Equals(nomAmoureux1, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant);

                // choisi le deuxieme amoureux
                Console.WriteLine("Deuxieme joueur : ");
                var nomAmoureux2 = Console.ReadLine();
                var amoureux2 = joueurs.FirstOrDefault(j => j.Nom.Equals(nomAmoureux2, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant);

                if (amoureux1 != null && amoureux2 != null && amoureux1 != amoureux2) // vérifie que les deux amoureux sont differents et "présent"
                {
                    amoureux1.Est_amoureux = true;
                    amoureux2.Est_amoureux = true;
                    amoureux.Add(amoureux1);
                    amoureux.Add(amoureux2);
                    Console.WriteLine($"{amoureux1.Nom} et {amoureux2.Nom} sont mantenant amoureux.");
                }
            }
        }

        private void role_Voyante()
        {
            Console.Write("La voyante va choisir un joueur a observer : ");
            var nomInspecte = Console.ReadLine();
            var joueurInspecte = joueurs.FirstOrDefault(j => j.Nom.Equals(nomInspecte, StringComparison.OrdinalIgnoreCase));
            if (joueurInspecte != null && joueurInspecte.Est_Vivant)
                Console.WriteLine($"La voyante a inspecte {joueurInspecte.Nom} et a decouvert qu'il est : {joueurInspecte.Role}.");
        }

        private void role_loup_garou()
        {
            Console.WriteLine("Les Loups-Garous vont choisir un joueur a eliminer : ");
            var nomVicitme = Console.ReadLine();
            var victimeNuit = joueurs.FirstOrDefault(j => j.Nom.Equals(nomVicitme, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant);
            if (victimeNuit != null)
            {
                victimeNuit.presque_mort = true;
                eliminationNuit.Add(victimeNuit); // Ajouter à la liste des éliminations
            }
        }

        private void role_Sorciere(Joueur Sorciere)
        {
            Console.WriteLine("Sorciere, veut-tu :");
            Console.WriteLine("- sauver le joueur mort (1)");
            Console.WriteLine("- tuer un autre joueur (2)");
            Console.WriteLine("- ne rien faire (3)");
            var choix = Console.ReadLine();

            if (choix == "1" && eliminationNuit.Count != 0 && Sorciere.potion_vie)
            {
                Console.WriteLine("D'accord");
                eliminationNuit.Last().presque_mort = false;
                eliminationNuit.RemoveAt(eliminationNuit.Count - 1);
                Sorciere.potion_vie = false;

            }
            else if (choix == "2" && Sorciere.potion_mort)
            {
                Console.WriteLine("Qu'elle joueur voulez vous tuer : ");
                var nomVictime = Console.ReadLine();
                var victimeSorciere = joueurs.FirstOrDefault(j => j.Nom.Equals(nomVictime, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant && j.presque_mort == false);
                if (victimeSorciere != null)
                {
                    victimeSorciere.presque_mort = true;
                    eliminationNuit.Add(victimeSorciere);
                }
                Sorciere.potion_mort = false;
            }
            else
            {
                Console.WriteLine("D'accord");
            }
        }

        private void role_Chasseur()
        {
            Console.Write("Le chasseur peut choisir un joueur a eliminer : ");
            var nomCible = Console.ReadLine();
            var cible = joueurs.FirstOrDefault(j => j.Nom.Equals(nomCible, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant && j.presque_mort);
            if (cible != null)
            {
                cible.Est_Vivant = false;
                Console.WriteLine($"Le chasseur a elimine {cible.Nom}, il etait {cible.Role}.");
            }
            else
                Console.WriteLine("Le chasseur n'a pas designer de cible");
        }

        private void affichageMort()
        {
            if (eliminationNuit.Any())
            {
                foreach (var joueur in eliminationNuit)
                {
                    joueur.Est_Vivant = false; // Marque le joueur comme mort
                    Console.WriteLine($"- {joueur.Nom} a ete elimine cette nuit, il etait {joueur.Role}.");
                    // verifier si le joueur mort fait parti du couple
                    if (joueur.Est_amoureux)
                    {
                        // Trouve le partenaire amoureux
                        var partenaire = joueurs.FirstOrDefault(j => j.Est_amoureux && j.Est_Vivant && j != joueur);
                        if (partenaire != null)
                        {
                            partenaire.Est_Vivant = false; // Marque le partenaire comme mort
                            Console.WriteLine($"- {partenaire.Nom} est mort par chagrin amoureux, il etait {partenaire.Role}.");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Aucune personne n'a ete eliminee cette nuit.");
            }
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
                    Console.WriteLine("veuillez choisir plus de personne !");
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

            // Calcule le nombre de villagois et de loup-garous
            int nombreLoupGarous = Math.Max(1, nombreJoueurs / 4); // 4 villagois = 1 loup
            int nombreVillagois = nombreJoueurs - nombreLoupGarous;

            // ajoute role loup-garou
            for (int i = 0; i<nombreLoupGarous; i++)
                roles.Add(Role.LoupGarou);
            
            // ajoute role villagois
            for (int i = 0; i<nombreVillagois; i++)
                roles.Add(Role.Villagois);
            /*
            // Ajoute les roles de villagois et de loup-garou suplementaire si le nombre de joueur est superieur au nombre de roles
            if (nombreJoueurs > roles.Count)
            {
                int rolesManquants = nombreJoueurs - roles.Count;
                Random roleAleatoire = new Random();

                for (int i = 0; i < rolesManquants; i++)
                {
                    if (roleAleatoire.Next(2) == 0) // 0 pour villagois et 1 pour loup garou
                    {
                        roles.Add(Role.Villagois);
                    }
                    else
                    {
                        roles.Add(Role.LoupGarou);
                    }
                }
            }*/

            // Mélanger les roles
            Random rand = new Random();
            roles = roles.OrderBy(x => rand.Next()).ToList();

            // Créer les joueurs sans les roles
            var joueurs = new List<Joueur>();
            for (int i = 0; i < noms.Count; i++)
                joueurs.Add(new Joueur(noms[i]));

            // Assigne les Roles aleatoirement
            for (int i = 0; i < joueurs.Count; i++)
            {
                joueurs[i].Role = roles[i];

                // si sorciere, alors potion mort et vit deviennent vrai
                if (joueurs[i].Role == Role.Sorciere)
                {
                    joueurs[i].potion_vie = true;
                    joueurs[i].potion_mort = true;
                }
            }

            // Initialiser et démarrer le jeu
            Jeu jeu = new Jeu(joueurs);
            jeu.Jouer();
        }
    }
}