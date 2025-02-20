public class Jeu
{
    private List<Joueur> joueurs;
    private List<Joueur> amoureux = new List<Joueur>();
    private List<Joueur> eliminationNuit = new List<Joueur>();
    private Chat chat = new Chat();

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

    private void Nuit()
    {
        Console.WriteLine("C'est la nuit.");
        // Afficher le chat des Loups-Garous
        var loupsGarous = joueurs.Where(j => j.Role == Role.LoupGarou && j.Est_Vivant).ToList();
        if (loupsGarous.Any())
        {
            Console.WriteLine("Les Loups-Garous, discutez entre vous :");
            string message;
            while ((message = Console.ReadLine()) != null)
            {
                chat.EnvoyerMessageLoup("Loup", message); // Remplace "Loup" par le nom du joueur
                chat.AfficherMessagesLoups();
            }
        }

        // Autres actions de nuit...
    }

    private void Jour()
    {
        Console.WriteLine("C'est le jour. Les joueurs discutent et votent pour éliminer un joueur.");
        chat.AfficherMessagesPublics(); // Afficher le chat public

        // Afficher les joueurs vivants
        Console.WriteLine("Joueurs vivants :");
        foreach (var joueur in joueurs.Where(j => j.Est_Vivant))
        {
            Console.WriteLine($"- {joueur.Nom} ({joueur.Role})");
        }

        // Chat public
        Console.WriteLine("Entrez votre message (ou tapez 'exit' pour quitter) :");
        string messagePublic;
        while ((messagePublic = Console.ReadLine()) != null && messagePublic != "exit")
        {
            chat.EnvoyerMessagePublic("Joueur", messagePublic); // Remplace "Joueur" par le nom du joueur
            chat.AfficherMessagesPublics();
        }

        // Voter pour éliminer un joueur...
    }

    private void role_Cupidon()
    {
        var cupidon = joueurs.FirstOrDefault(j => j.Role == Role.Cupidon && j.Est_Vivant);
        if (cupidon != null)
        {
            Console.WriteLine("Cupidon va choisir


//

using Projet_loup_garou;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Projet_loup_garou
{
    public class Chat
    {
        private List<string> messages = new List<string>();
        private bool isChatActive = true;

        public void StartChat()
        {
            Thread chatThread = new Thread(() =>
            {
                while (isChatActive)
                {
                    string message = Console.ReadLine();
                    if (message.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        isChatActive = false;
                    }
                    else
                    {
                        messages.Add(message);
                        Console.WriteLine($"[Chat] {message}");
                    }
                }
            });
            chatThread.Start();
        }

        public void StopChat()
        {
            isChatActive = false;
        }
    }

    // ... (le reste de votre code)
}

private void Jour()
{
    Console.WriteLine("C'est le jour. Les joueurs discutent et votent pour éliminer un joueur.");

    // Démarrer le chat
    Chat chat = new Chat();
    chat.StartChat();

    // Afficher les joueurs vivants
    Console.WriteLine("Joueurs vivants :");
    foreach (var joueur in joueurs.Where(j => j.Est_Vivant))
        Console.WriteLine($"- {joueur.Nom} ({joueur.Role})");

    //  Voter pour éliminer un joueur
    Console.Write("Choisissez un joueur a eliminer : ");
    var nomJoueurAEliminer = Console.ReadLine();
    var victimeJour = joueurs.FirstOrDefault(j => j.Nom.Equals(nomJoueurAEliminer, StringComparison.OrdinalIgnoreCase) && j.Est_Vivant);

    if (victimeJour != null)
    {
        eliminationNuit.Add(victimeJour);
        if (victimeJour.Role == Role.Chasseur)
            role_Chasseur();
        affichageMort();
    }
    if (victimeJour == null)
        Console.WriteLine("Personne n'a été designé par le village.");

    // Arrêter le chat à la fin de la phase de jour
    chat.StopChat();
}
