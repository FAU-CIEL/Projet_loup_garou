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
