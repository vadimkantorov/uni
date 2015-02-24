using System.Configuration;
using System.Data.SqlClient;

namespace Insurance.Controllers
{
	public class ClientService
	{
		static bool? Crashed(InsuranceInfo info, SqlConnection conn)
		{
			using(var cmd = conn.CreateCommand())
			{
				cmd.CommandText = "SELECT Crashed FROM Clients WHERE FirstName = @fn AND LastName = @ln AND MiddleName = @mn";
				cmd.Parameters.AddWithValue("@fn", info.PersonalDetails.FirstName);
				cmd.Parameters.AddWithValue("@ln", info.PersonalDetails.LastName);
				cmd.Parameters.AddWithValue("@mn", info.PersonalDetails.MiddleName);
				using (var reader = cmd.ExecuteReader())
				{
					if (reader.Read())
						return (bool) reader["Crashed"];
				}
			}
			return null;
		}

		static SqlConnection OpenConnection()
		{
			return new SqlConnection(ConfigurationManager.ConnectionStrings["sql"].ConnectionString);
		}
		
		public void AddOrUpdateClient(InsuranceInfo info)
		{
			using (var conn = OpenConnection())
			{
				conn.Open();

				string cmdText;
				if (Crashed(info, conn) != null)
					cmdText = "UPDATE Clients SET Crashed = 0";
				else
					cmdText =
						@"INSERT INTO Clients 
					(FirstName, LastName, MiddleName, ModelName, Passport, StateId, Crashed) VALUES (@fn, @ln, @mn, @modName, @pass, @stId, 0)";
				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandText = cmdText;
					cmd.Parameters.AddWithValue("@fn", info.PersonalDetails.FirstName);
					cmd.Parameters.AddWithValue("@ln", info.PersonalDetails.LastName);
					cmd.Parameters.AddWithValue("@mn", info.PersonalDetails.MiddleName);
					cmd.Parameters.AddWithValue("@modName", info.CarDetails.ModelName);
					cmd.Parameters.AddWithValue("@pass", info.CarDetails.Passport);
					cmd.Parameters.AddWithValue("@stId", info.CarDetails.StateId);

					cmd.ExecuteNonQuery();
				}
				
			}
		}

		public bool WasWithoutCrashes(InsuranceInfo info)
		{
			using (var conn = OpenConnection())
			{
				conn.Open();
				return Crashed(info, conn) == false;
			}
		}
	}
}