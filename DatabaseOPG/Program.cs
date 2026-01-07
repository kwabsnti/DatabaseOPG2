namespace DatabaseOPG;
using System;
using System.Data.SqlClient;

class Program
{
    static string connectionString =
        "Server=LAPTOP-12Q6A6J0;Database=ImdbDB;Trusted_Connection=True;TrustServerCertificate=True;";

    static void Main()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== IMDb Database Applikation ===");
            Console.WriteLine("1. Søg film");
            Console.WriteLine("2. Tilføj film");
            Console.WriteLine("3. Opdater film");
            Console.WriteLine("4. Slet film");
            Console.WriteLine("0. Afslut");
            Console.Write("\nVælg et nummer: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchMovie();
                    break;
                case "2":
                    AddMovie();
                    break;
                case "3":
                    UpdateMovie();
                    break;
                case "4":
                    DeleteMovie();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Ugyldigt valg.");
                    Pause();
                    break;
            }
        }
    }

    // ---------- READ ----------
    static void SearchMovie()
    {
        Console.Clear();
        Console.WriteLine("=== SØG FILM ===");
        Console.Write("Indtast filmtitel (brug % som wildcard, fx Star%): ");
        string search = Console.ReadLine();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        string sql = @"
            SELECT TitleId, PrimaryTitle
            FROM app.Title
            WHERE PrimaryTitle LIKE @search
            ORDER BY PrimaryTitle";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@search", search);

        using var reader = cmd.ExecuteReader();

        Console.WriteLine("\nResultater:");
        while (reader.Read())
        {
            Console.WriteLine($"{reader["TitleId"]} - {reader["PrimaryTitle"]}");
        }

        Pause();
    }

    // ---------- CREATE ----------
    static void AddMovie()
    {
        Console.Clear();
        Console.WriteLine("=== TILFØJ FILM ===");

        Console.Write("Indtast TitleId (fx tt9999999): ");
        string id = Console.ReadLine();

        Console.Write("Indtast filmtitel: ");
        string title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("TitleId og titel må ikke være tomme.");
            Pause();
            return;
        }

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        string sql = @"
            INSERT INTO app.Title (TitleId, PrimaryTitle, TitleType)
            VALUES (@id, @title, 'movie')";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);

        try
        {
            cmd.ExecuteNonQuery();
            Console.WriteLine("\nFilm er nu tilføjet ✔");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("\nFejl ved indsættelse:");
            Console.WriteLine(ex.Message);
        }

        Pause();
    }

    // ---------- UPDATE ----------
    static void UpdateMovie()
    {
        Console.Clear();
        Console.WriteLine("=== OPDATER FILM ===");

        Console.Write("Indtast TitleId på filmen: ");
        string id = Console.ReadLine();

        Console.Write("Indtast ny titel: ");
        string title = Console.ReadLine();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        string sql = @"
            UPDATE app.Title
            SET PrimaryTitle = @title
            WHERE TitleId = @id";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@title", title);

        int rows = cmd.ExecuteNonQuery();

        if (rows > 0)
            Console.WriteLine("\nFilm opdateret ✔");
        else
            Console.WriteLine("\nIngen film fundet med det TitleId.");

        Pause();
    }

    // ---------- DELETE ----------
    static void DeleteMovie()
    {
        Console.Clear();
        Console.WriteLine("=== SLET FILM ===");

        Console.Write("Indtast TitleId på filmen: ");
        string id = Console.ReadLine();

        using var conn = new SqlConnection(connectionString);
        conn.Open();

        string sql = "DELETE FROM app.Title WHERE TitleId = @id";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        int rows = cmd.ExecuteNonQuery();

        if (rows > 0)
            Console.WriteLine("\nFilm slettet ✔");
        else
            Console.WriteLine("\nIngen film fundet med det TitleId.");

        Pause();
    }

    // ---------- PAUSE ----------
    static void Pause()
    {
        Console.WriteLine("\nTryk på en tast for at fortsætte...");
        Console.ReadKey();
    }
}


