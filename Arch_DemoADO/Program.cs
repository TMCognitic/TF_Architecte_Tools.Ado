using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Tools.Ado;

namespace Arch_DemoADO
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Connection connection = new Connection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DemoAdo;Integrated Security=True");
            Command insert = new Command("INSERT INTO Produit (Nom, Prix) OUTPUT INSERTED.Id VALUES (@Nom, @Prix);", false);
            insert.AddParameter("Nom", "Eau 1l");
            insert.AddParameter("Prix", 1.1);

            int id = (int)connection.ExecuteScalar(insert);
            Console.WriteLine(id);


            //using (SqlConnection connection = new SqlConnection())
            //{
            //    connection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DemoAdo;Integrated Security=True";

            //    using (SqlCommand sqlCommand = connection.CreateCommand())
            //    {
            //        sqlCommand.CommandText = "INSERT INTO Produit (Nom, Prix, Descriptif) OUTPUT INSERTED.Id VALUES (@Nom, @Prix, @Descriptif);";
            //        sqlCommand.Parameters.AddWithValue("Nom", "Eau 33cl");
            //        sqlCommand.Parameters.AddWithValue("Prix", .70);
            //        sqlCommand.Parameters.AddWithValue("Descriptif", DBNull.Value);
            //        connection.Open();

            //        int id = (int)sqlCommand.ExecuteScalar();
            //        Console.WriteLine(id);
            //    }
            //}

            Command selectNom = new Command("SELECT Nom FROM Produit WHERE Id = 1;", false);
            string nom = (string)connection.ExecuteScalar(selectNom);

            Console.WriteLine(nom);

            //using (SqlConnection connection = new SqlConnection())
            //{
            //    connection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DemoAdo;Integrated Security=True";

            //    List<Produit> list = new List<Produit>();

            //    using (SqlCommand sqlCommand = connection.CreateCommand())
            //    {
            //        sqlCommand.CommandText = "SELECT Nom FROM Produit WHERE Id = 1;";
            //        connection.Open();

            //        object? result = sqlCommand.ExecuteScalar();

            //        Console.WriteLine(result);
            //    }
            //}

            Command selectAll = new Command("SELECT * FROM Produit;", false);

            IEnumerable<Produit> produits = connection.ExecuteReader(selectAll, (dr) => new Produit()
            {
                Id = (int)dr["Id"],
                Nom = (string)dr["Nom"],
                Prix = (double)dr["Prix"],
                Creation = (DateTime)dr["Creation"],
                Descriptif = dr["Descriptif"] as string
            });

            foreach (Produit produit in produits)
            {
                Write(produit);
            }

            //using (SqlConnection connection = new SqlConnection())
            //{
            //    connection.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=DemoAdo;Integrated Security=True";

            //    List<Produit> list = new List<Produit>();

            //    using (SqlCommand sqlCommand = connection.CreateCommand())
            //    {
            //        sqlCommand.CommandText = "SELECT * FROM Produit;";
            //        connection.Open();

            //        using (SqlDataReader reader = sqlCommand.ExecuteReader())
            //        {
            //            while (reader.Read())
            //            {
            //                list.Add(new Produit()
            //                {
            //                    Id = (int)reader["Id"],
            //                    Nom = (string)reader["Nom"],
            //                    Prix = (double)reader["Prix"],
            //                    Creation = (DateTime)reader["Creation"],
            //                    Descriptif = reader["Descriptif"] as string
            //                });
            //            }
            //        }
            //    }

            //    foreach (Produit produit in list)
            //    {
            //        Write(produit);
            //    }
            //}
        }

        static void Write(object obj)
        {
            IProduit? produit = obj as IProduit;

            if(produit is not null)
            {
                Console.WriteLine($"{produit.Id} {produit.Nom}");
            }
            else
            {
                Console.WriteLine(obj.GetType().Name);
            }
        }
    }



    interface IProduit
    {
        int Id { get; set; }
        string? Nom { get; set; }
        double Prix { get; set; }
        string? Descriptif { get; set; }
        DateTime Creation { get; set; }
    }

    class Produit : IProduit
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public double Prix { get; set; }
        public string? Descriptif { get; set; }
        public DateTime Creation { get; set; }

    }
}