using System;
using Microsoft.Data.SqlClient;

namespace DatabaseOPG
{
    class Program
    {
        //  Connection string (Windows) 
        static string connectionString =
            "Server=LAPTOP-12Q6A6J0;Database=ImdbDB;Trusted_Connection=True;TrustServerCertificate=True;";

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("\n=== DATABASE MENU ===");
                Console.WriteLine("1. Søg film");
                Console.WriteLine("2. Tilføj film");
                Console.WriteLine("3. Slet film");
                Console.WriteLine("0. Afslut");
                Console.Write("Vælg: ");

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
                        DeleteMovie();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Ugyldigt valg");
                        break;
                }
            }
        }

        //  SØG FILM (Wildcard)
        static void SearchMovie()
        {
            Console.Write("Skriv filmnavn (brug %): ");
            string search = Console.ReadLine();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"
                SELECT TitleId, PrimaryTitle, StartYear
                FROM app.Title
                WHERE PrimaryTitle LIKE @search
                ORDER BY PrimaryTitle";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@search", search);

            using SqlDataReader reader = cmd.ExecuteReader();

            Console.WriteLine("\nResultater:");
            while (reader.Read())
            {
                Console.WriteLine(
                    reader["TitleId"] + " | " +
                    reader["PrimaryTitle"] + " | " +
                    reader["StartYear"]);
            }
        }

        //  TILFØJ FILM (KORREKT – INGEN NULL)
        static void AddMovie()
        {
            Console.Write("TitleId (fx tt9999999): ");
            string id = Console.ReadLine();

            Console.Write("Titel: ");
            string title = Console.ReadLine();

            Console.Write("År: ");
            int year = int.Parse(Console.ReadLine());

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = @"
                INSERT INTO app.Title
                (TitleId, TitleType, PrimaryTitle, StartYear, IsAdult)
                VALUES
                (@id, 'movie', @title, @year, 0)";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@title", title);
            cmd.Parameters.AddWithValue("@year", year);

            cmd.ExecuteNonQuery();
            Console.WriteLine("✅ Film tilføjet");
        }

        //  SLET FILMe
        static void DeleteMovie()
        {
            Console.Write("Skriv TitleId der skal slettes: ");
            string id = Console.ReadLine();

            using SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();

            string sql = "DELETE FROM app.Title WHERE TitleId = @id";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "✅ Film slettet" : "❌ Ikke fundet");
        }
    }
}




