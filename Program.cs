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
        public bool presque_mort { get; set; } = true;
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
            var autres = vivants.Count(j => j.Role != Role.LoupGarou);

            if (loupsGarous == vivants.Count) // Reste que des Loups-Garous
            {
                Console.WriteLine("Les Loups-Garous ont gagné !");
                Thread.Sleep(1500);
                return true;
            }
            else if (autres == vivants.Count) // Reste que les roles du village
            {
                Console.WriteLine("Les Villagois ont gagné !");
                Thread.Sleep(1500);
                return true;
            }
            else if (amoureux.All(a => a.Est_Vivant)  && vivants.Count  == amoureux.Count) // Reste que le couple
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
                var amoureux1 = joueurs.FirstOrDefault(j => j.Nom.Equals(nomAmoureux1, StringComparison.OrdinalIgnoreCase));

                // choisi le deuxieme amoureux
                Console.WriteLine("Deuxieme joueur : ");
                var nomAmoureux2 = Console.ReadLine();
                var amoureux2 = joueurs.FirstOrDefault(j => j.Nom.Equals(nomAmoureux2, StringComparison.OrdinalIgnoreCase));

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
            var victimeNuit = joueurs.FirstOrDefault(j => j.Nom.Equals(nomVicitme, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant && j.presque_mort);
            if (victimeNuit != null)
            {
                victimeNuit.presque_mort = false;
                eliminationNuit.Add(victimeNuit); // Ajouter à la liste des éliminations
            }
        }

        private void role_Sorciere(Joueur Sorciere)
        {
            Console.WriteLine("Sorciere, veut-tu :");
            Console.WriteLine("- sauver le joueur mort (1)");
            Console.WriteLine("- tuer un autre joueur (2)");
            Console.WriteLine("- ne rien faire (3)");
            var choix = Console.Read();

            if (choix == '1' && eliminationNuit.Count != 0 && Sorciere.potion_vie == true)
            {
                Console.WriteLine("D'accord");
                eliminationNuit.Last().presque_mort = true;
                eliminationNuit.RemoveAt(eliminationNuit.Count - 1);
                Sorciere.potion_vie = false;

            }
            else if (choix == '2' && Sorciere.potion_mort == true)
            {
                Console.WriteLine("Qu'elle joueur voulez vous tuer : ");
                var nomVictime = Console.ReadLine();
                var victimeSorciere = joueurs.FirstOrDefault(j => j.Nom.Equals(nomVictime, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant && j.presque_mort);
                if (victimeSorciere != null)
                {
                    victimeSorciere.presque_mort = false;
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
                    Console.WriteLine($"- {joueur.Nom} a ete eliminer cette nuit, il etait {joueur.Role}.");
                    foreach (var amoureuxMort in amoureux.Where(a => a.presque_mort).ToList())
                    {
                        foreach (var partenaire in amoureux.Where(a => a.Est_Vivant))
                        {
                            partenaire.Est_Vivant = false;
                            Console.WriteLine($"- {partenaire.Nom} est mort par chagrin amoureux, il etait {partenaire.Role}.");
                        }
                    }
                    joueur.Est_Vivant = false;
                }
            }
            else
            {
                Console.WriteLine("Aucun elimination cette nuit.");
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
// bug avec cupidon a regarder
// regler bug où on peut pas choisir le joueur a tuer pour la sorciere
// regler bug du vote du village le jour
